using System;
using System.Globalization;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.IO;
using Newtonsoft.Json;

namespace A6k.Messaging.Benchmarks
{
    public class SampleMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp22)]
    [MemoryDiagnoser]
    public class MessageSerializationBenchmarks
    {
        private static SampleMessage item = new SampleMessage { Id = 999, Name = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" };

        [Benchmark]
        public void Simple()
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
        }


        private static JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();

        [Benchmark]
        public void String()
        {
            // this is how JsonConvert.Serialize works internally
            var sb = new StringBuilder(256);
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = jsonSerializer.Formatting;
                jsonSerializer.Serialize(jsonWriter, item, typeof(SampleMessage));
            }

            var data = sw.ToString();
        }

        [Benchmark]
        public void MemoryStream()
        {
            var memory = new MemoryStream(256);
            var sw = new StreamWriter(memory);
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = jsonSerializer.Formatting;
                jsonSerializer.Serialize(jsonWriter, item, typeof(SampleMessage));
            }
            var data = memory.ToArray();
        }

        private static readonly RecyclableMemoryStreamManager manager = new RecyclableMemoryStreamManager();

        [Benchmark]
        public void Recyclable()
        {
            using (var m = manager.GetStream())
            {
                using (var sw = new StreamWriter(m))
                {
                    jsonSerializer.Serialize(sw, item, typeof(SampleMessage));
                    sw.Flush();
                    var data = m.ToArray();
                }
            }
        }
    }
}
