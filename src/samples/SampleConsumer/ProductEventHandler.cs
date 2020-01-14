using System;
using System.Threading.Tasks;
using A6k.Messaging;

namespace SampleConsumer
{
    /// <summary>
    /// Simplest possible handler.
    /// Full DI is supported
    /// </summary>
    public class ProductEventHandler : IMessageHandler<string, ProductEvent>
    {
        public Task HandleAsync(IMessage<string, ProductEvent> message)
        {
            Console.WriteLine($"{nameof(ProductEventHandler)}: id={message.Value.Identifier}");

            return Task.CompletedTask;
        }
    }
}
