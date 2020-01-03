using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace A6k.Messaging.Internal
{
    public static class WaitTokenExtensions
    {
        public static Task WhenAll(this IEnumerable<WaitToken> waitTokens, params string[] tokenNames)
            => WhenAll(waitTokens, default, tokenNames);

        public static async Task WhenAll(this IEnumerable<WaitToken> waitTokens, CancellationToken cancellationToken, params string[] tokenNames)
        {
            // get list of tasks with selected names
            if (tokenNames != null && tokenNames.Length > 0)
            {
                var requestedTokens = waitTokens.Where(w => tokenNames.Contains(w.Name)).ToArray();
                if (requestedTokens.Length != tokenNames.Length)
                    throw new InvalidOperationException("Not all wait tokens are available");

                waitTokens = requestedTokens;
            }

            await Task.WhenAll(waitTokens.Select(t => t.GetTask(cancellationToken)));
        }
    }
}
