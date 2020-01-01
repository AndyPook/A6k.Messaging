using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace A6k.Messaging.Tests
{
    public partial class MessagePumpTests
    {
        [Fact]
        public async Task EnsureMessageIsHandled()
        {
            // Arrange
            // fakes
            var fakeBus = new FakeMessageTopic<string, string>();
            var fakeHandler = new TestingMessageHandler<string, string>();

            // sut, the pump
            var pump = new MessagePump<string, string>(() => fakeBus, fakeHandler);
            await pump.StartAsync(default);

            // Act
            // add a message
            await fakeBus.ProduceAsync("123", "blah blah");

            // indicate we're done and wait for completion
            fakeBus.CompleteAdding();
            await fakeBus.CompletedAndEmpty();
            await pump.StopAsync(default);

            // Assert
            Assert.Single(fakeHandler.Messages);

            var msg = fakeHandler.Messages.First();
            Assert.Equal("123", msg.Key);
            Assert.Equal("blah blah", msg.Value);
        }
    }
}
