using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DiscordBot.Packets.Sample;

namespace DiscordBot.Core
{
    public class DiscordBot : LogEntity, ICommandConfigsModOnlyProvider
    {
        private readonly DiscordClient _discord;
        private readonly List<PacketBase> _packets;
        private readonly string _token;
        private readonly Dictionary<ulong, CommandHandler> _commandHandlers = new Dictionary<ulong, CommandHandler>();
        private readonly ServiceCollection _services = new ServiceCollection();
        private readonly CommandService _guildModules = new CommandService();
        private readonly CommandService _dmModules = new CommandService();
        private ICommandConfigsProvider _configsProvider;

        public BotConfig Config { get; }

        public ICommandConfigsProvider ConfigsProvider
        {
            get
            {
                return _configsProvider;
            }
            set
            {
                _configsProvider = value;
                SubscribeCommandConfigsProviderOnChanges();
            }
        }

        IReadOnlyCollection<CommandConfig> ICommandConfigsModOnlyProvider.Configs
        {
            get
            {
                var configs = new Collection<CommandConfig>();
                foreach(var handler in _commandHandlers.Values)
                {
                    configs.Add(handler.Config);
                }
                return configs;
            }
        }

        void ICommandConfigsModOnlyProvider.UpdateCommandConfig(ulong id, CommandConfig config) //доделать ексепшн
        {
            if (CommandConfigExsist(id))
            {
                _commandHandlers[id].Config = config;
            }
        }

        public DiscordBot(BotConfig config, ICommandConfigsProvider configsProvider, ICollection<PacketBase> packets) : this(config, packets)
        {
            ThrowArgumentExceptionIfNull(configsProvider);
            ConfigsProvider = configsProvider;
        }

        public DiscordBot(BotConfig config, ICollection<PacketBase> packets)
        {
            ThrowArgumentExceptionIfNull(packets);
            _token = config.Token;
            config.Token = "";
            Config = config;
            _discord = new DiscordClient(Config.DiscordSocket);
            _packets = new List<PacketBase>(packets);
            _discord.Log += RaiseLogAsync;
            InitPackets();
            InitCommandsHandler();
        }

        public void UpdateCommandHandlerConfig(ulong id, CommandConfig config)
        {
            if (_commandHandlers.ContainsKey(id))
            {
                if (_commandHandlers[id].Config != config)
                {
                    _commandHandlers[id].Config = config;
                    ConfigsProvider?.UpdateCommandConfig(_commandHandlers[id].Config.Id, _commandHandlers[id].Config);
                }
            }
        }

        public async Task StartAsync()
        {
            await _discord.LoginAsync(TokenType.Bot, _token);
            await _discord.StartAsync();
        }

        public CommandConfig GetCommandConfig(ulong id) //доделать ексепшн
        {
            if (CommandConfigExsist(id))
            {
                return _commandHandlers[id].Config;
            }
            throw new Exception();
        }

        public bool CommandConfigExsist(ulong id)
        {
            return _commandHandlers.ContainsKey(id);
        }

        private void SubscribeCommandConfigsProviderOnChanges()
        {
            if (ConfigsProvider != null)
            {
                foreach (var handler in _commandHandlers)
                {
                    SubscribeCommandConfigsProviderOnChanges(handler.Value);
                }
            }
        }

        private void SubscribeCommandConfigsProviderOnChanges(CommandHandler handler)
        {
            if (ConfigsProvider != null)
            {
                handler.ConfigUpdated += ConfigsProvider.UpdateCommandConfig;
            }
        }

        private static void ThrowArgumentExceptionIfNull(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
        }

        private void InitPackets()
        {
            foreach (var packet in _packets)
            {
                packet.Log += RaiseLogAsync;
                packet.InitPacket(_discord, this);
            }
        }

        private void InitCommandsHandler()
        {
            _discord.GuildAvailable += AddCommandHandler;
            _discord.Ready += AddDMCommandHandlersAsync;
            _discord.JoinedGuild += AddCommandHandler;
            _discord.GuildUnavailable += RemoveCommandHandler;
            _discord.MessageReceived += HandleMessage;
            _discord.ChannelCreated += AddDMCommandHandler;
            _discord.ChannelDestroyed += RemoveDMCommandHandler;
            foreach (var packet in _packets)
            {
                ExtractCommandsData(packet);
            }
        }

        private async Task AddDMCommandHandlersAsync()
        {
            var channels = await _discord.GetDMChannelsAsync();
            foreach (var channel in channels)
            {
                AddCommandHandler(CommandSource.User, channel);
            }
        }

        private Task RemoveDMCommandHandler(SocketChannel arg)
        {
            if (DMCommandHandlerExsist(arg))
            {
                var channel = arg as IDMChannel;
                RemoveCommandHandler(channel.Recipient.Id);
            }
            return Task.CompletedTask;
        }

