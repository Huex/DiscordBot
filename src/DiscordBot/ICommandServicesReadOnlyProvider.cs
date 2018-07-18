using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public interface ICommandServicesReadOnlyProvider
    {
        IReadOnlyDictionary<ulong, CommandService> CommandServices { get; }
    }
}
