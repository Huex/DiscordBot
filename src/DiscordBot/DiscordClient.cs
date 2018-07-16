using Discord.WebSocket;

namespace DiscordBot
{
    public class DiscordClient : DiscordSocketClient, IPacketDiscordClient
    {
        public DiscordClient() : base() { }
        public DiscordClient(DiscordSocketConfig config) : base(config) { }
    }
}
