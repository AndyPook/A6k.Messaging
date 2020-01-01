using System;
using Confluent.Kafka;
using A6k.Messaging.Features;
using A6k.Messaging.Kafka.Features;
using Newtonsoft.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaFeatureCollectionExtensions
    {
        public static IFeatureCollection SetJsonSerialization(this IFeatureCollection f, JsonSerializerSettings valueSettings, JsonSerializerSettings keySettings = null)
        {
            if (valueSettings == null && keySettings == null)
                return f;

            f.Set(new KafkaSerializationFeature(keySettings, valueSettings));

            return f;
        }

        public static IFeatureCollection SetProducerFactory<TKey, TValue>(this IFeatureCollection f, IProducer<TKey, TValue> producer)
            => f.SetProducerFactory<TKey, TValue>(_ => producer);

        public static IFeatureCollection SetProducerFactory<TKey, TValue>(this IFeatureCollection f, Func<ProducerBuilder<TKey, TValue>, IProducer<TKey, TValue>> factory)
        {
            if (factory == null)
                return f;

            f.Set(ProducerFactoryFeature.Create(factory));

            return f;
        }

        public static IFeatureCollection SetConsumerFactory<TKey, TValue>(this IFeatureCollection f, IConsumer<TKey, TValue> consumer)
            => f.SetConsumerFactory<TKey, TValue>(_ => consumer);

        public static IFeatureCollection SetConsumerFactory<TKey, TValue>(this IFeatureCollection f, Func<ConsumerBuilder<TKey, TValue>, IConsumer<TKey, TValue>> factory)
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
        public static IFeatureCollection StartAtBeginning(this IFeatureCollection f)
        {
            f.Set(StartAtPositionFeature.Beginning);

            return f;
        }

        /// <summary>
        /// Start at the end of the Topic
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFeatureCollection StartAtEnd(this IFeatureCollection f)
        {
            f.Set(StartAtPositionFeature.End);

            return f;
        }

        /// <summary>
        /// Set the GroupId to something unique
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IFeatureCollection UniqueGroupId(this IFeatureCollection f)
        {
            f.Set(new UniqueGroupIdFeature());

            return f;
        }
    }
}
