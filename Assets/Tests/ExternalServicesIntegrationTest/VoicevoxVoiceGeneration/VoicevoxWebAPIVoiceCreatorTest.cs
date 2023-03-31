using System.Collections;
using System;
using NUnit.Framework;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;
using ZundaTeller.ExternalService;

namespace ZundaTeller.Test.ExternalServiceIntegrationTest
{
    public class VoicevoxWebAPIVoiceCreationServiceTest
    {
        [UnityTest]
        public IEnumerator GenerateAudioClipFromWebAPI() => UniTask.ToCoroutine(async () =>
        {
            using var creationService = new VoicevoxWebAPIVoiceCreationService(Env.Get("VOICEVOX_WEB_API_KEY"));
            using var voice = await creationService.CreateVoiceAsync("Test");
            Assert.That(voice.audioClip, Is.Not.Null);
            Assert.That(voice.audioClip.length, Is.Not.Zero);
            var samples = new float[voice.audioClip.samples];
            voice.audioClip.GetData(samples, 0);
            Assert.That(samples, Has.Some.Not.Zero);
        });

        [UnityTest]
        public IEnumerator WhenIncorrectAPIKey_ThrowsVoiceCreationException() => UniTask.ToCoroutine(async () =>
        {
            Exception exception = null;
            try
            {
                using var creationService = new VoicevoxWebAPIVoiceCreationService("incorrect_key");
                await creationService.CreateVoiceAsync("Test");
            }
            catch (Exception e)
            {
                exception = e;
            }

            Assert.That(exception, Is.InstanceOf<VoiceCreationException>());
            Assert.That((exception as VoiceCreationException).Code, Is.EqualTo(VoiceCreationErrorCode.AuthenticationFailed));
        });
    }
}