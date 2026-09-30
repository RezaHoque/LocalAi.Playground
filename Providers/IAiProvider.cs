using System;
using System.Collections.Generic;
using System.Text;

namespace LocalAI.Playground.Providers
{
    public interface IAiProvider
    {
        IAsyncEnumerable<string> ChatAsync(
        string prompt,
        CancellationToken cancellationToken = default);

        void ResetConversation();
    }
}
