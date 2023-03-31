using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using ZundaTeller.AIGeneration;

namespace ZundaTeller
{
    public class PlayableStoryGeneration : IDisposable
    {
        const int PreferredLength = 16;

        public Queue<PlayableStorySentence> SentenceQueue => sentences;
        public bool IsCompleted { get; private set; }
        public string StoryTitle { get; private set; }

        Queue<PlayableStorySentence> sentences = new Queue<PlayableStorySentence>();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        StoryContentsGenerator storyGenerator = null;
        IVoiceCreationService voiceCreationService = null;

        public PlayableStoryGeneration(StoryContentsGenerator storyGenerator, IVoiceCreationService voiceCreation, string title)
        {
            this.voiceCreationService = voiceCreation;
            this.storyGenerator = storyGenerator;
            StoryTitle = title;
        }

        public async UniTask RunAsync()
        {
            await foreach (var scene in storyGenerator.GenerateAsyncEnumerable(StoryTitle, PreferredLength, cancellationTokenSource.Token))
            {

                await foreach (var page in StoryContentToPlayableAsync(scene))
                {
                    sentences.Enqueue(page);
                }
            }

            IsCompleted = true;
            Debug.Log("Story Generation Completed");
        }

        async IAsyncEnumerable<PlayableStorySentence> StoryContentToPlayableAsync(StoryContent scene)
        {
            // 文章が長めでだれる可能性があるのでセンテンス単位での再生にマッピングする
            var sentenceStrings = scene.content.Replace("。", "。\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var s in sentenceStrings)
            {
                var voice = await voiceCreationService.CreateVoiceAsync(s, cancellationTokenSource.Token);
                yield return new PlayableStorySentence() { sentence = s, voice = voice, emotion = scene.emotion };
            }
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            foreach (var c in sentences) c.Dispose();
            sentences.Clear();
        }
    }
}