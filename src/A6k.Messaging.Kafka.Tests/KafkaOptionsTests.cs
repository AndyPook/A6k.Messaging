using Xunit;

namespace A6k.Messaging.Kafka.Tests
{
    public class KafkaOptionsTests
    {
        [Fact]
        public void BootstrapServers_ViaProp()
        {
            var options = new KafkaOptions();

            options.BootstrapServers = "bs";

            Assert.Equal("bs", options.Configuration["bootstrap.servers"]);
        }

        [Fact]
        public void BootstrapServers_Configuration()
        {
            var options = new KafkaOptions();

            options.Configuration["bootstrap.servers"] = "bs";
            

            Assert.Equal("bs", options.BootstrapServers);
        }

        [Fact]
        public void GroupId_ViaProp()
        {
            var options = new KafkaOptions();

            options.GroupId = "grp";

            Assert.Equal("grp", options.Configuration["group.id"]);
        }

        [Fact]
        public void GroupId_Configuration()
        {
            var options = new KafkaOptions();

            options.Configuration["group.id"] = "grp";


            Assert.Equal("grp", options.GroupId);
        }
    }
}
