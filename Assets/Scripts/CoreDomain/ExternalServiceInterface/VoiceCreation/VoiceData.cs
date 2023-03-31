using UnityEngine;
using System;

namespace ZundaTeller
{
    public class VoiceData : IDisposable
    {
        public AudioClip audioClip;

        public void Dispose()
        {
            if (audioClip != null) UnityEngine.Object.Destroy(audioClip);
        }
    }
}