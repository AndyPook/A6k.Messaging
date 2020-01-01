namespace A6k.Messaging.Features
{
    public interface IStatisticsHandlingFeature : IFeature
    {
        void OnStatistics(string statsJson);
    }
}
