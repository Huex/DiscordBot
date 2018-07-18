using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class LogRaiser
    {
        private readonly Func<LogMessage, Task> _logMethod;

        public LogRaiser(Func<LogMessage, Task> logMethod)
        {
            _logMethod = logMethod;
        }

        public void Raise(LogSeverity severity, string message, Exception exception)
        {
            Raise(new LogMessage(severity, GetType().Name, message, exception));
        }

        public async Task RaiseAsync(LogSeverity severity, string message)
        {
            await RaiseAsync(new LogMessage(severity, GetType().Name, message, null));
        }

        public async Task RaiseAsync(LogSeverity severity, string message, Exception exception)
        {
            await RaiseAsync(new LogMessage(severity, GetType().Name, message, exception));
        }

        public void Raise(LogSeverity severity, string message)
        {
            Raise(new LogMessage(severity, GetType().Name, message, null));
        }

        public void Raise(LogMessage message)
        {
            _logMethod(message);
        }

        public async Task RaiseAsync(LogMessage message)
        {
            await _logMethod(message);
        }

        public async Task RaiseAsync(Discord.LogMessage message)
        {
            await _logMethod(new LogMessage(message));
        }
    }
}
