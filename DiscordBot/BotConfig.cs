using Discord.WebSocket;

namespace DiscordBot
{
    public struct BotConfig
    {
        public string DefaultPrefix { get; set; } 
        public LogSeverity LogLevel { get; set; }
        public Discord.LogSeverity DiscordSocketLogLevel { get; set; }
        public string Token { get; set; }
        public int ConnectionTimeout { get; set; }
        public int MessageCacheSize { get; set; }
        public int LargeThreshold { get; set; }
        public bool AlwaysDownloadUsers { get; set; }
        public int? HandlerTimeout { get; set; }

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
    }
}
