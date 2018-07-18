using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    interface ICommandServiceReadOnlyProvider
    {
        IReadOnlyDictionary<ulong, CommandService> CommandServices { get; }
    }
}
