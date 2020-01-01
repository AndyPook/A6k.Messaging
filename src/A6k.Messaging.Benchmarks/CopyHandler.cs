using System;
using System.Threading.Tasks;
using A6k.Messaging;

namespace A6k.Messaging.Benchmarks
{
    public class CopyHandler : IMessageHandler<byte[], byte[]>
    {
        private readonly IMessageProducer<byte[], byte[]> producer;

        public CopyHandler(IMessageProducer<byte[], byte[]> producer)
        {
            this.producer = producer ?? throw new ArgumentNullException(nameof(producer));
        }

        public async Task HandleAsync(IMessage<byte[], byte[]> message)
        {
            await producer.ProduceAsync(message.Key, message.Value);
        }
    }
}
