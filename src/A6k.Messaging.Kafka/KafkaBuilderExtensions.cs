
using Confluent.Kafka;
using A6k.Messaging.Kafka.Features;
using Newtonsoft.Json;

namespace A6k.Messaging.Kafka
{
    public static class KafkaBuilderExtensions
    {
        public static ConsumerBuilder<TKey, TValue> SetDeserializers<TKey, TValue>(this ConsumerBuilder<TKey, TValue> builder, IKafkaSerializationFeature serializationFeature)
        {
            if (serializationFeature == null)
                builder.SetDefaultDeserializers();
            else
            {
                if (serializationFeature.KeySettings == null)
                    builder.SetDefaultKeyDeserializer();
                else
                    builder.SetKeyDeserializer(serializationFeature.KeySettings);

                if (serializationFeature.ValueSettings == null)
                    builder.SetDefaultValueDeserializer();
                else
                    builder.SetValueDeserializer(serializationFeature.ValueSettings);
            }

            return builder;
        }

        public static ProducerBuilder<TKey, TValue> SetSerializers<TKey, TValue>(this ProducerBuilder<TKey, TValue> builder, IKafkaSerializationFeature serializationFeature)
        {
            if (serializationFeature == null)
                builder.SetDefaultSerializers();
            else
            {
                if (serializationFeature.KeySettings == null)
                    builder.SetDefaultKeySerializer();
                else
                    builder.SetKeySerializer(serializationFeature.KeySettings);

                if (serializationFeature.ValueSettings == null)
                    builder.SetDefaultValueSerializer();
                else
                    builder.SetValueDeserializer(serializationFeature.ValueSettings);
            }

            return builder;
        }

        public static ConsumerBuilder<TKey, TValue> SetDefaultDeserializers<TKey, TValue>(this ConsumerBuilder<TKey, TValue> builder)
        {
            return builder
                .SetKeyDeserializer(SerdesRegistry.GetDeserializer<TKey>())
                .SetValueDeserializer(SerdesRegistry.GetDeserializer<TValue>());
        }
        public static ConsumerBuilder<TKey, TValue> SetDefaultKeyDeserializer<TKey, TValue>(this ConsumerBuilder<TKey, TValue> builder)
        {
            return builder.SetKeyDeserializer(SerdesRegistry.GetDeserializer<TKey>());
        }
        public static ConsumerBuilder<TKey, TValue> SetDefaultValueDeserializer<TKey, TValue>(this ConsumerBuilder<TKey, TValue> builder)
        {
            return builder.SetValueDeserializer(SerdesRegistry.GetDeserializer<TValue>());
        }

        public static ProducerBuilder<TKey, TValue> SetDefaultSerializers<TKey, TValue>(this ProducerBuilder<TKey, TValue> builder)
        {
            return builder
                .SetKeySerializer(SerdesRegistry.GetSerializer<TKey>())
                .SetValueSerializer(SerdesRegistry.GetSerializer<TValue>());
        }
        public static ProducerBuilder<TKey, TValue> SetDefaultKeySerializer<TKey, TValue>(this ProducerBuilder<TKey, TValue> builder)
        {
            return builder.SetKeySerializer(SerdesRegistry.GetSerializer<TKey>());
        }
        public static ProducerBuilder<TKey, TValue> SetDefaultValueSerializer<TKey, TValue>(this ProducerBuilder<TKey, TValue> builder)
        {
            return builder.SetValueSerializer(SerdesRegistry.GetSerializer<TValue>());
        }

        public static ConsumerBuilder<TKey, TValue> SetKeyDeserializer<TKey, TValue>(this ConsumerBuilder<TKey, TValue> builder, JsonSerializerSettings settings)
        {
            return builder.SetKeyDeserializer(SerdesRegistry.GetJsonDeserializer<TKey>(settings));
        }
        public static ConsumerBuilder<TKey, TValue> SetValueDeserializer<TKey, TValue>(this ConsumerBuilder<TKey, TValue> builder, JsonSerializerSettings settings)
        {
            return builder.SetValueDeserializer(SerdesRegistry.GetJsonDeserializer<TValue>(settings));
        }

        public static ProducerBuilder<TKey, TValue> SetKeySerializer<TKey, TValue>(this ProducerBuilder<TKey, TValue> builder, JsonSerializerSettings settings)
        {
            return builder.SetKeySerializer(SerdesRegistry.GetJsonSerializer<TKey>(settings));
        }
        public static ProducerBuilder<TKey, TValue> SetValueDeserializer<TKey, TValue>(this ProducerBuilder<TKey, TValue> builder, JsonSerializerSettings settings)
        {
            return builder.SetValueSerializer(SerdesRegistry.GetJsonSerializer<TValue>(settings));
        }
    }
}
