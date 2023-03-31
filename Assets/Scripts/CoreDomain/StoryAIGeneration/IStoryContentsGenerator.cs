using System.Collections.Generic;
using System.Threading;

namespace ZundaTeller.AIGeneration
{
    public interface IStoryContentsGenerator
    {
        IAsyncEnumerable<StoryContent> GenerateAsyncEnumerable(string title, int preferredLength, CancellationToken cancellationToken);
    }
}