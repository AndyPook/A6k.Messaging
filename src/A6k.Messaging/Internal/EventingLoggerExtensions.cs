using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace A6k.Messaging.Internal
{
    /// <summary>
    /// Extension for "high performance logging"
    /// <see href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage"/>
    /// </summary>
    public static class EventingLoggerExtensions
    {
        private static readonly Action<ILogger, string, string, Exception> messagePumpStarted = LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(1, "MessagePumpStarted"),
            "MessagePump '{PumpName}' started: {Handler}"
        );

        private static readonly Action<ILogger, string, string, Exception> messagePumpStopped = LoggerMessage.Define<string ,string>(
            LogLevel.Information,
            new EventId(2, "MessagePumpStopped"),
            "MessagePump '{PumpName}' stopped: {Handler}"
        );

        private static readonly Action<ILogger, string, string, string[], Exception> messagePumpWaiting = LoggerMessage.Define<string, string, string[]>(
            LogLevel.Information,
            new EventId(3, "MessagePumpWaiting"),
            "MessagePump '{PumpName}' waiting: {Handler}; waiting for {Names}"
        );

        private static readonly Action<ILogger, string, string, TimeSpan, string[], Exception> messagePumpResumed = LoggerMessage.Define<string, string, TimeSpan, string[]>(
            LogLevel.Information,
            new EventId(4, "MessagePumpResumed"),
            "MessagePump '{PumpName}' resumed: {Handler}; duration {Duration}; after {Tokens}"
        );

        private static readonly Action<ILogger, string, Activity, Exception> messageHandlerError = LoggerMessage.Define<string, Activity>(
            LogLevel.Error,
            new EventId(5, "MessageHandler"),
            "Handler failed: {Handler}; {Activity}"
        );

        private static readonly Action<ILogger, string, Activity, Exception> consumerError = LoggerMessage.Define<string, Activity>(
            LogLevel.Error,
            new EventId(6, "Consumer"),
            "Consumer failed: {Handler}; {Activity}"
        );

        public static void MessagePumpStarted(this ILogger logger, string pumpName, string handlerName) => messagePumpStarted(logger, pumpName, handlerName, null);
        public static void MessagePumpStopped(this ILogger logger, string pumpName, string handlerName) => messagePumpStopped(logger, pumpName, handlerName, null);

        public static void MessagePumpWaiting(this ILogger logger, string pumpName, string handlerName, string[] tokenNames) => messagePumpWaiting(logger, pumpName, handlerName, tokenNames, null);
        public static void MessagePumpResumed(this ILogger logger, string pumpName, string handlerName, TimeSpan duration, string[] tokenNames) => messagePumpResumed(logger, pumpName, handlerName, duration, tokenNames, null);

        public static void MessageHandlerFailed(this ILogger logger, string handlerName, Activity activity, Exception ex) => messageHandlerError(logger, handlerName, activity, ex);
        public static void ConsumerFailed(this ILogger logger, string handlerName, Activity activity, Exception ex) => consumerError(logger, handlerName, activity, ex);
    }
}
