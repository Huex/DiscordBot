using System;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public abstract class BotServiceBase
    {
        public event Func<LogMessage, Task> Log;

        protected void RaiseLog(LogSeverity severity, string message, Exception exception = null)
        {
            RaiseLog(new LogMessage(severity, GetType().Name, message, exception));
        }

        protected void RaiseLog(LogMessage message)
        {
            Log?.Invoke(message);
        }
    }
}