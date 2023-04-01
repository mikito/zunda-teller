using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

namespace ZundaTeller
{
    public enum ChatCompletionServiceRole
    {
        System,
        Uesr,
        Assistant
    }

    public class ChatCompletionServiceMessage
    {
        public ChatCompletionServiceRole role;
        public string content;
    }

    public interface IChatCompletionService : ITestable, IDisposable
    {
        UniTask<ChatCompletionServiceMessage> CompletionAsync(List<ChatCompletionServiceMessage> messages, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> GetCompletionLineAsyncEnumerable(List<ChatCompletionServiceMessage> messages, CancellationToken cancellationToken = default);
    }
}