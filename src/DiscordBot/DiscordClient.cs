using System;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot
{
    public class DiscordClient : DiscordSocketClient, IPacketDiscordClient, ILogEntity
    {
        public DiscordClient() : base() { }
        public DiscordClient(DiscordSocketConfig config) : base(config) { }

        public new event Func<LogMessage, Task> Log
        {
            add
            {
                base.Log += async (arg) =>
                {
                    await value(new LogMessage(arg));
                };
            }
            remove
            {
                base.Log -= async (arg) =>
                {
                    await value(new LogMessage(arg));
                };
            }
        }
    }
}
