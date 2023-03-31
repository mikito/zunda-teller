using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZundaTeller.ExternalService
{
    public class VoicevoxWebAPIVoiceCreationService : IVoiceCreationService
    {
        [Serializable]
        public struct Error
        {
            public string errorMessage;

            public VoiceCreationErrorCode ToErrorCode()
            {
                switch (errorMessage)
                {
                    case "invalidApiKey": return VoiceCreationErrorCode.AuthenticationFailed;
                    case "notEnoughPoints": return VoiceCreationErrorCode.LimitationExceeded;
                    case "failed": return VoiceCreationErrorCode.ProcessingFailed;
                    default: return VoiceCreationErrorCode.Unknown;
                }
            }
        }

        HttpClient httpClient;
        string apiKey;

        public VoicevoxWebAPIVoiceCreationService(string apiKey)
        {
            httpClient = new HttpClient();
            this.apiKey = apiKey;
        }

        async UniTask<Stream> SynthesisAsync(string text, CancellationToken cancellationToken = default)
        {
            string url = "https://api.su-shiki.com/v2/voicevox/audio/?speaker=1";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            var parameters = new Dictionary<string, string>
            {
                { "text", text },
                { "key",  apiKey },
            };
            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = null;

            try
            {
                response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    cancellationToken.ThrowIfCancellationRequested();
                    return stream;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    cancellationToken.ThrowIfCancellationRequested();

                    // 空の場合がたまにある
                    if (string.IsNullOrEmpty(message))
                    {
                        var errorMessage = $"voicevox web api error. status : {response.StatusCode}";
                        throw new VoiceCreationException(VoiceCreationErrorCode.Unknown, errorMessage);
                    }
                    var error = JsonUtility.FromJson<Error>(message);
                    throw new VoiceCreationException(error.ToErrorCode());
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (VoiceCreationException) { throw; }
            catch (Exception e)
            {
                response?.Dispose();
                throw new VoiceCreationException(VoiceCreationErrorCode.Unknown, e);
            }
        }

        public async UniTask<VoiceData> CreateVoiceAsync(string text, CancellationToken cancellationToken = default)
        {
            using var stream = await SynthesisAsync(text, cancellationToken);

            AudioClip clip = null;
            try
            {
                clip = await VoicevoxBridge.AudioClipUtil.CreateFromStreamAsync(stream, cancellationToken);
                return new VoiceData() { audioClip = clip };
            }
            catch
            {
                if (clip != null) UnityEngine.Object.Destroy(clip);
                throw;
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public async UniTask<TestResult> TestAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(15);

                var hostUri = new Uri("https://api.su-shiki.com/v2/api/");

                using var request = new HttpRequestMessage(HttpMethod.Post, hostUri);
                var parameters = new Dictionary<string, string> { { "key", apiKey } };
                request.Content = new FormUrlEncodedContent(parameters);

                using var response = await httpClient.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return TestResult.Success;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    cancellationToken.ThrowIfCancellationRequested();
                    var error = JsonUtility.FromJson<Error>(message);
                    return TestResult.CreateAsFail(new VoiceCreationException(error.ToErrorCode()));
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception e)
            {
                return TestResult.CreateAsFail(new VoiceCreationException(VoiceCreationErrorCode.Unknown, e));
            }
        }
    }
}