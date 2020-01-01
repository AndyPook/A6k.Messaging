using System;
using System.Linq.Expressions;

using Confluent.Kafka;
using Microsoft.Extensions.Logging;

using Moq;

namespace A6k.Messaging.Tests
{
    public static class MockVerifyExtensions
    {
        public static void VerifyProduceAsync<TKey, TValue>(this Mock<IProducer<TKey, TValue>> producer, string topic, Expression<Func<Confluent.Kafka.Message<TKey, TValue>, bool>> predicate)
        {
            producer.VerifyProduceAsync(x => x == topic, predicate);
        }

        public static void VerifyProduceAsync<TKey, TValue>(this Mock<IProducer<TKey, TValue>> producer, Expression<Func<string, bool>> topicPredicate, Expression<Func<Confluent.Kafka.Message<TKey, TValue>, bool>> messagePredicate)
        {
            producer.Verify(x =>
                x.ProduceAsync(
                    It.Is(topicPredicate),
                    It.Is(messagePredicate)
                ),
                Times.Once
            );
        }

        public static void VerifyLogMessage<T>(this Mock<ILogger<T>> logger, LogLevel loglevel, Func<string, bool> messageTest)
        {
            logger.Verify(x =>
                x.Log(
                    loglevel,
                    It.IsAny<EventId>(),
                    It.Is<object>(o => messageTest(o.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}
