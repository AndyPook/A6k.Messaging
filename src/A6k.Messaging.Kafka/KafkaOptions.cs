using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace A6k.Messaging.Kafka
{
    /// <summary>
    /// Options required to configure a Producer/Consumer
    /// </summary>
    public partial class KafkaOptions
    {
        public const string ProviderName = "kafka";

        /// <summary>
        /// Keys used in the Kafka configuration settings
        /// </summary>
        public static class ConfigKeys
        {
            public const string GroupId = "group.id";
            public const string BootstrapServers = "bootstrap.servers";
        }

        private readonly ClientConfig config;

        public KafkaOptions()
        {
            config = new ClientConfig(Configuration);
        }


        /// <summary>
        /// The raw Kafka configuration. A set of key/value pairs
        /// <para>see https://github.com/edenhill/librdkafka/blob/master/CONFIGURATION.md </para>
        /// </summary>
        public IDictionary<string, string> Configuration { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Initial list of brokers as a CSV list of broker host or host:port. The application may also use `rd_kafka_brokers_add()` to add brokers during runtime.
        /// <para>Shortcut into the <see cref="Configuration"/> property</para>
        /// </summary>
        public string BootstrapServers { get { return config.BootstrapServers; } set { config.BootstrapServers = value; } }

        /// <summary>
        /// Client group id string. All clients sharing the same group.id belong to the same group.
        /// (Only valid for Consumers)
        /// (Will default to the "ApplicationName" of the hosting process)
        /// <para>Shortcut into the <see cref="Configuration"/> property</para>
        /// </summary>
        public string GroupId { get { return config.Get(ConfigKeys.GroupId); } set { config.Set(ConfigKeys.GroupId, value); } }

        /// <summary>
        /// The name of the topic to produce/consume
        /// </summary>
        public string Topic { get; set; }
    }
}
