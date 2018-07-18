using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    interface IPacketCommandService
    {
        IEnumerable<CommandInfo> Commands { get; }
        IEnumerable<ModuleInfo> Modules { get; }
        ILookup<Type, TypeReader> TypeReaders { get; }
        event Func<LogMessage, Task> Log;
    }
}
