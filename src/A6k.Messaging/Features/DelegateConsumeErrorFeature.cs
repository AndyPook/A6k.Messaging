using System;

namespace A6k.Messaging.Features
{
    public class DelegateConsumeErrorFeature : IConsumeErrorFeature
    {
        private readonly Action<Exception> onError;

        public DelegateConsumeErrorFeature(Action<Exception> onError)
        {
            this.onError = onError ?? throw new ArgumentNullException(nameof(onError));
        }

        public void OnConsumeError(Exception exception) => onError(exception);
    }
}
