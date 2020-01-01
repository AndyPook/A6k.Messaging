using A6k.Messaging.Kafka;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public class KafkaOptionsValidation : IValidateOptions<KafkaOptions>
    {
        public ValidateOptionsResult Validate(string name, KafkaOptions options)
        {
            if (options.Configuration.Count == 0)
                return ValidateOptionsResult.Fail($"Kafka {name}: {nameof(options.Configuration)} MUST be specified");
            if (string.IsNullOrWhiteSpace(options.BootstrapServers))
                return ValidateOptionsResult.Fail($"Kafka {name}: {nameof(options.BootstrapServers)} MUST be specified");
            if (string.IsNullOrWhiteSpace(options.Topic))
                return ValidateOptionsResult.Fail($"Kafka {name}: {nameof(options.Topic)} MUST be specified");

            return ValidateOptionsResult.Success;
        }
    }
}
