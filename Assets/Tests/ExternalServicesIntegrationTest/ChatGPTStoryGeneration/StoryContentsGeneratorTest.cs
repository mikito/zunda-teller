using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;
using System.Threading;
using ZundaTeller.AIGeneration;
using ZundaTeller.ExternalService;

namespace ZundaTeller.Test.ExternalServiceIntegrationTest
{
    public class StoryContentsGeneratorTest
    {
        [UnityTest]
        public IEnumerator Generate10ContentsStoryAsStream() => UniTask.ToCoroutine(async () =>
        {
            int preferred = 10;
            var service = new OpenAIChatCompletionService(Env.Get("OPEN_AI_API_KEY"));
            var generator = new StoryContentsGenerator(service);
            int count = 0;
            await foreach (var s in generator.GenerateAsyncEnumerable("ある日森の中、アライグマさんに出会った", preferred, default(CancellationToken)))
            {
                Debug.Log(s.emotion + " : " + s.content);
                count++;
            }
            Assert.That(count, Is.GreaterThanOrEqualTo(preferred - (preferred / 2)));
            Assert.That(count, Is.LessThanOrEqualTo(preferred + (preferred / 2)));
        });
    }
}