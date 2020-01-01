using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace A6k.Messaging.Kafka
{
    /// <summary>
    /// Access to the set of Serializers and Deserializers for use within the Confulent.Kafka library.
    /// Provides a Json serde for non-intrinsic types
    /// </summary>
    public static class SerdesRegistry
    {
        private static readonly Dictionary<Type, object> serializers = new Dictionary<Type, object>
        {
            { typeof(Null), Serializers.Null },
            { typeof(int), Serializers.Int32 },
            { typeof(string), Serializers.Utf8 },
            { typeof(long), Serializers.Int64 },
            { typeof(float), Serializers.Single },
            { typeof(double), Serializers.Double },
            { typeof(byte[]), Serializers.ByteArray }
        };

        private static readonly Dictionary<Type, object> deserializers = new Dictionary<Type, object>
        {
            { typeof(Null), Deserializers.Null },
            { typeof(Ignore), Deserializers.Ignore },
            { typeof(int), Deserializers.Int32 },
            { typeof(string), Deserializers.Utf8 },
            { typeof(long), Deserializers.Int64 },
            { typeof(float), Deserializers.Single },
            { typeof(double), Deserializers.Double },
            { typeof(byte[]), Deserializers.ByteArray }
        };

        public static ISerializer<T> GetSerializer<T>()
        {
            if (serializers.TryGetValue(typeof(T), out var x))
                return (ISerializer<T>)x;

            return new JsonSerializer<T>();
        }

        public static IDeserializer<T> GetDeserializer<T>()
        {
            if (deserializers.TryGetValue(typeof(T), out var x))
                return (IDeserializer<T>)x;

            return new JsonDeserializer<T>();
        }
        public static ISerializer<T> GetJsonSerializer<T>(JsonSerializerSettings settings = null) => new JsonSerializer<T>(settings);

        public static IDeserializer<T> GetJsonDeserializer<T>(JsonSerializerSettings settings = null) => new JsonDeserializer<T>(settings);

        public class JsonSerializer<T> : ISerializer<T>
        {
            private readonly JsonSerializerSettings settings;

            public JsonSerializer(JsonSerializerSettings settings = null)
            {
                this.settings = settings;
            }

            public byte[] Serialize(T data, SerializationContext context)
            {
                //TODO optimize
                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            }
        }

        public class JsonDeserializer<T> : IDeserializer<T>
        {
            private readonly JsonSerializerSettings settings;

            public JsonDeserializer(JsonSerializerSettings settings = null)
            {
                this.settings = settings;
            }

            public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                //TODO optimize
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data.ToArray()), settings);
            }
        }
    }
}
