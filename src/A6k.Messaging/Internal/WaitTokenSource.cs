using System.Threading;

namespace A6k.Messaging.Internal
{
    /// <summary>
    /// Holds a <see cref="WaitToken"/> that can be Triggered. Used for flow control.
    /// </summary>
    public class WaitTokenSource
    {
        private readonly ManualResetEvent mre = new ManualResetEvent(false);

        public WaitTokenSource(string name)
        {
            Token = new WaitToken(name, mre);
        }

        public WaitToken Token { get; }

        public void Trigger() => mre.Set();
        public void Reset() => mre.Reset();
    }
}
