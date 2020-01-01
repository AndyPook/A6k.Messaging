using A6k.Messaging.Features;

namespace A6k.Messaging.Kafka.Features
{
    public interface IStatisticsHandlingFeature : IFeature
    {
        void OnStatistics(string statsJson);
    }
}
