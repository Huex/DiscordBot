using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class DiscordClient : DiscordSocketClient, IPublicDiscordSocket
    {
        public DiscordClient() : base() { }
        public DiscordClient(DiscordSocketConfig config) : base(config) { }
    }
}
