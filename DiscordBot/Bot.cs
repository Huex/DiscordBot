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

        public DiscordSocketClient _discord;
        public BotConfig Config { get; private set; }

        public IReadOnlyList<IPacket> Packets => _packets;

        public async Task StartAsync()
        {
            await _discord.StartAsync();
        }

        public async Task LoginAsync()
        {
            await _discord.LoginAsync(TokenType.Bot, Config.Token);
        }

        public Bot(BotConfig config)
        {          
            Config = config;
            _discord = new DiscordSocketClient(Config.DiscordSocket);
            _discord.Log += (p) =>
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
            _discord.ChannelCreated += packet.EventHandlers.ChannelCreated;
            _discord.ChannelDestroyed += packet.EventHandlers.ChannelDestroyed;
            _discord.ChannelUpdated += packet.EventHandlers.ChannelUpdated;
            _discord.Connected += packet.EventHandlers.Connected;
            _discord.CurrentUserUpdated += packet.EventHandlers.CurrentUserUpdated;
            _discord.Disconnected += packet.EventHandlers.Disconnected;
            _discord.GuildAvailable += packet.EventHandlers.GuildAvailable;
            _discord.GuildMembersDownloaded += packet.EventHandlers.GuildMembersDownloaded;
            _discord.GuildMemberUpdated += packet.EventHandlers.GuildMemberUpdated;
            _discord.GuildUnavailable += packet.EventHandlers.GuildUnavailable;
            _discord.GuildUpdated += packet.EventHandlers.GuildUpdated;
            _discord.JoinedGuild += packet.EventHandlers.JoinedGuild;
            _discord.LeftGuild += packet.EventHandlers.LeftGuild;
            _discord.LoggedIn += packet.EventHandlers.LoggedIn;
            _discord.LoggedOut += packet.EventHandlers.LoggedOut;
            _discord.MessageUpdated += packet.EventHandlers.MessageUpdated;
            _discord.MessageReceived += packet.EventHandlers.MessageReceived;
            _discord.ReactionAdded += packet.EventHandlers.ReactionAdded;
            _discord.ReactionRemoved += packet.EventHandlers.ReactionRemoved;
            _discord.ReactionsCleared += packet.EventHandlers.ReactionsCleared;
            _discord.Ready += packet.EventHandlers.Ready;
            _discord.RecipientAdded += packet.EventHandlers.RecipientAdded;
            _discord.RecipientRemoved += packet.EventHandlers.RecipientRemoved;
            _discord.RoleCreated += packet.EventHandlers.RoleCreated;
            _discord.RoleDeleted += packet.EventHandlers.RoleDeleted;
            _discord.RoleUpdated += packet.EventHandlers.RoleUpdated;
            _discord.UserBanned += packet.EventHandlers.UserBanned;
            _discord.UserIsTyping += packet.EventHandlers.UserIsTyping;
            _discord.UserJoined += packet.EventHandlers.UserJoined;
            _discord.UserLeft += packet.EventHandlers.UserLeft;
            _discord.UserUnbanned += packet.EventHandlers.UserUnbanned;
            _discord.UserUpdated += packet.EventHandlers.UserUpdated;
            _discord.UserVoiceStateUpdated += packet.EventHandlers.UserVoiceStateUpdated;
        }
    }
}
