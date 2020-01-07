namespace A6k.Messaging.Kafka.Features
{
    public interface IStatisticsHandlingFeature
    {
        void OnStatistics(string statsJson);
    }
}
