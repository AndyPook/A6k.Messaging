using Confluent.Kafka;
using Xunit;

namespace A6k.Messaging.Kafka.Tests
{
    public class ClientConfigTests
    {
        [Fact]
        public void BootstrapServers_ViaProp()
        {
            var options = new KafkaOptions();
            options.BootstrapServers = "bs";

            var config = new ConsumerConfig(options.Configuration);

            Assert.Equal("bs", config.BootstrapServers);
        }

        [Fact]
        public void BootstrapServers_Configuration()
        {
            var options = new KafkaOptions();
            options.Configuration["bootstrap.servers"] = "bs";

            var config = new ConsumerConfig(options.Configuration);

            Assert.Equal("bs", config.BootstrapServers);
        }

        [Fact]
        public void GroupId_ViaProp()
        {
            var options = new KafkaOptions();
            options.GroupId = "grp";

            var config = new ConsumerConfig(options.Configuration);

            Assert.Equal("grp", config.GroupId);
        }

        [Fact]
        public void GroupId_Configuration()
        {
            var options = new KafkaOptions();
            options.Configuration["group.id"] = "grp";

            var config = new ConsumerConfig(options.Configuration);

            Assert.Equal("grp", config.GroupId);
        }

        [Fact]
        public void MaxPoll_Configuration()
        {
            var options = new KafkaOptions();
            options.Configuration["max.poll.interval.ms"] = "10000";

            var config = new ConsumerConfig(options.Configuration);

            Assert.Equal(10000, config.MaxPollIntervalMs);
        }
    }
}
