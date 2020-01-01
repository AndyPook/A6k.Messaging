namespace A6k.Messaging
{
    public interface IMessageHeader
    {
        string Key { get; }
        object Value { get; }
    }
}
