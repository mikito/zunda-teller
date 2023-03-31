using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System;

namespace ZundaTeller.AIGeneration
{
    public class StoryTitleGenerator
    {
        const string SystemPrompt = @"あなたは子供向けの物語のプロ作家です。楽しく教育的な物語を作る作業をアシストしてください。";
        const string UserPromptTemplete =
@"面白そうな子供向けのオリジナル物語のタイトルを{NUMBER}個考えてください。

## 制約事項
- タイトルは15文字以内にすること
- 日本語のタイトルにすること
- 以下のjson形式のリストで返答し、json以外の会話文、コメント、補足などの情報は一切返答しないこと

## 出力フォーマット
[
    {""title"": ""Title 1""},
    {""title"": ""Title 2""},
    {""title"": ""Title 3""},
]
";

        IChatCompletionService chatCompletionService;

        public StoryTitleGenerator(IChatCompletionService chatCompletionService)
        {
            this.chatCompletionService = chatCompletionService;
        }

        public async UniTask<List<StoryTitle>> GenerateAsync(int size, CancellationToken cancellationToken = default)
        {
            try
            {
                var userPrompt = UserPromptTemplete.Replace("{NUMBER}", size.ToString());

                var messages = new List<ChatCompletionServiceMessage>() {
                    new ChatCompletionServiceMessage(){role = ChatCompletionServiceRole.System, content = SystemPrompt},
                    new ChatCompletionServiceMessage(){role = ChatCompletionServiceRole.Uesr, content = userPrompt},
                };

                var completion = await chatCompletionService.CompletionAsync(
                    messages,
                    cancellationToken: cancellationToken
                );

                return JsonConvert.DeserializeObject<List<StoryTitle>>(completion.content);
            }
            catch (Exception e)
            {
                throw new AIGenerationException("StoryTitleGenerator: Failed to generate titles. ", e);
            }
        }
    }
}