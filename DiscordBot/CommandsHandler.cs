using Discord.Commands;
using DiscordBot.Services;
using System;
using System.Collections.ObjectModel;

namespace DiscordBot.Core
{
    public class CommandsHandler
    {
        public Collection<BotServiceBase> Services { get; set; } = new Collection<BotServiceBase>();
        public Collection<Type> Modules { get; set; } = new Collection<Type>();
    }
}