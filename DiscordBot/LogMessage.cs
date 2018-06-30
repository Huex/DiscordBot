using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public struct LogMessage
    {
        public LogMessage(Discord.LogMessage logMessage)
        {
            Severity = (LogSeverity)logMessage.Severity;
            Source = logMessage.Source;
            Message = logMessage.Message;
            Exception = logMessage.Exception;
        }

        public LogMessage(LogSeverity severity, string source, string message, Exception exception = null)
        {
            Severity = severity;
            Source = source;
            Message = message;
            Exception = exception;
        }

        public LogSeverity Severity { get; }
        public string Source { get; }
        public string Message { get; }
        public Exception Exception { get; }
    }
}
