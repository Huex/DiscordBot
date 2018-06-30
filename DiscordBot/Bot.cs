using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
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
        private readonly List<Packet> _packets = new List<Packet>();

        public DiscordSocketClient Discord { get; private set; }
        public BotConfig Config { get; private set; }

        public async Task StartAsync()
        {
            await Discord.StartAsync();
        }

        public async Task LoginAsync()
        {
            await Discord.LoginAsync(TokenType.Bot, Config.Token);
        }

        public Bot(BotConfig config, Collection<Packet> packets)
        {
            _packets = new List<Packet>(packets);
            Config = config;
            Discord = new DiscordSocketClient(Config.DiscordSocket);
            Discord.Log += (p) =>
            {
                RaiseLog(new LogMessage(p));
                return Task.CompletedTask;
            };
            foreach(var packet in packets)
            {
                foreach(var handler in packet.ChannelCreated)
                {
                    Discord.ChannelCreated += handler;
                }
                foreach (var handler in packet.ChannelDestroyed)
                {
                    Discord.ChannelDestroyed += handler;
                }
                foreach (var handler in packet.ChannelUpdated)
                {
                    Discord.ChannelUpdated += handler;
                }
                foreach (var handler in packet.Connected)
                {
                    Discord.Connected += handler;
                }
                foreach (var handler in packet.CurrentUserUpdated)
                {
                    Discord.CurrentUserUpdated += handler;
                }
                foreach (var handler in packet.Disconnected)
                {
                    Discord.Disconnected += handler;
                }
                foreach (var handler in packet.GuildAvailable)
                {
                    Discord.GuildAvailable += handler;
                }
                foreach (var handler in packet.GuildMembersDownloaded)
                {
                    Discord.GuildMembersDownloaded += handler;
                }
                foreach (var handler in packet.GuildMemberUpdated)
                {
                    Discord.GuildMemberUpdated += handler;
                }
                foreach (var handler in packet.GuildUnavailable)
                {
                    Discord.GuildUnavailable += handler;
                }
                foreach (var handler in packet.GuildUpdated)
                {
                    Discord.GuildUpdated += handler;
                }
                foreach (var handler in packet.JoinedGuild)
                {
                    Discord.JoinedGuild += handler;
                }
                foreach (var handler in packet.LeftGuild)
                {
                    Discord.LeftGuild += handler;
                }
                foreach (var handler in packet.LoggedIn)
                {
                    Discord.LoggedIn += handler;
                }
                foreach (var handler in packet.LoggedOut)
                {
                    Discord.LoggedOut += handler;
                }
                foreach (var handler in packet.MessageUpdated)
                {
                    Discord.MessageUpdated += handler;
                }
                foreach (var handler in packet.MessageReceived)
                {
                    Discord.MessageReceived += handler;
                }
                foreach (var handler in packet.ReactionAdded)
                {
                    Discord.ReactionAdded += handler;
                }
                foreach (var handler in packet.ReactionRemoved)
                {
                    Discord.ReactionRemoved += handler;
                }
                foreach (var handler in packet.ReactionsCleared)
                {
                    Discord.ReactionsCleared += handler;
                }
                foreach (var handler in packet.Ready)
                {
                    Discord.Ready += handler;
                }
                foreach (var handler in packet.RecipientAdded)
                {
                    Discord.RecipientAdded += handler;
                }
                foreach (var handler in packet.RecipientRemoved)
                {
                    Discord.RecipientRemoved += handler;
                }
                foreach (var handler in packet.RoleCreated)
                {
                    Discord.RoleCreated += handler;
                }
                foreach (var handler in packet.RoleDeleted)
                {
                    Discord.RoleDeleted += handler;
                }
                foreach (var handler in packet.RoleUpdated)
                {
                    Discord.RoleUpdated += handler;
                }
                foreach (var handler in packet.UserBanned)
                {
                    Discord.UserBanned += handler;
                }
                foreach (var handler in packet.UserIsTyping)
                {
                    Discord.UserIsTyping += handler;
                }
                foreach (var handler in packet.UserJoined)
                {
                    Discord.UserJoined += handler;
                }
                foreach (var handler in packet.UserLeft)
                {
                    Discord.UserLeft += handler;
                }
                foreach (var handler in packet.UserUnbanned)
                {
                    Discord.UserUnbanned += handler;
                }
                foreach (var handler in packet.UserUpdated)
                {
                    Discord.UserUpdated += handler;
                }
                foreach (var handler in packet.UserVoiceStateUpdated)
                {
                    Discord.UserVoiceStateUpdated += handler;
                }
            }
        }
    }
}
