namespace A6k.Messaging.Features
{
    public interface IMessagePumpWaitFeature
    {
        /// <summary>
        /// The set of named <see cref="Internal.WaitToken"/> to wait for before starting the pump
        /// </summary>
        string[] Names { get; }
    }
}