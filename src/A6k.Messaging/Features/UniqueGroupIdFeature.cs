using System;
using System.Collections.Generic;

namespace A6k.Messaging.Features
{
    /// <summary>
    /// Ensures that the gorup.id is unique allowing a consumer to always start at the beginning of a topic
    /// <para>uses State to remember the id across pump stop/start</para>
    /// </summary>
    public class UniqueGroupIdFeature : IConfigFeature, IRequiresState
    {
        private StateFeature state;
        private string uniqueId;

        public void SetState(StateFeature state) => this.state = state;

        public void Configure(IDictionary<string, string> config)
        {
            // NOTE: the id is remembered from the first use
            // this handles the situation where the service is Paused/Resumed
            // as the Consumer is recreated and Configured on resume
            // See MessagePump.RunMessagePump and consumerFactory
            uniqueId = state?.Get<string>(nameof(UniqueGroupIdFeature));
            if (string.IsNullOrEmpty(uniqueId))
            {
                if (config.TryGetValue("group.id", out var groupid))
                    uniqueId = groupid + "-" + Guid.NewGuid().ToString("N");
                else
                    uniqueId = "groupid-" + Guid.NewGuid().ToString("N");

                state?.Set(nameof(UniqueGroupIdFeature), uniqueId);
            }

            config["group.id"] = uniqueId;
        }
    }
}
