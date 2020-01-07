using System;

namespace A6k.Messaging.Features
{
    public interface IConsumeErrorFeature
    {
        void OnConsumeError(Exception exception);
    }
}
