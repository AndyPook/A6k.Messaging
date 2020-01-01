namespace A6k.Messaging.Features
{
    public interface IPartitionEofFeature : IFeature
    {
        void PartitionIsEof(string topic, int partition);
    }
}
