using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class Packet
    {
        public readonly Collection<BotServiceBase> Services = new Collection<BotServiceBase>();
        public readonly Collection<ModuleBase> Modules = new Collection<ModuleBase>();
        public readonly DiscordEventsHandlers EventHandlers = new DiscordEventsHandlers();
    }
}
