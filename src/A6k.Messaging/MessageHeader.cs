namespace A6k.Messaging
{
    public sealed class MessageHeader : IMessageHeader
    {
        public MessageHeader(string key, object value, MessageHeader next)
        {
            Key = key;
            Value = value;
            Next = next;
        }

        public string Key { get; }
        public object Value { get; }

        public MessageHeader Next { get; }
    }
}
