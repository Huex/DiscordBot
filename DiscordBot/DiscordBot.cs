using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class DiscordBot : LogEntity
    {
        private DiscordSocketClient _discord;

        private readonly List<Packet> _packets;
        private readonly NotifyDictonary<ulong, CommandHandler> _commandHandlers = new NotifyDictonary<ulong, CommandHandler>();
        private readonly NotifyDictonary<ulong, CommandConfig> _commandConfigs = new NotifyDictonary<ulong, CommandConfig>();
        private readonly ServiceCollection _guildServices = new ServiceCollection();
        private readonly ServiceCollection _dmServices = new ServiceCollection();
        private readonly CommandService _guildModules = new CommandService();
        private readonly CommandService _dmModules = new CommandService();

        public IReadOnlyDictionary<ulong, CommandConfig> CommandConfigs => _commandConfigs;
        public event Action<ulong, CommandConfig> CommandConfigUpdated;


        public BotConfig Config { get; }

        public DiscordBot(BotConfig config, Dictionary<ulong, CommandConfig> commandConfigs, ICollection<Packet> packets)
        {
            _commandConfigs.ValueUpdated += CommandConfigsValueUpdated;
            config.Token = "";
            Config = config;
            _discord = new DiscordSocketClient(Config.DiscordSocket);
            _discord.Log += RaiseLogAsync;
            _packets = new List<Packet>(packets);
            InitPackets();
            InitCommandsHandler();
        }

        private void CommandConfigsValueUpdated(ulong id)
        {
            CommandConfigUpdated?.Invoke(id, _commandConfigs[id]);
        }

        private void InitPackets()
        {
            foreach (var packet in _packets)
            {
                packet.Log += RaiseLogAsync;
                packet.SetDiscordSocket(_discord);
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
            foreach(var channel in channels)
            {
                AddCommandHandler(CommandSource.User, channel.Id);
            }
        }

        private Task RemoveDMCommandHandler(SocketChannel arg)
        {
            if (arg is IDMChannel channel)
            {
                RemoveCommandHandler(channel.Recipient.Id);
            }
            return Task.CompletedTask;
        }

        private Task AddDMCommandHandler(SocketChannel arg)
        {
            if (arg is IDMChannel channel)
            {
                AddCommandHandler(CommandSource.User, channel.Recipient.Id);
            }       
            return Task.CompletedTask;
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
            AddCommandHandler(CommandSource.Guild, guild.Id);
            return Task.CompletedTask;
        }

        private void AddCommandHandler(CommandSource source, ulong id)
        {
            if (!_commandHandlers.ContainsKey(id))
            {
                switch (source)
                {
                    case CommandSource.User:
                        AddCommandHandler(new CommandHandler(_discord, _dmServices.BuildServiceProvider(), _dmModules, Config.DefaultPrefix, id));
                        break;
                    case CommandSource.Guild:
                        AddCommandHandler(new CommandHandler(_discord, _guildServices.BuildServiceProvider(), _guildModules, Config.DefaultPrefix, id));
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddCommandHandler(CommandHandler handler)
        {
            handler.Log += RaiseLogAsync;
            _commandHandlers.Add(handler.Id, handler);
            RaiseLog(LogSeverity.Debug, $"Command handler added, id user/guild = {handler.Id}");
        }

        private void ExtractCommandsData(Packet packet)
        {
            foreach (var service in packet.GuildCommands.Services)
            {
                _guildServices.AddSingleton(service);
            }
            foreach (var module in packet.GuildCommands.Modules)
            {
                _guildModules.AddModuleAsync(module);
            }
            foreach (var service in packet.DMCommands.Services)
            {
                _dmServices.AddSingleton(service);
            }
            foreach (var module in packet.DMCommands.Modules)
            {
                _guildModules.AddModuleAsync(module);
            }
        }

        private Task HandleMessage(SocketMessage arg)
        {
            if (arg is SocketUserMessage msg)
            {
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



        //private readonly Collection<Type> _avalaibleModules = new Collection<Type>
        //{
        //    typeof(SettingsModule)
        //};

        //private readonly Collection<Type> _defaultGuildModules = new Collection<Type>
        //{
        //    typeof(SettingsModule)
        //};

        //private static string GetNameFromModule(Type module)
        //{
        //    var attributes = new List<CustomAttributeData>(module.GetCustomAttributesData());
        //    var needed = attributes?.Find(p => p.AttributeType == typeof(NameAttribute));
        //    return (string)needed?.ConstructorArguments[0].Value;
        //}

        //public static Collection<string> GetNamesOfModules(Collection<Type> modules)
        //{
        //    var modulesNames = new Collection<string>();
        //    foreach (var module in modules)
        //    {
        //        if (module.IsSubclassOf(typeof(ModuleBase)))
        //        {
        //            modulesNames.Add(GetNameFromModule(module));
        //        }
        //    }
        //    return modulesNames;
        //}

        //private GuildConfig GetDefaultGuildConfig(SocketGuild guild)
        //{
        //    return new GuildConfig(guild.Name,guild.Id, Prefix, guild.Owner.Id, $"{guild.Owner.Username}#{guild.Owner.Discriminator}", GetNamesOfModules(_defaultGuildModules));
        //}

        private void SubscribeEventsHandlersByPacket(Packet packet)
        {
            _discord.ChannelCreated += packet.EventsHandlers.ChannelCreated;
            _discord.ChannelDestroyed += packet.EventsHandlers.ChannelDestroyed;
            _discord.ChannelUpdated += packet.EventsHandlers.ChannelUpdated;
            _discord.Connected += packet.EventsHandlers.Connected;
            _discord.CurrentUserUpdated += packet.EventsHandlers.CurrentUserUpdated;
            _discord.Disconnected += packet.EventsHandlers.Disconnected;
            _discord.GuildAvailable += packet.EventsHandlers.GuildAvailable;
            _discord.GuildMembersDownloaded += packet.EventsHandlers.GuildMembersDownloaded;
            _discord.GuildMemberUpdated += packet.EventsHandlers.GuildMemberUpdated;
            _discord.GuildUnavailable += packet.EventsHandlers.GuildUnavailable;
            _discord.GuildUpdated += packet.EventsHandlers.GuildUpdated;
            _discord.JoinedGuild += packet.EventsHandlers.JoinedGuild;
            _discord.LeftGuild += packet.EventsHandlers.LeftGuild;
            _discord.LoggedIn += packet.EventsHandlers.LoggedIn;
            _discord.LoggedOut += packet.EventsHandlers.LoggedOut;
            _discord.MessageUpdated += packet.EventsHandlers.MessageUpdated;
            _discord.MessageReceived += packet.EventsHandlers.MessageReceived;
            _discord.ReactionAdded += packet.EventsHandlers.ReactionAdded;
            _discord.ReactionRemoved += packet.EventsHandlers.ReactionRemoved;
            _discord.ReactionsCleared += packet.EventsHandlers.ReactionsCleared;
            _discord.Ready += packet.EventsHandlers.Ready;
            _discord.RecipientAdded += packet.EventsHandlers.RecipientAdded;
            _discord.RecipientRemoved += packet.EventsHandlers.RecipientRemoved;
            _discord.RoleCreated += packet.EventsHandlers.RoleCreated;
            _discord.RoleDeleted += packet.EventsHandlers.RoleDeleted;
            _discord.RoleUpdated += packet.EventsHandlers.RoleUpdated;
            _discord.UserBanned += packet.EventsHandlers.UserBanned;
            _discord.UserIsTyping += packet.EventsHandlers.UserIsTyping;
            _discord.UserJoined += packet.EventsHandlers.UserJoined;
            _discord.UserLeft += packet.EventsHandlers.UserLeft;
            _discord.UserUnbanned += packet.EventsHandlers.UserUnbanned;
            _discord.UserUpdated += packet.EventsHandlers.UserUpdated;
            _discord.UserVoiceStateUpdated += packet.EventsHandlers.UserVoiceStateUpdated;
            _discord.MessageDeleted += packet.EventsHandlers.MessageDeleted;
        }
    }
}
