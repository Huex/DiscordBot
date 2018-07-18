using System;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public abstract class ServiceBase : ILogEntity
    {
        public event Func<LogMessage, Task> Log;
        protected LogRaiser Logger;

        public ServiceBase()
        {
            Logger = new LogRaiser(GetType().Name, async (msg) => await Log?.Invoke(msg));
        }
    }
}