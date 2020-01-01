using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public class KafkaRestProxyClient
    {
        private const string JsonAcceptType = "application/vnd.kafka+json";
        private const string JsonContentType = "application/vnd.kafka.json.v2+json";
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly HttpClient client;

        public KafkaRestProxyClient() : this(new HttpClient()) { }

        public KafkaRestProxyClient(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<List<string>> GetTopicsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/topics");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonAcceptType));

            var response = await client.SendAsync(request);
            return await ReadJsonStreamAsync<List<string>>(response);
        }

        public async Task<KafkaTopicMeta> GetTopicAsync(string topic)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/topics/{topic}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonAcceptType));

            var response = await client.SendAsync(request);
            return await ReadJsonStreamAsync<KafkaTopicMeta>(response);
        }

        public async Task<PublishResponse> PublishAsync<TKey, TValue>(string topic, TKey key, TValue value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"/topics/{topic}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonAcceptType));

            var records = new PublishRecords<TKey, TValue>().Add(key, value);

            var stream = WriteJsonStream(records);

            var body = new StreamContent(stream);
            body.Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);
            request.Content = body;

            var response = await client.SendAsync(request);
            return await ReadJsonStreamAsync<PublishResponse>(response);
        }

        private Stream WriteJsonStream<T>(T item)
        {
            var serializer = JsonSerializer.Create(SerializerSettings);

            var stream = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8, 4096, true))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, item);
            }
            stream.Position = 0;
            return stream;
        }

        private async Task<T> ReadJsonStreamAsync<T>(HttpResponseMessage message)
        {
            using (var s = await message.Content.ReadAsStreamAsync())
            using (var sr = new StreamReader(s))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = JsonSerializer.Create(SerializerSettings);

                // read the json from a stream
                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                return serializer.Deserialize<T>(reader);
            }
        }

        public class KafkaTopicMeta
        {
            public string Name { get; set; }
            public Dictionary<string, string> Configs { get; set; }
            public List<PartitionMeta> Partitions { get; set; }

            public class PartitionMeta
            {
                public int Partition { get; set; }
                public int Leader { get; set; }

                public class RelicaMeta
                {
                    public List<int> Broker { get; set; }
                    public bool Leader { get; set; }
                    public bool InSync { get; set; }
                }
            }
        }

        public class PublishRecords<TKey, TValue>
        {
            public List<PublishRecord> Records { get; set; } = new List<PublishRecord>();

            public PublishRecords<TKey, TValue> Add(TKey key, TValue value)
            {
                Records.Add(new PublishRecord { Key = key, Value = value });
                return this;
            }

            public class PublishRecord
            {
                public TKey Key { get; set; }
                public TValue Value { get; set; }
            }
        }

        public class PublishResponse
        {
            [JsonProperty(PropertyName = "key_schema_id")]
            public int? KeySchemaId { get; set; }
            [JsonProperty(PropertyName = "value_schema_id")]
            public int? ValueSchemaId { get; set; }
            public List<OffsetDetail> Offsets { get; set; }

            public class OffsetDetail
            {
                public int Partition { get; set; }
                public int Offset { get; set; }
                [JsonProperty(PropertyName = "error_code")]
                public int? ErrorCode { get; set; }
                public string Error { get; set; }
            }
        }
    }
}
