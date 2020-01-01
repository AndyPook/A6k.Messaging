using App.Metrics;
using App.Metrics.Timer;

namespace A6k.Messaging.Internal
{
    public static class MetricsExtensions
    {
        public static ITimer HandlerRequestTimer(this IMetrics metrics, string handlerName)
        {
            var tags = new MetricTags(
                new[] { "Handler", "success" },
                new[] { handlerName, "true" }
            );
            return metrics.Provider.Timer.Instance(Timers.HandlerExecutionDuration, tags);
        }
        public static ITimer HandlerFailRequestTimer(this IMetrics metrics, string handlerName)
        {
            var tags = new MetricTags(
                new[] { "Handler", "success" },
                new[] { handlerName, "false" }
            );
            return metrics.Provider.Timer.Instance(Timers.HandlerExecutionDuration, tags);
        }

        public static string ContextName = "Application";

        public static class Timers
        {
            public static readonly TimerOptions HandlerExecutionDuration = new TimerOptions
            {
                Context = ContextName,
                Name = "Messages",
                MeasurementUnit = Unit.Calls
            };
        }
    }
}
