using A6k.Messaging.Features;

namespace A6k.Messaging.Kafka.Features
{
    /// <summary>
    /// A kafka concept of where in the queue to start
    /// <para>uses State to remember the id across pump stop/start</para>
    /// </summary>
    public class StartAtPositionFeature : IPartitionAssignmentFeature, IRequiresState
    {
        // magic Kafka value that means "at the beginning of the partition"
        // copied from <see ref="Confluent.Kafka.Options"/>
        public const long RD_KAFKA_OFFSET_BEGINNING = -2;

        // magic Kafka value that means "at the end of the partition"
        // copied from <see ref="Confluent.Kafka.Options"/>
        public const long RD_KAFKA_OFFSET_END = -1;

        public static StartAtPositionFeature Beginning => new StartAtPositionFeature(RD_KAFKA_OFFSET_BEGINNING);
        public static StartAtPositionFeature End => new StartAtPositionFeature(RD_KAFKA_OFFSET_END);

        private readonly long position;
        private StateFeature state;

        private StartAtPositionFeature(long position)
        {
            this.position = position;
        }

        public void SetState(StateFeature state) => this.state = state;

        public long? PartitionAssigned(string topic, int partition)
        {
            // NOTE: this will only be set the first time the feature is used
            // this handles the situation where the service is Paused/Resumed
            // as the Consumer is recreated and Configured on resume
            // See MessagePump.RunMessagePump and consumerFactory
            if (state?.IsSet(nameof(StartAtPositionFeature)) == true)
                return null;

            state?.Set(nameof(StartAtPositionFeature));
            return position;
        }

        public void PartitionRevoked(string topic, int partition, long offset) { }
    }
}
