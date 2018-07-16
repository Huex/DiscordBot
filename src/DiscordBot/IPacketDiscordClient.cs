using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordBot
{
    public interface IPacketDiscordClient
    {
        IReadOnlyCollection<RestVoiceRegion> VoiceRegions { get; }
        IReadOnlyCollection<SocketGroupChannel> GroupChannels { get; }
        IReadOnlyCollection<SocketDMChannel> DMChannels { get; }
        IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels { get; }
        IReadOnlyCollection<SocketGuild> Guilds { get; }
        SocketSelfUser CurrentUser { get; }
        int Latency { get; }
        ConnectionState ConnectionState { get; }
        int ShardId { get; }

        event Func<SocketGuild, SocketGuild, Task> GuildUpdated;
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;
        event Func<SocketRole, Task> RoleCreated;
        event Func<SocketRole, Task> RoleDeleted;
        event Func<SocketRole, SocketRole, Task> RoleUpdated;
        event Func<SocketGuild, Task> JoinedGuild;
        event Func<SocketUser, ISocketMessageChannel, Task> UserIsTyping;
        event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;
        event Func<SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;
        event Func<SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;
        event Func<SocketUser, SocketUser, Task> UserUpdated;
        event Func<SocketUser, SocketGuild, Task> UserUnbanned;
        event Func<SocketUser, SocketGuild, Task> UserBanned;
        event Func<SocketGuildUser, Task> UserLeft;
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;
        event Func<SocketGuild, Task> LeftGuild;
        event Func<SocketGuild, Task> GuildAvailable;
        event Func<SocketGuild, Task> GuildUnavailable;
        event Func<SocketGuild, Task> GuildMembersDownloaded;
        event Func<SocketGuildUser, Task> UserJoined;
        event Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;
        event Func<int, int, Task> LatencyUpdated;
        event Func<SocketMessage, Task> MessageReceived;
        event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;
        event Func<Task> Connected;
        event Func<Exception, Task> Disconnected;
        event Func<Task> Ready;
        event Func<SocketGroupUser, Task> RecipientRemoved;
        event Func<SocketChannel, Task> ChannelCreated;
        event Func<SocketChannel, Task> ChannelDestroyed;
        event Func<SocketChannel, SocketChannel, Task> ChannelUpdated;
        event Func<SocketGroupUser, Task> RecipientAdded;

        Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null);
        // Сводка:
        //     Downloads the users list for the provided guilds, if they don't have a complete
        //     list.
        Task DownloadUsersAsync(IEnumerable<IGuild> guilds);
        SocketChannel GetChannel(ulong id);
        SocketGuild GetGuild(ulong id);
        Task<RestInvite> GetInviteAsync(string inviteId);
        SocketUser GetUser(string username, string discriminator);
        SocketUser GetUser(ulong id);
        RestVoiceRegion GetVoiceRegion(string id);
        Task SetGameAsync(string name, string streamUrl = null, StreamType streamType = StreamType.NotStreaming);
        Task SetStatusAsync(UserStatus status);
    }
}