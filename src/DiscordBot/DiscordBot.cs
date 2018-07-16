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
       
        public BotConfig Config { get; }

        private ICommandConfigsProvider _configsProvider;
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

        #region ICommandConfigsModOnlyProvider
        IReadOnlyDictionary<ulong, CommandConfig> ICommandConfigsModOnlyProvider.CommandConfigs
        {
            get
            {
                var configs = new Dictionary<ulong, CommandConfig>();
                foreach(var handler in _commandHandlers.Values)
                {
                    configs.Add(handler.Config.Id, handler.Config);
                }
                return configs;
            }
        }

        void ICommandConfigsModOnlyProvider.UpdateCommandConfig(ulong id, string prefix, IEnumerable<string> modules)
        {
            if (_commandHandlers.ContainsKey(id))
            {
                _commandHandlers[id].Config = new CommandConfig(_commandHandlers[id].Config.Source, _commandHandlers[id].Config.Name, _commandHandlers[id].Config.Id, prefix, modules);
            }
        }
        #endregion

        #region Constructors
        public DiscordBot(BotConfig config, ICommandConfigsProvider configsProvider, ICollection<PacketBase> packets) : this(config, packets)
        {
            ConfigsProvider = configsProvider;
        }

        public DiscordBot(BotConfig config, ICollection<PacketBase> packets)
        {
            if (packets == null)
            {
                throw new ArgumentNullException(nameof(packets));
            }

            _token = config.Token;
            config.Token = "";
            Config = config;
            _discord = new DiscordClient(Config.DiscordSocket);
            _packets = new List<PacketBase>(packets);
            _discord.Log += RaiseLogAsync;
            InitPackets();
            InitCommandsHandler();
        }
        #endregion

        #region Public methods
        public async Task StartAsync()
        {
            await _discord.LoginAsync(TokenType.Bot, _token);
            await _discord.StartAsync();
        }


        #endregion

        #region Discord events
        private Task AddCommandHandler(SocketGuild guild)
        {
            if (!_commandHandlers.ContainsKey(guild.Id))
            {
                AddCommandHandler(CommandSource.Guild, guild);
            }
            return Task.CompletedTask;
        }

        private async Task AddDMCommandHandlersAsync()
        {
            var channels = await _discord.GetDMChannelsAsync();
            foreach (var channel in channels)
            {
                AddCommandHandler(CommandSource.User, channel);
            }
        }

        private Task RemoveCommandHandler(SocketGuild arg)
        {
            RemoveCommandHandler(arg.Id);
            return Task.CompletedTask;
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

        private Task AddDMCommandHandler(SocketChannel arg)
        {
            if (!DMCommandHandlerExsist(arg))
            {
                AddCommandHandler(CommandSource.User, arg);
            }
            return Task.CompletedTask;
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
        #endregion

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

        private void InitPackets()
        {
            foreach (var packet in _packets)
            {
                packet.Log += RaiseLogAsync;
                packet.InitPacket(Config, _discord, this);
            }
        }

        private void InitCommandsHandler()
        {
            SubscribeCommandHandlerOnDiscordEvents();
            foreach (var packet in _packets)
            {
                ExtractCommandsData(packet);
            }
        }

        private void SubscribeCommandHandlerOnDiscordEvents()
        {
            _discord.GuildAvailable += AddCommandHandler;
            _discord.Ready += AddDMCommandHandlersAsync;
            _discord.JoinedGuild += AddCommandHandler;
            _discord.GuildUnavailable += RemoveCommandHandler;
            _discord.MessageReceived += HandleMessage;
            _discord.ChannelCreated += AddDMCommandHandler;
            _discord.ChannelDestroyed += RemoveDMCommandHandler;
        }

        private void ExtractCommandsData(PacketBase packet)
        {
            ExtractServices(packet.Services);
            ExtractModules(packet.DMModules, _dmModules);
            ExtractModules(packet.GuildModules, _guildModules);
        }

        private void ExtractModules(Collection<Type> modules, CommandService service)
        {
            foreach (var module in modules)
            {
                if (module.IsSubclassOf(typeof(ModuleBase)))
                {
                    service.AddModuleAsync(module);
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

        private bool DMCommandHandlerExsist(SocketChannel arg)
        {
            if (arg is IDMChannel)
            {
                if (_commandHandlers.ContainsKey((arg as IDMChannel).Recipient.Id))
                {
                    return true;
                }
            }
            return false;
        }

        private void RemoveCommandHandler(ulong id)
        {
            if (_commandHandlers.ContainsKey(id))
            {
                _commandHandlers.Remove(id);
            }
        }

        private void AddCommandHandler(CommandSource source, IEntity<ulong> socket)
        {
            if (!_commandHandlers.ContainsKey(socket.Id))
            {
                switch (source)
                {
                    case CommandSource.Guild:
                        AddCommandHandler(new CommandConfigBuilder(GetCommandConfigFromProvider(CommandSource.Guild, (socket as SocketGuild).Id))
                        {
                            Id = (socket as SocketGuild).Id,
                            Name = (socket as SocketGuild).Name,
                            Source = CommandSource.Guild
                        }.Build(), _services, _guildModules);
                        break;
                    case CommandSource.User:
                        AddCommandHandler(new CommandConfigBuilder(GetCommandConfigFromProvider(CommandSource.User, (socket as IDMChannel).Recipient.Id))
                        {
                            Id = (socket as IDMChannel).Recipient.Id,
                            Name = (socket as IDMChannel).Recipient.Username,
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
            foreach(var config in ConfigsProvider.CommandConfigs.Values)
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
    }
}
