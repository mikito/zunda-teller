using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace ZundaTeller
{
    public interface IVoiceCreationService : ITestable, IDisposable
    {
        UniTask<VoiceData> CreateVoiceAsync(string text, CancellationToken cancellationToken = default);
    }
}