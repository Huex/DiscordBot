using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Modules;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class Bot : BotServiceBase
    {
        //TODO: СДЕЛАТЬ ХРАНИЛИЩЕ
        private readonly List<IPacket> _packets = new List<IPacket>();

        public BotConfig Config { get; }

        public DiscordSocketClient Discord { get; }

        public IReadOnlyList<IPacket> Packets => _packets;

        public async Task StartAsync()
        {
            await Discord.StartAsync();
        }

        public async Task LoginAsync()
        {
            await Discord.LoginAsync(TokenType.Bot, Config.Token);
        }

        public Bot(BotConfig config)
        {          
            Config = config;
            Discord = new DiscordSocketClient(Config.DiscordSocket);
            Discord.Log += (p) =>
            {
                RaiseLog(new LogMessage(p));
                return Task.CompletedTask;
            };
        }

        public void AddPacket(IPacket packet)
        {
            _packets.Add(packet);
            SubscribeEventsHandlersByPacket(packet);
        }

        private void SubscribeEventsHandlersByPacket(IPacket packet)
        {
            Discord.ChannelCreated += packet.EventHandlers.ChannelCreated;
            Discord.ChannelDestroyed += packet.EventHandlers.ChannelDestroyed;
            Discord.ChannelUpdated += packet.EventHandlers.ChannelUpdated;
            Discord.Connected += packet.EventHandlers.Connected;
            Discord.CurrentUserUpdated += packet.EventHandlers.CurrentUserUpdated;
            Discord.Disconnected += packet.EventHandlers.Disconnected;
            Discord.GuildAvailable += packet.EventHandlers.GuildAvailable;
            Discord.GuildMembersDownloaded += packet.EventHandlers.GuildMembersDownloaded;
            Discord.GuildMemberUpdated += packet.EventHandlers.GuildMemberUpdated;
            Discord.GuildUnavailable += packet.EventHandlers.GuildUnavailable;
            Discord.GuildUpdated += packet.EventHandlers.GuildUpdated;
            Discord.JoinedGuild += packet.EventHandlers.JoinedGuild;
            Discord.LeftGuild += packet.EventHandlers.LeftGuild;
            Discord.LoggedIn += packet.EventHandlers.LoggedIn;
            Discord.LoggedOut += packet.EventHandlers.LoggedOut;
            Discord.MessageUpdated += packet.EventHandlers.MessageUpdated;
            Discord.MessageReceived += packet.EventHandlers.MessageReceived;
            Discord.ReactionAdded += packet.EventHandlers.ReactionAdded;
            Discord.ReactionRemoved += packet.EventHandlers.ReactionRemoved;
            Discord.ReactionsCleared += packet.EventHandlers.ReactionsCleared;
            Discord.Ready += packet.EventHandlers.Ready;
            Discord.RecipientAdded += packet.EventHandlers.RecipientAdded;
            Discord.RecipientRemoved += packet.EventHandlers.RecipientRemoved;
            Discord.RoleCreated += packet.EventHandlers.RoleCreated;
            Discord.RoleDeleted += packet.EventHandlers.RoleDeleted;
            Discord.RoleUpdated += packet.EventHandlers.RoleUpdated;
            Discord.UserBanned += packet.EventHandlers.UserBanned;
            Discord.UserIsTyping += packet.EventHandlers.UserIsTyping;
            Discord.UserJoined += packet.EventHandlers.UserJoined;
            Discord.UserLeft += packet.EventHandlers.UserLeft;
            Discord.UserUnbanned += packet.EventHandlers.UserUnbanned;
            Discord.UserUpdated += packet.EventHandlers.UserUpdated;
            Discord.UserVoiceStateUpdated += packet.EventHandlers.UserVoiceStateUpdated;
            Discord.MessageDeleted += packet.EventHandlers.MessageDeleted;
        }
    }
}
