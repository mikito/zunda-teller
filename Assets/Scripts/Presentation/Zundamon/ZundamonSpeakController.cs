using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ZundaTeller.Presentation
{
    public class ZundamonSpeakController
    {
        const int SpeakerZundamonAmaAma = 1;

        ZundamonView zundamonView;

        public ZundamonView View => zundamonView;

        public ZundamonSpeakController(ZundamonView zundamonView)
        {
            this.zundamonView = zundamonView;
        }

        public async UniTask SpeakAsync(AudioClip audioClip, Emotion emotion = Emotion.Normal, CancellationToken cancellationToken = default)
        {
            try
            {
                zundamonView.Speak = true;
                zundamonView.Emotion = emotion;
                await PlayAudioClipAsync(audioClip, cancellationToken);
            }
            finally
            {
                if (zundamonView != null) zundamonView.Speak = false;
            }
        }

        async UniTask PlayAudioClipAsync(AudioClip clip, CancellationToken cancellationToken = default)
        {
            zundamonView.AudioSource.clip = clip;
            zundamonView.AudioSource.Play();

            try
            {
                await UniTask.WaitUntil(() =>
                {
                    if (zundamonView == null) return false;
                    return !zundamonView.AudioSource.isPlaying;
                }, cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                if (zundamonView != null && zundamonView.AudioSource != null)
                {
                    zundamonView.AudioSource.Stop();
                }
                throw;
            }
        }
    }
}