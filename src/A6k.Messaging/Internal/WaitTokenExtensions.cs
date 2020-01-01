using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace A6k.Messaging.Internal
{
    public static class WaitTokenExtensions
    {
        // this is pattern is "duplicated" in other places such as Game.Eventing
        // this is deliberate as we do not want to take dependencies
        // Using Tuple means that they are compatible
        // So WaitTokens created anywhere are compatible and can WhenAll'd for anywhere

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
