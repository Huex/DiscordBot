using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class LogRaiser
    {
        private readonly string _source;
        private readonly Func<LogMessage, Task> _logMethod;

        public LogRaiser(string source, Func<LogMessage, Task> logMethod)
        {
            _source = source;
            _logMethod = logMethod;
        }

        public void Raise(LogSeverity severity, string message, Exception exception)
        {
            Raise(new LogMessage(severity, _source, message, exception));
        }

        public async Task RaiseAsync(LogSeverity severity, string message)
        {
            await RaiseAsync(new LogMessage(severity, _source, message, null));
        }

        public async Task RaiseAsync(LogSeverity severity, string message, Exception exception)
        {
            await RaiseAsync(new LogMessage(severity, _source, message, exception));
        }

        public void Raise(LogSeverity severity, string message)
        {
            Raise(new LogMessage(severity, _source, message, null));
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
