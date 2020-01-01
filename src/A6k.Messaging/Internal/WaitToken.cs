using System;
using System.Threading;
using System.Threading.Tasks;

namespace A6k.Messaging.Internal
{
    /// <summary>
    /// Used to cause services to "wait" for some other part to Trigger
    /// </summary>
    public sealed class WaitToken
    {
        private readonly ManualResetEvent mre;

        internal WaitToken(string name, ManualResetEvent mre)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            this.mre = mre ?? throw new ArgumentNullException(nameof(mre));
        }

        public string Name { get; }

        /// <summary>
        /// Create a Task that will complete when the parent <see cref="WaitTokenSource"/> is Triggered
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task GetTask(CancellationToken cancellationToken = default)
            => Task.Run(() => WaitHandle.WaitAny(new WaitHandle[] { mre, cancellationToken.WaitHandle }));
    }
}
