using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class Packet
    {
        public readonly Collection<Func<SocketChannel, Task>> ChannelCreated = new Collection<Func<SocketChannel, Task>>();
        public readonly Collection<Func<SocketChannel, Task>> ChannelDestroyed = new Collection<Func<SocketChannel, Task>>();
        public readonly Collection<Func<SocketChannel, SocketChannel, Task>> ChannelUpdated = new Collection<Func<SocketChannel, SocketChannel, Task>>();
        public readonly Collection<Func<SocketSelfUser, SocketSelfUser, Task>> CurrentUserUpdated = new Collection<Func<SocketSelfUser, SocketSelfUser, Task>>();
        public readonly Collection<Func<Task>> Connected = new Collection<Func<Task>>();
        public readonly Collection<Func<Exception, Task>> Disconnected = new Collection<Func<Exception, Task>>();
        public readonly Collection<Func<SocketGuild, Task>> GuildAvailable = new Collection<Func<SocketGuild, Task>>();
        public readonly Collection<Func<SocketGuild, Task>> GuildMembersDownloaded = new Collection<Func<SocketGuild, Task>>();
        public readonly Collection<Func<SocketGuildUser, SocketGuildUser, Task>> GuildMemberUpdated = new Collection<Func<SocketGuildUser, SocketGuildUser, Task>>();
        public readonly Collection<Func<SocketGuild, Task>> GuildUnavailable = new Collection<Func<SocketGuild, Task>>();
        public readonly Collection<Func<SocketGuild, SocketGuild, Task>> GuildUpdated = new Collection<Func<SocketGuild, SocketGuild, Task>>();
        public readonly Collection<Func<SocketGuild, Task>> JoinedGuild = new Collection<Func<SocketGuild, Task>>();
        public readonly Collection<Func<SocketGuild, Task>> LeftGuild = new Collection<Func<SocketGuild, Task>>();
        public readonly Collection<Func<Task>> LoggedIn = new Collection<Func<Task>>();
        public readonly Collection<Func<Task>> LoggedOut = new Collection<Func<Task>>();
        public readonly Collection<Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task>> MessageDeleted = new Collection<Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task>>();
        public readonly Collection<Func<SocketMessage, Task>> MessageReceived = new Collection<Func<SocketMessage, Task>>();
        public readonly Collection<Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task>> MessageUpdated = new Collection<Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task>>();
        public readonly Collection<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>> ReactionAdded = new Collection<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>>();
        public readonly Collection<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>> ReactionRemoved = new Collection<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>>();
        public readonly Collection<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task>> ReactionsCleared = new Collection<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task>>();
        public readonly Collection<Func<Task>> Ready = new Collection<Func<Task>>();
        public readonly Collection<Func<SocketGroupUser, Task>> RecipientAdded = new Collection<Func<SocketGroupUser, Task>>();
        public readonly Collection<Func<SocketGroupUser, Task>> RecipientRemoved = new Collection<Func<SocketGroupUser, Task>>();
        public readonly Collection<Func<SocketRole, Task>> RoleCreated = new Collection<Func<SocketRole, Task>>();
        public readonly Collection<Func<SocketRole, Task>> RoleDeleted = new Collection<Func<SocketRole, Task>>();
        public readonly Collection<Func<SocketRole, SocketRole, Task>> RoleUpdated = new Collection<Func<SocketRole, SocketRole, Task>>();
        public readonly Collection<Func<SocketUser, SocketGuild, Task>> UserBanned = new Collection<Func<SocketUser, SocketGuild, Task>>();
        public readonly Collection<Func<SocketUser, ISocketMessageChannel, Task>> UserIsTyping = new Collection<Func<SocketUser, ISocketMessageChannel, Task>>();
        public readonly Collection<Func<SocketGuildUser, Task>> UserJoined = new Collection<Func<SocketGuildUser, Task>>();
        public readonly Collection<Func<SocketGuildUser, Task>> UserLeft = new Collection<Func<SocketGuildUser, Task>>();
        public readonly Collection<Func<SocketUser, SocketGuild, Task>> UserUnbanned = new Collection<Func<SocketUser, SocketGuild, Task>>();
        public readonly Collection<Func<SocketUser, SocketUser, Task>> UserUpdated = new Collection<Func<SocketUser, SocketUser, Task>>();
        public readonly Collection<Func<SocketUser, SocketVoiceState, SocketVoiceState, Task>> UserVoiceStateUpdated = new Collection<Func<SocketUser, SocketVoiceState, SocketVoiceState, Task>>();
        public readonly Collection<BotServiceBase> Services = new Collection<BotServiceBase>();
        public readonly Collection<ModuleBase> Modules = new Collection<ModuleBase>();
    }
}
