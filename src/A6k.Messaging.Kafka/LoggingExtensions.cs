using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace A6k.Messaging.Kafka
{
    public static class LoggingExtensions
    {
        public static LogLevel GetLogLevel(this SyslogLevel level)
        {
            switch (level)
            {
                case SyslogLevel.Emergency:
                case SyslogLevel.Alert:
                case SyslogLevel.Critical: return LogLevel.Critical;
                case SyslogLevel.Error: return LogLevel.Error;
                case SyslogLevel.Warning: return LogLevel.Warning;
                case SyslogLevel.Notice:
                case SyslogLevel.Info: return LogLevel.Information;
                case SyslogLevel.Debug: return LogLevel.Debug;
            }
            return LogLevel.None;
        }
    }
}
