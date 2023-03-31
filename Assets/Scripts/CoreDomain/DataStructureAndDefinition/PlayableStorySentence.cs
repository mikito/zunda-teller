using System;

namespace ZundaTeller
{
    public class PlayableStorySentence : IDisposable
    {
        public string sentence;
        public VoiceData voice;
        public Emotion emotion;

        public void Dispose()
        {
            voice?.Dispose();
        }
    }
}