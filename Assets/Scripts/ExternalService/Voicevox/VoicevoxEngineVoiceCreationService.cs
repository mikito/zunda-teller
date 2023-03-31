using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using VoicevoxBridge;
using System.Net.Http;

namespace ZundaTeller.ExternalService
{
    public class VoicevoxEngineVoiceCreationService : IVoiceCreationService
    {
        const int SpeakerZundamonAmaAma = 1;

        string host;
        VoicevoxPlayer voicevox;

        public VoicevoxEngineVoiceCreationService(string voicevoxEngineHost)
        {
            host = voicevoxEngineHost;
            voicevox = new VoicevoxPlayer(voicevoxEngineHost);
            voicevox.EnableLog = false;
        }

        public async UniTask<VoiceData> CreateVoiceAsync(string text, CancellationToken cancellationToken = default)
        {
            try
            {
                var voice = await voicevox.CreateVoice(SpeakerZundamonAmaAma, text, cancellationToken).AsUniTask();
                return new VoiceData() { audioClip = voice.AudioClip };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new VoiceCreationException(VoiceCreationErrorCode.Unknown, e);
            }
        }

        public void Dispose()
        {
            voicevox.Dispose();
        }

        public async UniTask<TestResult> TestAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                var hostUri = new Uri(host);
                using var response = await httpClient.GetAsync(hostUri.ToString() + "version", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return TestResult.Success;
                }
                else
                {
                    return TestResult.CreateAsFail(new VoiceCreationException(VoiceCreationErrorCode.Unknown));
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception e)
            {
                return TestResult.CreateAsFail(e);
            }
        }
    }
}