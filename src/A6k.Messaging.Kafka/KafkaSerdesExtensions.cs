using System;
using System.Collections.Generic;

using Confluent.Kafka;
using A6k.Messaging.Kafka.Features;
using A6k.Messaging.Kafka.Json;

namespace A6k.Messaging.Kafka
{
    public static class KafkaSerdesExtensions
    {
        private static readonly HashSet<Type> intrinsicTypes = new HashSet<Type>
        {
            typeof(Null),
            typeof(Ignore),
            typeof(int),
            typeof(string),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(byte[]),
        };

        public static bool IsIntrinsic<T>() => intrinsicTypes.Contains(typeof(T));
        public static bool IsIntrinsic(Type type) => intrinsicTypes.Contains(type);

        private static IKafkaSerializerFactory serializerFactory = new KafkaJsonSerializerFactory();
        private static IKafkaDeserializerFactory deserializerFactory = new KafkaJsonDeserializerFactory();

        public static void SetDefaultSerdes(IKafkaSerializerFactory serializerFactory, IKafkaDeserializerFactory deserializerFactory)
        {
            KafkaSerdesExtensions.serializerFactory = serializerFactory;
            KafkaSerdesExtensions.deserializerFactory = deserializerFactory;
        }

        public static KafkaSerializationFeature<TKey, TValue> GetDefaultSerializers<TKey, TValue>()
        {
            return new KafkaSerializationFeature<TKey, TValue>
            {
                KeySerializer = IsIntrinsic<TKey>() ? null : serializerFactory.Create<TKey>(),
                ValueSerializer = IsIntrinsic<TValue>() ? null : serializerFactory.Create<TValue>()
            };
        }

        public static KafkaDeserializationFeature<TKey, TValue> GetDefaultDeserializers<TKey, TValue>()
        {
            return new KafkaDeserializationFeature<TKey, TValue>
            {
                KeyDeserializer = IsIntrinsic<TKey>() ? null : deserializerFactory.Create<TKey>(),
                ValueDeserializer = IsIntrinsic<TValue>() ? null : deserializerFactory.Create<TValue>()
            };
        }

        public static ProducerBuilder<TKey, TValue> SetSerializers<TKey, TValue>(this ProducerBuilder<TKey, TValue> builder, KafkaSerializationFeature<TKey, TValue> serializationFeature = null)
        {
            if (serializationFeature == null)
                serializationFeature = GetDefaultSerializers<TKey, TValue>();

            if (serializationFeature?.KeySerializer == null && !IsIntrinsic<TKey>())
                throw new ArgumentException($"Key type is not intrinsic, a serializer must be set");
            if (serializationFeature?.ValueSerializer == null && !IsIntrinsic<TValue>())
                throw new ArgumentException($"Value type is not intrinsic, a serializer must be set");

            builder.SetKeySerializer(serializationFeature.KeySerializer);
            builder.SetValueSerializer(serializationFeature.ValueSerializer);

            return builder;
        }

        public static ConsumerBuilder<TKey, TValue> SetDeserializers<TKey, TValue>(this ConsumerBuilder<TKey, TValue> builder, KafkaDeserializationFeature<TKey, TValue> serializationFeature = null)
        {
            if (serializationFeature == null)
                serializationFeature = GetDefaultDeserializers<TKey, TValue>();

            if (serializationFeature?.KeyDeserializer == null && !IsIntrinsic<TKey>())
                throw new ArgumentException($"Key type is not intrinsic, a deserializer must be set");
            if (serializationFeature?.ValueDeserializer == null && !IsIntrinsic<TValue>())
                throw new ArgumentException($"Value type is not intrinsic, a deserializer must be set");

            builder.SetKeyDeserializer(serializationFeature.KeyDeserializer);
            builder.SetValueDeserializer(serializationFeature.ValueDeserializer);

            return builder;
        }
    }
}
