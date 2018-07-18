using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    public interface ILogEntity
    {
        event Func<LogMessage, Task> Log;
    }
}
