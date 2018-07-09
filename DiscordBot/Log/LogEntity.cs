using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    public abstract class LogEntity
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

        protected async Task RaiseLogAsync(LogMessage message)
        {
            await Log?.Invoke(message);
        }

        protected async Task RaiseLogAsync(Discord.LogMessage message)
        {
            await Log?.Invoke(new LogMessage(message));
        }
    }
}
