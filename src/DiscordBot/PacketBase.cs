using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public abstract class PacketBase : LogEntity
    {
        public IPublicDiscordSocket Discord { get; private set; }
        private Action<ulong, CommandConfig> _updateCommandConfig;
        private Func<ulong, CommandConfig> _getCommandConfig;

        public event Action DiscordInitialized;
        public bool DiscordIsInitialized { get; private set; } = false;

        //public SocketChannel GetChannel(ulong id) => _discord.GetChannel(id);
        //public SocketGuild GetGuild(ulong id) => _discord.GetGuild(id);
        //public Task<RestInvite> GetInviteAsync(string inviteId) => _discord.GetInviteAsync(inviteId);
        //public SocketUser GetUser(string username, string discriminator) => _discord.GetUser(username, discriminator);
        //public SocketUser GetUser(ulong id) => _discord.GetUser(id);
        //public RestVoiceRegion GetVoiceRegion(string id) => _discord.GetVoiceRegion(id);
        //public Task SetGameAsync(string name, string streamUrl = null, StreamType streamType = StreamType.NotStreaming) => _discord.SetGameAsync(name, streamUrl, streamType);
        //public Task SetStatusAsync(UserStatus status) => _discord.SetStatusAsync(status);
        //public Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null) => _discord.CreateGuildAsync(name, region, jpegIcon);

        //public IReadOnlyCollection<RestVoiceRegion> VoiceRegions { get { return _discord.VoiceRegions; } }
        //public IReadOnlyCollection<SocketGroupChannel> GroupChannels { get { return _discord.GroupChannels; } }
        //public IReadOnlyCollection<SocketDMChannel> DMChannels { get { return _discord.DMChannels; } }
        //public IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels { get { return _discord.PrivateChannels; } }
        //public IReadOnlyCollection<SocketGuild> Guilds { get { return _discord.Guilds; } }
        //public SocketSelfUser CurrentUser { get { return _discord.CurrentUser; } }
        //public ConnectionState ConnectionState { get { return _discord.ConnectionState; } }

        //public Func<SocketChannel, Task> ChannelCreated { get; set; } = new Func<SocketChannel, Task>((a) => Task.CompletedTask);
        //public Func<SocketChannel, Task> ChannelDestroyed { get; set; } = new Func<SocketChannel, Task>((a) => Task.CompletedTask);
        //public Func<SocketChannel, SocketChannel, Task> ChannelUpdated { get; set; } = new Func<SocketChannel, SocketChannel, Task>((a, b) => Task.CompletedTask);
        //public Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated { get; set; } = new Func<SocketSelfUser, SocketSelfUser, Task>((a, b) => Task.CompletedTask);
        //public Func<Task> Connected { get; set; } = new Func<Task>(() => Task.CompletedTask);
        //public Func<Exception, Task> Disconnected { get; set; } = new Func<Exception, Task>((a) => Task.CompletedTask);
        //public Func<SocketGuild, Task> GuildAvailable { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        //public Func<SocketGuild, Task> GuildMembersDownloaded { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        //public Func<SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated { get; set; } = new Func<SocketGuildUser, SocketGuildUser, Task>((a, b) => Task.CompletedTask);
        //public Func<SocketGuild, Task> GuildUnavailable { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        //public Func<SocketGuild, SocketGuild, Task> GuildUpdated { get; set; } = new Func<SocketGuild, SocketGuild, Task>((a, b) => Task.CompletedTask);
        //public Func<SocketGuild, Task> JoinedGuild { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        //public Func<SocketGuild, Task> LeftGuild { get; set; } = new Func<SocketGuild, Task>((a) => Task.CompletedTask);
        //public Func<Task> LoggedIn { get; set; } = new Func<Task>(() => Task.CompletedTask);
        //public Func<Task> LoggedOut { get; set; } = new Func<Task>(() => Task.CompletedTask);
        //public Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted { get; set; } = new Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task>((a, b) => Task.CompletedTask);
        //public Func<SocketMessage, Task> MessageReceived { get; set; } = new Func<SocketMessage, Task>((a) => Task.CompletedTask);
        //public Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated { get; set; } = new Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task>((a, b, c) => Task.CompletedTask);
        //public Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded { get; set; } = new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>((a, b, c) => Task.CompletedTask);
        //public Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved { get; set; } = new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>((a, b, c) => Task.CompletedTask);
        //public Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared { get; set; } = new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task>((a, b) => Task.CompletedTask);
        //public Func<Task> Ready { get; set; } = new Func<Task>(() => Task.CompletedTask);
        //public Func<SocketGroupUser, Task> RecipientAdded { get; set; } = new Func<SocketGroupUser, Task>((a) => Task.CompletedTask);
        //public Func<SocketGroupUser, Task> RecipientRemoved { get; set; } = new Func<SocketGroupUser, Task>((a) => Task.CompletedTask);
        //public Func<SocketRole, Task> RoleCreated { get; set; } = new Func<SocketRole, Task>((a) => Task.CompletedTask);
        //public Func<SocketRole, Task> RoleDeleted { get; set; } = new Func<SocketRole, Task>((a) => Task.CompletedTask);
        //public Func<SocketRole, SocketRole, Task> RoleUpdated { get; set; } = new Func<SocketRole, SocketRole, Task>((a, b) => Task.CompletedTask);
        //public Func<SocketUser, SocketGuild, Task> UserBanned { get; set; } = new Func<SocketUser, SocketGuild, Task>((a, b) => Task.CompletedTask);
        //public Func<SocketUser, ISocketMessageChannel, Task> UserIsTyping { get; set; } = new Func<SocketUser, ISocketMessageChannel, Task>((a, b) => Task.CompletedTask);
        //public Func<SocketGuildUser, Task> UserJoined { get; set; } = new Func<SocketGuildUser, Task>((a) => Task.CompletedTask);
        //public Func<SocketGuildUser, Task> UserLeft { get; set; } = new Func<SocketGuildUser, Task>((a) => Task.CompletedTask);
        //public Func<SocketUser, SocketGuild, Task> UserUnbanned { get; set; } = new Func<SocketUser, SocketGuild, Task>((a, b) => Task.CompletedTask);
        //public Func<SocketUser, SocketUser, Task> UserUpdated { get; set; } = new Func<SocketUser, SocketUser, Task>((a, b) => Task.CompletedTask);
        //public Func<SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated { get; set; } = new Func<SocketUser, SocketVoiceState, SocketVoiceState, Task>((a, b, c) => Task.CompletedTask);

        public ServiceCollection Services { get; set; } = new ServiceCollection();
        public Collection<Type> GuildModules { get; set; } = new Collection<Type>();
        public Collection<Type> DMModules { get; set; } = new Collection<Type>();

        private void SetDiscordSocket(IPublicDiscordSocket discord)
        {
            Discord = discord;
        }

        private void SetCommandConfigMethods(Action<ulong, CommandConfig> updateCommandConfig, Func<ulong, CommandConfig> getCommandConfig, Func<ulong, bool> commandConfigExsist)
        {
            _updateCommandConfig = updateCommandConfig;
            _getCommandConfig = getCommandConfig;
        }

        internal void InitPacket(DiscordClient discord, Action<ulong, CommandConfig> updateCommandConfig, Func<ulong, 
            CommandConfig> getCommandConfig, Func<ulong, bool> commandConfigExsist)
        {
            SetDiscordSocket(discord);
            SetCommandConfigMethods(updateCommandConfig, getCommandConfig, commandConfigExsist);
            DiscordIsInitialized = true;
            DiscordInitialized?.Invoke();
        }
    }
}
