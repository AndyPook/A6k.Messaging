using System;
using Confluent.Kafka;
using A6k.Messaging.Features;
using A6k.Messaging.Kafka.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaFeatureCollectionExtensions
    {
        public static IFeatureSet SetProducerFactory<TKey, TValue>(this IFeatureSet f, IProducer<TKey, TValue> producer)
            => f.SetProducerFactory<TKey, TValue>(_ => producer);

        public static IFeatureSet SetProducerFactory<TKey, TValue>(this IFeatureSet f, Func<ProducerBuilder<TKey, TValue>, IProducer<TKey, TValue>> factory)
        {
            if (factory == null)
                return f;

            f.Set(ProducerFactoryFeature.Create(factory));

            return f;
        }

        public static IFeatureSet SetConsumerFactory<TKey, TValue>(this IFeatureSet f, IConsumer<TKey, TValue> consumer)
            => f.SetConsumerFactory<TKey, TValue>(_ => consumer);

        public static IFeatureSet SetConsumerFactory<TKey, TValue>(this IFeatureSet f, Func<ConsumerBuilder<TKey, TValue>, IConsumer<TKey, TValue>> factory)
        {
            if (factory == null)
                return f;

            f.Set(ConsumerFactoryFeature.Create(factory));

            return f;
        }

        /// <summary>
        /// Start at the beginning of the Topic
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFeatureSet StartAtBeginning(this IFeatureSet f)
        {
            f.Set(StartAtPositionFeature.Beginning);

            return f;
        }

        /// <summary>
        /// Start at the end of the Topic
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFeatureSet StartAtEnd(this IFeatureSet f)
        {
            f.Set(StartAtPositionFeature.End);

            return f;
        }

        /// <summary>
        /// Set the GroupId to something unique
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFeatureSet UniqueGroupId(this IFeatureSet f)
        {
            f.Set(new UniqueGroupIdFeature());

            return f;
        }
    }
}
