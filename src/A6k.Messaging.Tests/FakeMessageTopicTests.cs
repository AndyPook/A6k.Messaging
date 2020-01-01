using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace A6k.Messaging.Tests
{
    public class FakeMessageTopicTests
    {
        [Fact]
        public async Task EnsureMessageAvailable()
        {
            // Arrange
            var topic = new FakeMessageTopic<string, string>();

            // Act
            await topic.ProduceAsync("123", "BLAH");
            var msg = await topic.ConsumeAsync();

            // Assert
            Assert.NotNull(msg);
            Assert.Equal("123", msg.Key);
            Assert.Equal("BLAH", msg.Value);
        }

        [Fact]
        public void RegisterProvider()
        {
            var sp = new ServiceCollection()
                .AddFakeMessageProviders()
                .BuildServiceProvider();

            var producer = sp.GetMessageProducer<string, string>(nameof(RegisterProvider));

            Assert.NotNull(producer);
            Assert.IsType<FakeMessageTopic<string, string>>(producer);
            Assert.Equal(nameof(RegisterProvider), ((FakeMessageTopic<string, string>)producer).TopicName);
        }
    }
}
