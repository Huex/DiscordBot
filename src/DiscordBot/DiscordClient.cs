using Discord.WebSocket;

namespace DiscordBot
{
    public class DiscordClient : DiscordSocketClient, IPublicDiscordClient
    {
        public DiscordClient() : base() { }
        public DiscordClient(DiscordSocketConfig config) : base(config) { }
    }
}
