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
    public class DiscordBot : LogEntity
    {
        private readonly List<Packet> _packets;
        private readonly List<CommandHandler> _commandHandlers = new List<CommandHandler>();
        private readonly ServiceCollection _guildServices = new ServiceCollection();
        private readonly ServiceCollection _dmServices = new ServiceCollection();
        private readonly Collection<Type> _guildModules = new Collection<Type>();
        private readonly Collection<Type> _dmModules = new Collection<Type>();

        public BotConfig Config { get; }
        public DiscordSocketClient Discord { get; }

        public DiscordBot(BotConfig config, ICollection<Packet> packets)
        {
            config.Token = "";
            Config = config;
            Discord = new DiscordSocketClient(Config.DiscordSocket);
            Discord.Log += RaiseLogAsync;
            _packets = new List<Packet>(packets);
            InitPackets();
            InitCommandsHandler();
        }

        private void InitPackets()
        {
            foreach (var packet in _packets)
            {
                packet.Log += RaiseLogAsync;
                packet.Discord = Discord;
                SubscribeEventsHandlersByPacket(packet);
            }
        }

        private void InitCommandsHandler()
        {
            Discord.GuildAvailable += AddCommandHandler;
            Discord.Ready += AddDMCommandHandlersAsync;
            Discord.JoinedGuild += AddCommandHandler;
            Discord.GuildUnavailable += RemoveCommandHandler;
            Discord.MessageReceived += HandleMessage;
            Discord.ChannelCreated += AddDMCommandHandler;
            Discord.ChannelDestroyed += RemoveDMCommandHandler;
            foreach (var packet in _packets)
            {
                ExtractCommandsData(packet);
            }
        }

        private async Task AddDMCommandHandlersAsync()
        {
            var channels = await Discord.GetDMChannelsAsync();
            foreach(var channel in channels)
            {
                AddCommandHandler(channel.Id);
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
                AddCommandHandler(channel.Recipient.Id);
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
            var handler = _commandHandlers.Find(g => g.Id == id);
            if (handler != null)
            {
                _commandHandlers.Remove(handler);
            }
        }

        private Task AddCommandHandler(SocketGuild guild)
        {
            AddCommandHandler(guild.Id);
            return Task.CompletedTask;
        }

        private void AddCommandHandler(ulong id)
        {
            if (!_commandHandlers.Exists(h => h.Id == id))
            {
                CommandHandler handler = new CommandHandler(Discord, _guildServices, _guildModules, Config.DefaultPrefix, id);
                handler.Log += RaiseLogAsync;
                _commandHandlers.Add(handler);
                RaiseLog(LogSeverity.Debug, $"Command handler added, id user/guild = {id}");
            }
        }

        private void ExtractCommandsData(Packet packet)
        {
            foreach (var service in packet.GuildCommands.Services)
            {
                _guildServices.AddSingleton(service);
            }
            foreach (var module in packet.GuildCommands.Modules)
            {
                _guildModules.Add(module);
            }
            foreach (var service in packet.DMCommands.Services)
            {
                _dmServices.AddSingleton(service);
            }
            foreach (var module in packet.DMCommands.Modules)
            {
                _guildModules.Add(module);
            }
        }

        private Task HandleMessage(SocketMessage arg)
        {
            if (arg is SocketUserMessage msg)
            {
                var context = new SocketCommandContext(Discord, msg);
                if (context.Guild != null)
                {
                    var handler = _commandHandlers.Find(p => p.Id == context.Guild.Id);
                    handler?.HandleMessage(arg).ConfigureAwait(false);
                }
                else
                {
                    var handler = _commandHandlers.Find(p => p.Id == context.User.Id);
                    handler?.HandleMessage(arg).ConfigureAwait(false);
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
            Discord.ChannelCreated += packet.EventsHandlers.ChannelCreated;
            Discord.ChannelDestroyed += packet.EventsHandlers.ChannelDestroyed;
            Discord.ChannelUpdated += packet.EventsHandlers.ChannelUpdated;
            Discord.Connected += packet.EventsHandlers.Connected;
            Discord.CurrentUserUpdated += packet.EventsHandlers.CurrentUserUpdated;
            Discord.Disconnected += packet.EventsHandlers.Disconnected;
            Discord.GuildAvailable += packet.EventsHandlers.GuildAvailable;
            Discord.GuildMembersDownloaded += packet.EventsHandlers.GuildMembersDownloaded;
            Discord.GuildMemberUpdated += packet.EventsHandlers.GuildMemberUpdated;
            Discord.GuildUnavailable += packet.EventsHandlers.GuildUnavailable;
            Discord.GuildUpdated += packet.EventsHandlers.GuildUpdated;
            Discord.JoinedGuild += packet.EventsHandlers.JoinedGuild;
            Discord.LeftGuild += packet.EventsHandlers.LeftGuild;
            Discord.LoggedIn += packet.EventsHandlers.LoggedIn;
            Discord.LoggedOut += packet.EventsHandlers.LoggedOut;
            Discord.MessageUpdated += packet.EventsHandlers.MessageUpdated;
            Discord.MessageReceived += packet.EventsHandlers.MessageReceived;
            Discord.ReactionAdded += packet.EventsHandlers.ReactionAdded;
            Discord.ReactionRemoved += packet.EventsHandlers.ReactionRemoved;
            Discord.ReactionsCleared += packet.EventsHandlers.ReactionsCleared;
            Discord.Ready += packet.EventsHandlers.Ready;
            Discord.RecipientAdded += packet.EventsHandlers.RecipientAdded;
            Discord.RecipientRemoved += packet.EventsHandlers.RecipientRemoved;
            Discord.RoleCreated += packet.EventsHandlers.RoleCreated;
            Discord.RoleDeleted += packet.EventsHandlers.RoleDeleted;
            Discord.RoleUpdated += packet.EventsHandlers.RoleUpdated;
            Discord.UserBanned += packet.EventsHandlers.UserBanned;
            Discord.UserIsTyping += packet.EventsHandlers.UserIsTyping;
            Discord.UserJoined += packet.EventsHandlers.UserJoined;
            Discord.UserLeft += packet.EventsHandlers.UserLeft;
            Discord.UserUnbanned += packet.EventsHandlers.UserUnbanned;
            Discord.UserUpdated += packet.EventsHandlers.UserUpdated;
            Discord.UserVoiceStateUpdated += packet.EventsHandlers.UserVoiceStateUpdated;
            Discord.MessageDeleted += packet.EventsHandlers.MessageDeleted;
        }
    }
}
