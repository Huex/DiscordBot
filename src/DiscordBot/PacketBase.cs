using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;

namespace DiscordBot.Core
{
    public abstract class PacketBase : LogEntity
    {
        public DiscordSocketClient Discord { get; private set; }
        public DiscordEventsHandlers EventsHandlers { get; } = new DiscordEventsHandlers();

        public ServiceCollection Services { get; set; } = new ServiceCollection();
        public Collection<Type> GuildModules { get; set; } = new Collection<Type>();
        public Collection<Type> DMModules { get; set; } = new Collection<Type>();
        public object DMCommands { get; internal set; }

        internal void SetDiscordSocket(DiscordSocketClient discord)
        {
            Discord = discord;
        }
    }
}
