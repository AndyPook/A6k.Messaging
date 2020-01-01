using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace A6k.Messaging.Kafka
{
    public class KafkaHealthCheck : IHealthCheck
    {
        private readonly IOptionsMonitor<KafkaOptions> monitor;

        public KafkaHealthCheck(IOptionsMonitor<KafkaOptions> monitor)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            KafkaOptions options = null;
            try
            {
                options = monitor.Get(context.Registration.Name);

                using (var admin = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = options.BootstrapServers }).Build())
                {
                    var result = await admin.DescribeConfigsAsync(
                        new[] { new ConfigResource { Name = options.Topic, Type = ResourceType.Topic } }, 
                        new DescribeConfigsOptions { RequestTimeout = TimeSpan.FromSeconds(1) }
                    );
                    if (result.Count != 1)
                        return new HealthCheckResult(context.Registration.FailureStatus, $"Topic not found: {options.Topic}", data: GetData());

                    return HealthCheckResult.Healthy(data: GetData());
                }
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, ex.Message, ex, GetData());
            }

            IReadOnlyDictionary<string, object> GetData()
            {
                var data = options?.Configuration.ToDictionary(x => x.Key, x => (object)x.Value) ?? new Dictionary<string, object>();
                data["Topic"] = options?.Topic;
                return data;
            }
        }
    }
}