        private Task AddDMCommandHandler(SocketChannel arg)
        {
            if (!DMCommandHandlerExsist(arg))
            {
                AddCommandHandler(CommandSource.User, arg);
            }
            return Task.CompletedTask;
        }

        private bool DMCommandHandlerExsist(SocketChannel arg)
        {
            if (arg is IDMChannel)
            {
                var channel = arg as IDMChannel;
                if (_commandHandlers.ContainsKey(channel.Recipient.Id))
                {
                    return true;
                }
            }
            return false;
        }

        private Task RemoveCommandHandler(SocketGuild arg)
        {
            RemoveCommandHandler(arg.Id);
            return Task.CompletedTask;
        }

        private void RemoveCommandHandler(ulong id)
        {
            if (_commandHandlers.ContainsKey(id))
            {
                _commandHandlers.Remove(id);
            }
        }

        private Task AddCommandHandler(SocketGuild guild)
        {
            if (!_commandHandlers.ContainsKey(guild.Id))
            {
                AddCommandHandler(CommandSource.Guild, guild);
            }
            return Task.CompletedTask;
        }

        private void AddCommandHandler(CommandSource source, IEntity<ulong> socket)
        {
            if (!_commandHandlers.ContainsKey(socket.Id))
            {
                switch (source)
                {
                    case CommandSource.Guild:
                        var socketGuild = socket as SocketGuild;
                        AddCommandHandler(new CommandConfigBuilder(GetCommandConfigFromProvider(CommandSource.Guild, socketGuild.Id))
                        {
                            Id = socketGuild.Id,
                            Name = socketGuild.Name,
                            Source = CommandSource.Guild
                        }.Build(), _services, _guildModules);
                        break;
                    case CommandSource.User:
                        var channel = socket as IDMChannel;
                        AddCommandHandler(new CommandConfigBuilder(GetCommandConfigFromProvider(CommandSource.User, channel.Recipient.Id))
                        {
                            Id = channel.Recipient.Id,
                            Name = channel.Recipient.Username,
                            Source = CommandSource.User
                        }.Build(), _services, _dmModules);
                        break;
                    default:
                        break;
                }
            }
        }

        private CommandConfig GetCommandConfigFromProvider(CommandSource source, ulong id)
        {
            CommandConfigBuilder res = null;
            foreach(var config in ConfigsProvider.Configs)
            {
                if(config.Id == id)
                {
                    res = new CommandConfigBuilder(config);
                }
            }
            switch (source)
            {
                case CommandSource.User:
                    res = res ?? Config.DefaultUserCommandConfig;
                    break;
                case CommandSource.Guild:
                    res = res ?? Config.DefaultGuildCommandConfig;
                    break;
                default:
                    break;
            }  
            return res.Build();
        }

        private void AddCommandHandler(CommandConfig config, ServiceCollection services, CommandService modules)
        {
            CommandHandler handler = new CommandHandler(_discord, services.BuildServiceProvider(), modules, config);
            AddCommandHandler(handler);
            SubscribeCommandConfigsProviderOnChanges(handler);
            ConfigsProvider?.AddCommandConfig(handler.Config);
            RaiseLog(LogSeverity.Info, $"Command handler created for {handler.Config.Source.ToString().ToLower()} {handler.Config.Name}");
        }

        private void AddCommandHandler(CommandHandler handler)
        {
            handler.Log += RaiseLogAsync;
            _commandHandlers.Add(handler.Config.Id, handler);
        }

        private void ExtractCommandsData(PacketBase packet)
        {
            ExtractServices(packet.Services);
            ExtractModules(packet.DMModules);
            ExtractModules(packet.GuildModules);
        }

        private void ExtractModules(Collection<Type> modules)
        {
            foreach (var module in modules)
            {
                if (module.IsSubclassOf(typeof(ModuleBase)))
                {
                    _dmModules.AddModuleAsync(module);
                }        
            }
        }

        private void ExtractServices(ServiceCollection services)
        {
            foreach (var service in services)
            {
                if (service.ImplementationInstance is ServiceBase)
                {
                    ((ServiceBase)service.ImplementationInstance).Log -= RaiseLogAsync;
                    ((ServiceBase)service.ImplementationInstance).Log += RaiseLogAsync;
                }
                _services.Insert(_services.Count, service);
            }
        }

        private Task HandleMessage(SocketMessage arg)
        {
            if (arg is SocketUserMessage)
            {
                var msg = arg as SocketUserMessage;
                var context = new SocketCommandContext(_discord, msg);
                if (context.Guild != null)
                {
                    _commandHandlers.GetValueOrDefault(context.Guild.Id, null)?.HandleMessage(arg).ConfigureAwait(false);
                }
                else
                {
                    _commandHandlers.GetValueOrDefault(context.User.Id, null)?.HandleMessage(arg).ConfigureAwait(false);
                }
            }
            return Task.CompletedTask;
        }
    }
}
