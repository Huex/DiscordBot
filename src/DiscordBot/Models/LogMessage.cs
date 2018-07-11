using System;

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

        public LogMessage(LogSeverity severity, string source, string message, Exception exception)
        {
            Severity = severity;
            Source = source;
            Message = message;
            Exception = exception;
        }

        public LogMessage(LogSeverity severity, string source, string message) : this(severity, source, message, null) { }

        public LogSeverity Severity { get; }
        public string Source { get; }
        public string Message { get; }
        public Exception Exception { get; }
    }
}
