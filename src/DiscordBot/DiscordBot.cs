using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class DiscordBot : ILogEntity, ICommandConfigsModOnlyProvider, ICommandServicesReadOnlyProvider
    {
        private readonly string _token;
        private readonly DiscordClient _discord;
        private readonly List<PacketBase> _packets;
        private readonly LogRaiser _log;

        private readonly Dictionary<ulong, CommandHandler> _commandHandlers = new Dictionary<ulong, CommandHandler>();

        private readonly ServiceCollection _services = new ServiceCollection();
        private readonly CommandService _guildModules = new CommandService();
        private readonly CommandService _dmModules = new CommandService();
       
        public BotConfig Config { get; }

        public event Func<LogMessage, Task> Log;

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

        #region ICommandServiceReadOnlyProvider
        IReadOnlyDictionary<ulong, IPacketCommandService> ICommandServicesReadOnlyProvider.CommandServices
        {
            get
            {
                var commands = new Dictionary<ulong, IPacketCommandService>();
                foreach (var handler in _commandHandlers.Values)
                {
                    commands.Add(handler.Config.Id, handler.Commands);
                }
                return commands;
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
            _log = new LogRaiser(GetType().Name, async (msg) => await Log?.Invoke(msg));

            if (packets == null)
            {
                throw new ArgumentNullException(nameof(packets));
            }

            _token = config.Token;
            config.Token = "";
            Config = config;
            _discord = new DiscordClient(Config.DiscordSocket);
            _packets = new List<PacketBase>(packets);
            _discord.Log += _log.RaiseAsync;
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
        private async Task AddCommandHandlerIfNotExistsAsync(IEntity<ulong> source)
        {
            switch (source)
            {
                case SocketGuild guild:
                    if (!_commandHandlers.ContainsKey(guild.Id))
                    {
                        await AddCommandHandlerAsync(new CommandConfigBuilder(await GetCommandConfigFromProviderAsync(CommandSource.Guild, guild.Id))
                        {
                            Id = guild.Id,
                            Name = guild.Name,
                            Source = CommandSource.Guild
                        }.Build(), _services, _guildModules);
                    }
                    break;
                case IDMChannel channel:
                    if (!_commandHandlers.ContainsKey(channel.Recipient.Id))
                    {
                        await AddCommandHandlerAsync(new CommandConfigBuilder(await GetCommandConfigFromProviderAsync(CommandSource.User, channel.Recipient.Id))
                        {
                            Id = channel.Recipient.Id,
                            Name = channel.Recipient.Username,
                            Source = CommandSource.User
                        }.Build(), _services, _dmModules);
                    }
                    break;
                default:
                    await _log.RaiseAsync(LogSeverity.Warning, $"Can't add command handler with unknown source");
                    break;
            }
        }

        private async Task RemoveCommandHandlerAsync(IEntity<ulong> source)
        {
            switch (source)
            {
                case SocketGuild guild:
                    if (_commandHandlers.ContainsKey(guild.Id))
                    {
                        _commandHandlers.Remove(guild.Id);
                    }
                    break;
                case IDMChannel channel:
                    if (_commandHandlers.ContainsKey(channel.Recipient.Id))
                    {
                        _commandHandlers.Remove(channel.Recipient.Id);
                    }
                    break;
                default:
                    await _log.RaiseAsync(LogSeverity.Warning, $"Can't remove command handler with unknown source");
                    break;
            }
        }

        private async Task UpdateCommandHandlerConfigAsync(SocketChannel oldHandlerSource, SocketChannel newHandlerSource)
        {
            if((oldHandlerSource is IDMChannel) && (newHandlerSource is IDMChannel))
            {
                await UpdateCommandHandlerAsync(oldHandlerSource, newHandlerSource, (oldHandlerSource as IDMChannel).Recipient.Id, (newHandlerSource as IDMChannel).Recipient.Id);
            }        
        }

        private async Task UpdateCommandHandlerConfigAsync(SocketGuild oldHandlerSource, SocketGuild newHandlerSource)
        {
            await UpdateCommandHandlerAsync(oldHandlerSource, newHandlerSource, oldHandlerSource.Id, newHandlerSource.Id);
        }

        private async Task UpdateCommandHandlerAsync(IEntity<ulong> oldHandlerSource, IEntity<ulong> newHandlerSource, ulong oldId, ulong newId)
        {
            if (_commandHandlers.ContainsKey(oldId))
            {
                if (oldId == newId)
                {
                    await UpdateCommandHandlerIfExistsAsync(newHandlerSource);
                }
                else
                {
                    await RemoveCommandHandlerAsync(oldHandlerSource);
                    await AddCommandHandlerIfNotExistsAsync(newHandlerSource);
                }
            }
        }

        private async Task AddDMCommandHandlersAsync()
        {
            foreach (var channel in await _discord.GetDMChannelsAsync())
            {
                await AddCommandHandlerIfNotExistsAsync(channel).ConfigureAwait(true);
            }
        }

        private async Task HandleMessageAsync(SocketMessage arg)
        {
            if (arg is SocketUserMessage userMessage)
            {
                var context = new SocketCommandContext(_discord, userMessage);
                ulong id = context.Guild != null ? context.Guild.Id : context.User.Id;
                var handler = _commandHandlers.GetValueOrDefault(id, null);
                if(handler != null)
                {
                    handler.HandleMessage(arg).ConfigureAwait(false);
                }
                else
                {
                    await _log.RaiseAsync(new LogMessage(LogSeverity.Warning, this.GetType().Name, $"Missing command handler for {id}"));
                }
            }
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
                packet.Log += _log.RaiseAsync;
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
            _discord.GuildAvailable += AddCommandHandlerIfNotExistsAsync;
            _discord.JoinedGuild += AddCommandHandlerIfNotExistsAsync;
            _discord.GuildUnavailable += RemoveCommandHandlerAsync;                       
            _discord.GuildUpdated += UpdateCommandHandlerConfigAsync;

            _discord.ChannelCreated += AddCommandHandlerIfNotExistsAsync;
            _discord.ChannelUpdated += UpdateCommandHandlerConfigAsync;                     
            _discord.ChannelDestroyed += RemoveCommandHandlerAsync;

            _discord.Ready += AddDMCommandHandlersAsync;

            _discord.MessageReceived += HandleMessageAsync;
        }

        private async Task UpdateCommandHandlerIfExistsAsync(IEntity<ulong> newHandlerSource)
        {
            switch (newHandlerSource)
            {
                case SocketGuild newGuild:
                    UpdateCommandHandlerIfExist(new CommandConfig(CommandSource.Guild, newGuild.Name, newGuild.Id, _commandHandlers[newGuild.Id].Config.Prefix, _commandHandlers[newGuild.Id].Config.Modules));
                    break;
                case IDMChannel newChannel:
                    UpdateCommandHandlerIfExist(new CommandConfig(CommandSource.Guild, newChannel.Recipient.Username, newChannel.Recipient.Id, _commandHandlers[newChannel.Recipient.Id].Config.Prefix, _commandHandlers[newChannel.Recipient.Id].Config.Modules));
                    break;
                default:
                    await _log.RaiseAsync(LogSeverity.Warning, $"Can't update command handler with unknown source");
                    break;
            }
        }

        private void UpdateCommandHandlerIfExist(CommandConfig config)
        {
            if (_commandHandlers.ContainsKey(config.Id))
            {
                _commandHandlers[config.Id].Config = config;
            }           
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
                    ((ServiceBase)service.ImplementationInstance).Log -= _log.RaiseAsync;
                    ((ServiceBase)service.ImplementationInstance).Log += _log.RaiseAsync;
                }
                _services.Insert(_services.Count, service);
            }
        }

        private async Task<CommandConfig> GetCommandConfigFromProviderAsync(CommandSource source, ulong id)
        {
            CommandConfigBuilder res = null;
            foreach (var config in ConfigsProvider.CommandConfigs.Values)
            {
                if (config.Id == id)
                {
                    res = new CommandConfigBuilder(config);
                }
            }
            if (res == null)
            {
                switch (source)
                {
                    case CommandSource.User:
                        res = Config.DefaultUserCommandConfig;
                        break;
                    case CommandSource.Guild:
                        res = Config.DefaultGuildCommandConfig;
                        break;
                    default:
                        await _log.RaiseAsync(LogSeverity.Warning, $"Can't get command config with unknown source");
                        break;

                }
            }
            return res.Build();
        }

        private async Task AddCommandHandlerAsync(CommandConfig config, ServiceCollection services, CommandService modules)
        {
            CommandHandler handler = new CommandHandler(_discord, services.BuildServiceProvider(), modules, config);
            AddCommandHandler(handler);
            SubscribeCommandConfigsProviderOnChanges(handler);
            ConfigsProvider?.AddCommandConfig(handler.Config);
            await _log.RaiseAsync(LogSeverity.Info, $"Command handler created for {handler.Config.Source.ToString().ToLower()} {handler.Config.Name}");
        }

        private void AddCommandHandler(CommandHandler handler)
        {
            handler.Log += _log.RaiseAsync;
            _commandHandlers.Add(handler.Config.Id, handler);
        }
    }
}
