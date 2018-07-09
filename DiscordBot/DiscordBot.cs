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
            foreach (var packet in _packets)
            {
                packet.Log += RaiseLogAsync;
                packet.Discord = Discord;
                SubscribeEventsHandlersByPacket(packet);
            }

            InitCommandsHandler();
        }

        private void InitCommandsHandler()
        {
            Discord.GuildAvailable += GuildAvailable;
            Discord.MessageReceived += HandleMessage;
            foreach (var packet in _packets)
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
        }

        private Task GuildAvailable(SocketGuild guild)
        {
            CommandHandler handler = new CommandHandler(Discord, _guildServices, _guildModules, Config.DefaultPrefix, guild.Id);
            handler.Log += RaiseLogAsync;
            _commandHandlers.Add(handler);
            return Task.CompletedTask;
        }

        private Task HandleMessage(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg) || msg.Source != MessageSource.User)
            {
                return Task.CompletedTask;
            }
            var guild = (new SocketCommandContext(Discord, msg)).Guild;
            if (guild != null)
            {
                CommandHandler guildWorker = _commandHandlers.Find(p => p.Id == guild.Id);
                guildWorker?.HandleMessage(arg).ConfigureAwait(false);
            }
            else
            {
                msg.Channel.SendMessageAsync("чо?");
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
