using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

using Confluent.Kafka;
using Moq;
using Xunit;

namespace A6k.Messaging.Kafka.Tests
{
    public class KafkaProducerTests
    {
        [Fact]
        public async Task ProduceAMessage()
        {
            var kafka = CreateProducerMock<string, string>();
            var producer = CreateProducer(kafka);

            var message = new Message<string, string>
            {
                Key = "123",
                Value = "fred"
            };

            await producer.ProduceAsync(message);

            kafka.VerifyProduceAsync("topic", x => x.Key == "123" && x.Value == "fred");
        }

        [Fact]
        public async Task ProduceAMessageWithHeaders()
        {
            var kafka = CreateProducerMock<string, string>();
            var producer = CreateProducer(kafka);

            var message = new Message<string, string>
            {
                ActivityId = "activity",
                Key = "123",
                Value = "fred"
            };
            message.AddHeader("h1", "v1");

            await producer.ProduceAsync(message);

            // ActivityId is added as a header
            kafka.VerifyProduceAsync("topic", x => x.Key == "123" && x.Value == "fred" && x.Headers.Count == 2);
        }

        private Mock<IProducer<TKey, TValue>> CreateProducerMock<TKey, TValue>()
        {
            var kafka = new Mock<IProducer<TKey, TValue>>(MockBehavior.Strict);
            kafka
                .Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Confluent.Kafka.Message<TKey, TValue>>()))
                .Returns<string, Confluent.Kafka.Message<TKey, TValue>>((t, m) => Task.FromResult(new DeliveryResult<TKey, TValue> { Message = m }));
            return kafka;
        }

        private static KafkaProducer<string, string> CreateProducer(Mock<IProducer<string, string>> kafka)
        {
            var producer = new KafkaProducer<string, string>(
                new KafkaOptions { Topic = "topic" },
                NullLogger<KafkaProducer<string, string>>.Instance
            );
            producer.Configure(f => f.SetProducerFactory(kafka.Object));
            return producer;
        }
    }
}
