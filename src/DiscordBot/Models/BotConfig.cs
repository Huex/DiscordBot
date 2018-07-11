using Discord.WebSocket;
using System;

namespace DiscordBot
{
    public struct BotConfig : IEquatable<BotConfig>
    {
        public LogSeverity LogLevel { get; set; }
        public Discord.LogSeverity DiscordSocketLogLevel { get; set; }
        public string Token { get; set; }
        public int ConnectionTimeout { get; set; }
        public int MessageCacheSize { get; set; }
        public int LargeThreshold { get; set; }
        public bool AlwaysDownloadUsers { get; set; }
        public int? HandlerTimeout { get; set; }

        public CommandConfigBuilder DefaultUserCommandConfig { get; set; }
        public CommandConfigBuilder DefaultGuildCommandConfig { get; set; }

        public DiscordSocketConfig DiscordSocket
        {
            get
            {
                return new DiscordSocketConfig
                {
                    ConnectionTimeout = ConnectionTimeout,
                    MessageCacheSize = MessageCacheSize,
                    LargeThreshold = LargeThreshold,
                    AlwaysDownloadUsers = AlwaysDownloadUsers,
                    HandlerTimeout = HandlerTimeout,
                    LogLevel = DiscordSocketLogLevel
                };
            }
        }

        public bool Equals(BotConfig other)
        {
            return (LogLevel == other.LogLevel &&
                DiscordSocketLogLevel == other.DiscordSocketLogLevel &&
                Token == other.Token &&
                ConnectionTimeout == other.ConnectionTimeout &&
                MessageCacheSize == other.MessageCacheSize &&
                LargeThreshold == other.LargeThreshold &&
                AlwaysDownloadUsers == other.AlwaysDownloadUsers &&
                HandlerTimeout == other.HandlerTimeout);
        }
    }
}
