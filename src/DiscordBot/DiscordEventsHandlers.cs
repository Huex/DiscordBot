using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class DiscordEventsHandlers
    {
        public Func<SocketChannel, Task> ChannelCreated { get; set; } = new Func<SocketChannel, Task>((a)=>Task.CompletedTask);
        public Func<SocketChannel, Task> ChannelDestroyed { get; set; } = new Func<SocketChannel, Task>((a) => Task.CompletedTask);
        public Func<SocketChannel, SocketChannel, Task> ChannelUpdated { get; set; } = new Func<SocketChannel, SocketChannel, Task>((a, b) => Task.CompletedTask);
        public Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated { get; set; } = new Func<SocketSelfUser, SocketSelfUser, Task>((a, b) => Task.CompletedTask);
        public Func<Task> Connected { get; set; } = new Func<Task>(() => Task.CompletedTask);
        public Func<Exception, Task> Disconnected { get; set; } = new Func<Exception, Task>((a) => Task.CompletedTask);
        public Func<SocketGuild, Task> GuildAvailable { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        public Func<SocketGuild, Task> GuildMembersDownloaded { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        public Func<SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated { get; set; } = new Func<SocketGuildUser, SocketGuildUser, Task>((a, b) => Task.CompletedTask);
        public Func<SocketGuild, Task> GuildUnavailable { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        public Func<SocketGuild, SocketGuild, Task> GuildUpdated { get; set; } = new Func<SocketGuild, SocketGuild, Task>((a, b) => Task.CompletedTask);
        public Func<SocketGuild, Task> JoinedGuild { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        public Func<SocketGuild, Task> LeftGuild { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        public Func<Task> LoggedIn { get; set; } = new Func<Task>(() => Task.CompletedTask);
        public Func<Task> LoggedOut { get; set; } = new Func<Task>(() => Task.CompletedTask);
        public Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted { get; set; } = new Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task>((a, b) => Task.CompletedTask);
        public Func<SocketMessage, Task> MessageReceived { get; set; } = new Func<SocketMessage, Task>((a) => Task.CompletedTask);
        public Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated { get; set; } = new Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task>((a, b, c) => Task.CompletedTask);
        public Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded { get; set; } = new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>((a, b, c) => Task.CompletedTask);
        public Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved { get; set; } = new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>((a, b, c) => Task.CompletedTask);
        public Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared { get; set; } = new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task>((a, b) => Task.CompletedTask);
        public Func<Task> Ready { get; set; } = new Func<Task>(() => Task.CompletedTask);
        public Func<SocketGroupUser, Task> RecipientAdded { get; set; } = new Func<SocketGroupUser, Task>((a) => Task.CompletedTask);
        public Func<SocketGroupUser, Task> RecipientRemoved { get; set; } = new Func<SocketGroupUser, Task>((a) => Task.CompletedTask);
        public Func<SocketRole, Task> RoleCreated { get; set; } = new Func<SocketRole, Task>((a) => Task.CompletedTask);
        public Func<SocketRole, Task> RoleDeleted { get; set; } = new Func<SocketRole, Task>((a) => Task.CompletedTask);
        public Func<SocketRole, SocketRole, Task> RoleUpdated { get; set; } = new Func<SocketRole, SocketRole, Task>((a, b) => Task.CompletedTask);
        public Func<SocketUser, SocketGuild, Task> UserBanned { get; set; } = new Func<SocketUser, SocketGuild, Task>((a, b) => Task.CompletedTask);
        public Func<SocketUser, ISocketMessageChannel, Task> UserIsTyping { get; set; } = new Func<SocketUser, ISocketMessageChannel, Task>((a, b) => Task.CompletedTask);
        public Func<SocketGuildUser, Task> UserJoined { get; set; } = new Func<SocketGuildUser, Task>((a) => Task.CompletedTask);
        public Func<SocketGuildUser, Task> UserLeft { get; set; } = new Func<SocketGuildUser, Task>((a) => Task.CompletedTask);
        public Func<SocketUser, SocketGuild, Task> UserUnbanned { get; set; } = new Func<SocketUser, SocketGuild, Task>((a, b) => Task.CompletedTask);
        public Func<SocketUser, SocketUser, Task> UserUpdated { get; set; } = new Func<SocketUser, SocketUser, Task>((a, b) => Task.CompletedTask);
        public Func<SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated { get; set; } = new Func<SocketUser, SocketVoiceState, SocketVoiceState, Task>((a, b, c) => Task.CompletedTask);
    }
}
