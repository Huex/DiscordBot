using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public abstract class PacketBase : ILogEntity
    {
        public IPacketDiscordClient Discord { get; private set; }
        public ICommandConfigsModOnlyProvider CommandConfigsProvider { get; private set; }
        public BotConfig BotConfig { get; private set; }
        public bool IsInitialized { get; private set; } = false;

        public event Action Initialized;
        public event Func<LogMessage, Task> Log;

        public ServiceCollection Services { get; set; } = new ServiceCollection();
        public Collection<Type> GuildModules { get; set; } = new Collection<Type>();
        public Collection<Type> DMModules { get; set; } = new Collection<Type>();

        internal void InitPacket(BotConfig config, DiscordClient discord, ICommandConfigsModOnlyProvider commanderConfigsProvider)
        {
            Discord = discord;
            CommandConfigsProvider = commanderConfigsProvider;
            BotConfig = config;
            IsInitialized = true;
            Initialized?.Invoke();
        }

        #region Log
        protected void RaiseLog(LogSeverity severity, string message, Exception exception = null)
        {
            RaiseLog(new LogMessage(severity, GetType().Name, message, exception));
        }

        protected void RaiseLog(LogMessage message)
        {
            Log?.Invoke(message);
        }

        protected async Task RaiseLogAsync(LogMessage message)
        {
            await Log?.Invoke(message);
        }

        protected async Task RaiseLogAsync(Discord.LogMessage message)
        {
            await Log?.Invoke(new LogMessage(message));
        }
        #endregion
    }
}
