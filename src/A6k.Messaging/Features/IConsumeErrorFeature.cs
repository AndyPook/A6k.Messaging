using System;

namespace A6k.Messaging.Features
{
    public interface IConsumeErrorFeature : IFeature
    {
        void OnConsumeError(Exception exception);
    }
}
