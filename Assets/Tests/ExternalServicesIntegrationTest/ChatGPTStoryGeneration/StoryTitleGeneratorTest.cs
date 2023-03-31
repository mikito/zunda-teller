using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;
using System.Threading;
using ZundaTeller.AIGeneration;
using ZundaTeller.ExternalService;

namespace ZundaTeller.Test.ExternalServiceIntegrationTest
{
    public class StoryTitleGeneratorTest
    {
        [UnityTest]
        public IEnumerator Generate10StoryTitle() => UniTask.ToCoroutine(async () =>
        {
            var service = new OpenAIChatCompletionService(Env.Get("OPEN_AI_API_KEY"));
            var generator = new StoryTitleGenerator(service);
            var storyTitles = await generator.GenerateAsync(10, default(CancellationToken));

            foreach (var t in storyTitles) Debug.Log(t.title);
            Assert.That(storyTitles.Count, Is.EqualTo(10));
        });
    }
}
