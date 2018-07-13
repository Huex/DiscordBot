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
    public class DiscordBot : LogEntity
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

        public void UpdateCommandConfig(ulong id, CommandConfig config) //доделать ексепшн
        {
            if (CommandConfigExsist(id))
            {
                _commandHandlers[id].Config = config;
            }
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
                    SubscribeCommandConfigsProviderOn(handler.Value);
                }
            }
        }

        private void SubscribeCommandConfigsProviderOn(CommandHandler handler)
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
                packet.InitPacket((DiscordClient)_discord, UpdateCommandConfig, GetCommandConfig, CommandConfigExsist);
                SubscribeEventsHandlersByPacket(packet);
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
                    case CommandSource.User:
                        AddCommandHandler(socket as IDMChannel, _services, _dmModules);
                        break;
                    case CommandSource.Guild:
                        AddCommandHandler(socket as SocketGuild, _services, _guildModules);
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddCommandHandler(SocketGuild socketGuild, ServiceCollection guildServices, CommandService guildModules)
        {
            var config = ConfigsProvider?.GetCommandConfigIfExsist(socketGuild.Id);
            config = config != null ? config : Config.DefaultGuildCommandConfig.Build();
            var builder = new CommandConfigBuilder((CommandConfig)config)
            {
                Id = socketGuild.Id,
                Name = socketGuild.Name,
                Source = CommandSource.Guild
            };
            AddCommandHandler(builder.Build(), guildServices, guildModules);
        }

        private void AddCommandHandler(IDMChannel channel, ServiceCollection dmServices, CommandService dmModules)
        {
            var config = ConfigsProvider?.GetCommandConfigIfExsist(channel.Recipient.Id);
            config = config != null ? config : Config.DefaultUserCommandConfig.Build();
            var builder = new CommandConfigBuilder((CommandConfig)config)
            {
                Id = channel.Recipient.Id,
                Name = channel.Recipient.Username,
                Source = CommandSource.User
            };
            AddCommandHandler(builder.Build(), dmServices, dmModules);
        }

        private void AddCommandHandler(CommandConfig config, ServiceCollection services, CommandService modules)
        {
            CommandHandler handler = new CommandHandler(_discord, services.BuildServiceProvider(), modules, config);
            AddCommandHandler(handler);
            SubscribeCommandConfigsProviderOn(handler);
            ConfigsProvider?.CreateCommandConfig(handler.Config);
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

        private void SubscribeEventsHandlersByPacket(PacketBase packet)
        {
            //_discord.ChannelCreated += packet.ChannelCreated;
            //_discord.ChannelDestroyed += packet.ChannelDestroyed;
            //_discord.ChannelUpdated += packet.ChannelUpdated;
            //_discord.Connected += packet.Connected;
            //_discord.CurrentUserUpdated += packet.CurrentUserUpdated;
            //_discord.Disconnected += packet.Disconnected;
            //_discord.GuildAvailable += packet.GuildAvailable;
            //_discord.GuildMembersDownloaded += packet.GuildMembersDownloaded;
            //_discord.GuildMemberUpdated += packet.GuildMemberUpdated;
            //_discord.GuildUnavailable += packet.GuildUnavailable;
            //_discord.GuildUpdated += packet.GuildUpdated;
            //_discord.JoinedGuild += packet.JoinedGuild;
            //_discord.LeftGuild += packet.LeftGuild;
            //_discord.LoggedIn += packet.LoggedIn;
            //_discord.LoggedOut += packet.LoggedOut;
            //_discord.MessageUpdated += packet.MessageUpdated;
            //_discord.MessageReceived += packet.MessageReceived;
            //_discord.ReactionAdded += packet.ReactionAdded;
            //_discord.ReactionRemoved += packet.ReactionRemoved;
            //_discord.ReactionsCleared += packet.ReactionsCleared;
            //_discord.Ready += packet.Ready;
            //_discord.RecipientAdded += packet.RecipientAdded;
            //_discord.RecipientRemoved += packet.RecipientRemoved;
            //_discord.RoleCreated += packet.RoleCreated;
            //_discord.RoleDeleted += packet.RoleDeleted;
            //_discord.RoleUpdated += packet.RoleUpdated;
            //_discord.UserBanned += packet.UserBanned;
            //_discord.UserIsTyping += packet.UserIsTyping;
            //_discord.UserJoined += packet.UserJoined;
            //_discord.UserLeft += packet.UserLeft;
            //_discord.UserUnbanned += packet.UserUnbanned;
            //_discord.UserUpdated += packet.UserUpdated;
            //_discord.UserVoiceStateUpdated += packet.UserVoiceStateUpdated;
            //_discord.MessageDeleted += packet.MessageDeleted;
        }
    }
}
