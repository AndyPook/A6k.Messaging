namespace A6k.Messaging.Features
{
    public interface IMessagePumpWaitFeature : IFeature
    {
        string[] Names { get; }
    }
}