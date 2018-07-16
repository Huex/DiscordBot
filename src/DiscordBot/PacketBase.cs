using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;

namespace DiscordBot.Core
{
    public abstract class PacketBase : LogEntity
    {
        public IPacketDiscordClient Discord { get; private set; }
        public ICommandConfigsModOnlyProvider CommandConfigsProvider { get; private set; }
        public BotConfig BotConfig { get; private set; }
        public bool DiscordIsInitialized { get; private set; } = false;

        public event Action Initialized;

        public ServiceCollection Services { get; set; } = new ServiceCollection();
        public Collection<Type> GuildModules { get; set; } = new Collection<Type>();
        public Collection<Type> DMModules { get; set; } = new Collection<Type>();

        internal void InitPacket(BotConfig config, DiscordClient discord, ICommandConfigsModOnlyProvider commanderConfigsProvider)
        {
            Discord = discord;
            CommandConfigsProvider = commanderConfigsProvider;
            BotConfig = config;
            DiscordIsInitialized = true;
            Initialized?.Invoke();
        }
    }
}
