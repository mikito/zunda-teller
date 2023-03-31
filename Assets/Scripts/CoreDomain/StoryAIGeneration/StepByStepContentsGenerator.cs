using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Runtime.CompilerServices;

namespace ZundaTeller.AIGeneration
{
    public class StepByStepStoryContentsGenerator : IStoryContentsGenerator
    {
        const string SystemPrompt = @"あなたは子供向けの物語のプロ作家です。楽しく教育的な物語を作る作業をアシストしてください。";

        const string UserPromptTemplete =
@"「{TITLE}」という子供向けの物語の内容を考えてください。

## 出力フォーマット
{""content"": <content>, ""emotion"": <emotion>}
{""content"": <content>, ""emotion"": <emotion>}
{""content"": <content>, ""emotion"": <emotion>}

## 出力フォーマットの詳細
- <content>は物語の文章です
- <emotion>は<content>の感情を表す数字で、以下のEmotion Numberから選択します

### Emotion Number
0 -> 普通
1 -> 喜び
2 -> 驚き
3 -> 悲しみ
4 -> 怒り

## 制約事項
- <content>に改行を含めないこと
- <content>は50文字以内にすること
- <content>の語尾は「です・ます」口調にすること
- 1行ずつjson形式で出力し、それ以外の会話文、コメント、補足などの情報は一切返答しないこと

## 物語の内容について
- 抽象的な表現は避け、具体的な内容にすること
- 物語に起承転結の起伏をつけること
- 学びのテーマを設定し暗に物語に反映すること

## 出力の例
{""content"": ""昔々あるところにおじいさんとお婆さんがいました。"", ""emotion"": 0}
{""content"": ""おじいさんは芝刈りに、お婆さんは川に洗濯に行きました。"" , ""emotion"": 0}
{""content"": ""お婆さんが洗濯をしていると、川上から大きな桃が流れてきました。"" , ""emotion"": 2}
{""content"": ""お婆さんはその桃を家に持ち帰りました。"", ""emotion"": 1}
";

        string[] PhasePromptTempletes = new string[]
        {
            "上記のルールに従って、まずは起承転結の「起」の部分を{NUMBER}行で返答してください",
            "次に、起承転結の「承」の部分を{NUMBER}行で返答してください",
            "次に、起承転結の「転」の部分を{NUMBER}行で返答してください",
            "最後に、起承転結の「結」の部分を{NUMBER}行で返答してください",
        };

        IChatCompletionService chatCompletionService;

        public StepByStepStoryContentsGenerator(IChatCompletionService chatCompletionService)
        {
            this.chatCompletionService = chatCompletionService;
        }

        public async IAsyncEnumerable<StoryContent> GenerateAsyncEnumerable(string title, int preferredLength, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            List<ChatCompletionServiceMessage> messages = null;
            try
            {
                var userPrompt = UserPromptTemplete.Replace("{TITLE}", title);

                messages = new List<ChatCompletionServiceMessage>() {
                    new ChatCompletionServiceMessage(){role = ChatCompletionServiceRole.System, content = SystemPrompt},
                    new ChatCompletionServiceMessage(){role = ChatCompletionServiceRole.Uesr, content = userPrompt},
                };
            }
            catch (Exception e)
            {
                throw new AIGenerationException("StoryContentsGenerator: Failed to build prompt.", e);
            }

            int i = 0;
            foreach (var promptTemplete in PhasePromptTempletes)
            {
                // 起結は切り下げ、承転は切り上げ
                int number = 0; 
                if(i == 0 || i == 3 ) number = Mathf.FloorToInt(preferredLength / 4f); 
                else number = Mathf.CeilToInt(preferredLength / 4f); 
                i++;

                var prompt = promptTemplete.Replace("{NUMBER}", number.ToString());

                messages.Add(new ChatCompletionServiceMessage() { role = ChatCompletionServiceRole.Uesr, content = prompt });
                var enumerable = chatCompletionService.GetCompletionLineAsyncEnumerable(messages, cancellationToken);
                await using var enumerator = enumerable.GetAsyncEnumerator(cancellationToken);

                string assistantOutput = "";

                while (true)
                {
                    try
                    {
                        if (!await enumerator.MoveNextAsync()) break;
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception e)
                    {
                        throw new AIGenerationException("StoryContentsGenerator: Failed to complete line.", e);
                    }

                    assistantOutput += enumerator.Current + "\n";
                    yield return ParseLine(enumerator.Current);
                }

                messages.Add(new ChatCompletionServiceMessage() { role = ChatCompletionServiceRole.Assistant, content = assistantOutput });
            }
        }

        StoryContent ParseLine(string line)
        {
            try
            {
               return JsonUtility.FromJson<StoryContent>(line);
            }
            catch (Exception e)
            {
                throw new AIGenerationException($"StoryContentsGenerator: Failed to parse. : {line}", e);
            }
        }
    }
}