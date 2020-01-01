using System.Diagnostics;
using App.Metrics;
using App.Metrics.Meter;
using App.Metrics.Timer;

namespace A6k.Messaging
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

        //public static void RecordEndpointsRequestTime(this IMetrics metrics, string handlerName, long elapsedns)
        //{
        //    metrics
        //        .HandlerRequestTimer(handlerName)
        //        .Record(
        //            elapsedns,
        //            TimeUnit.Nanoseconds
        //        );
        //}

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

        public static class ApdexScores
        {
            //public static readonly string ApdexMetricName = "Apdex";

            //public static readonly Func<double, ApdexOptions> Apdex = apdexTSeconds => new ApdexOptions
            //{
            //    Context = ContextName,
            //    Name = ApdexMetricName,
            //    ApdexTSeconds = apdexTSeconds
            //};
        }

        public static class Counters
        {
            //public static readonly CounterOptions ActiveRequestCount = new CounterOptions
            //{
            //    Context = ContextName,
            //    Name = "Active",
            //    MeasurementUnit = Unit.Custom("Active Requests")
            //};

            //public static readonly CounterOptions TotalErrorRequestCount = new CounterOptions
            //{
            //    Context = ContextName,
            //    Name = "Errors",
            //    ResetOnReporting = true,
            //    MeasurementUnit = Unit.Errors
            //};

            //public static readonly CounterOptions UnhandledExceptionCount = new CounterOptions
            //{
            //    Context = ContextName,
            //    Name = "Exceptions",
            //    MeasurementUnit = Unit.Errors,
            //    ReportItemPercentages = false,
            //    ReportSetItems = false,
            //    ResetOnReporting = true
            //};
        }

        public static class Gauges
        {
            //public static readonly GaugeOptions EndpointOneMinuteErrorPercentageRate = new GaugeOptions
            //{
            //    Context = ContextName,
            //    Name = "One Minute Error Percentage Rate Per Endpoint",
            //    MeasurementUnit = Unit.Requests
            //};

            //public static readonly GaugeOptions OneMinErrorPercentageRate = new GaugeOptions
            //{
            //    Context = ContextName,
            //    Name = "One Minute Error Percentage Rate",
            //    MeasurementUnit = Unit.Requests
            //};
        }

        public static class Histograms
        {
            //public static readonly HistogramOptions PostRequestSizeHistogram = new HistogramOptions
            //{
            //    Context = ContextName,
            //    Name = "POST Size",
            //    MeasurementUnit = Unit.Bytes
            //};

            //public static readonly HistogramOptions PutRequestSizeHistogram = new HistogramOptions
            //{
            //    Context = ContextName,
            //    Name = "PUT Size",
            //    MeasurementUnit = Unit.Bytes
            //};
        }

        public static class Meters
        {
            //public static readonly MeterOptions EndpointErrorRequestPerStatusCodeRate = new MeterOptions
            //{
            //    Context = ContextName,
            //    Name = "Error Rate Per Endpoint And Status Code",
            //    MeasurementUnit = Unit.Requests
            //};

            //public static readonly MeterOptions HandlerErrorRate = new MeterOptions
            //{
            //    Context = ContextName,
            //    Name = "Error Rate Per Handler",
            //    MeasurementUnit = Unit.Calls
            //};

            //public static readonly MeterOptions ErrorRequestRate = new MeterOptions
            //{
            //    Context = ContextName,
            //    Name = "Error Rate",
            //    MeasurementUnit = Unit.Requests
            //};
        }
    }
}
