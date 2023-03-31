using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZundaTeller.Presentation
{
    [Serializable]
    public class BuiltinVoiceCollection
    {
        [SerializeField] List<AudioClip> voices;

        public AudioClip Find(string clipName)
        {
            return voices.Find(v => v.name == clipName);
        }
    }
}