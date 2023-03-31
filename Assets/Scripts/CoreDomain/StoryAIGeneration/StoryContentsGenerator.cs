using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Runtime.CompilerServices;

namespace ZundaTeller.AIGeneration
{
    public class StoryContentsGenerator
    {
        const string SystemPrompt = @"あなたは子供向けの物語のプロ作家です。楽しく教育的な物語を作る作業をアシストしてください。";

        const string UserPromptTemplete =
@"「{TITLE}」という子供向けの物語の内容を考えてください。

## 出力フォーマット
{emotion}: {content}
{emotion}: {content}
{emotion}: {content}

## 出力フォーマットの詳細
- {content}は物語の内容を表す50文字以下の文章です
- {emotion}は{content}の感情を表す数字で、以下のEmotion Numberから選択します

### Emotion Number
0 -> 普通
1 -> 喜び
2 -> 驚き
3 -> 悲しみ
4 -> 怒り

## 制約事項
- {PREFERRED_LENGTH}行程度の物語にすること
- 文章の語尾は「です、ます」口調にすること
- フォーマット以外の会話文、コメント、補足などの情報は一切返答しないこと

## 物語の内容について
- 抽象的な表現は避け、具体的な物語の内容にすること
- 物語に起承転結の起伏をつけること
- 学びのテーマを設定し暗に物語に反映すること

## 出力の例
0: 昔々あるところにおじいさんとお婆さんがいました。
0: おじいさんは芝刈りに、お婆さんは川に洗濯に行きました。
2: お婆さんが洗濯をしていると、川上から大きな桃が流れてきました。
1: お婆さんはその桃を家に持ち帰りました。
";

        IChatCompletionService chatCompletionService;

        public StoryContentsGenerator(IChatCompletionService chatCompletionService)
        {
            this.chatCompletionService = chatCompletionService;
        }

        // NOTE: gpt-3.5-turboだとpreferredLengthが効きにくい
        public async IAsyncEnumerable<StoryContent> GenerateAsyncEnumerable(string title, int preferredLength, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            List<ChatCompletionServiceMessage> messages = null;
            try
            {
                var userPrompt = UserPromptTemplete.Replace("{PREFERRED_LENGTH}", preferredLength.ToString());
                userPrompt = userPrompt.Replace("{TITLE}", title);

                messages = new List<ChatCompletionServiceMessage>() {
                    new ChatCompletionServiceMessage(){role = ChatCompletionServiceRole.System, content = SystemPrompt},
                    new ChatCompletionServiceMessage(){role = ChatCompletionServiceRole.Uesr, content = userPrompt},
                };
            }
            catch (Exception e)
            {
                throw new AIGenerationException("StoryContentsGenerator: Failed to build prompt.", e);
            }

            var enumerable = chatCompletionService.GetCompletionLineAsyncEnumerable(messages, cancellationToken);
            await using var enumerator = enumerable.GetAsyncEnumerator(cancellationToken);

            while (true)
            {
                try
                {
                    if (!await enumerator.MoveNextAsync()) break;
                }
                catch (Exception e)
                {
                    throw new AIGenerationException("StoryContentsGenerator: Failed to complete line.", e);
                }

                yield return ParseLine(enumerator.Current);
            }
        }

        StoryContent ParseLine(string line)
        {
            try
            {
                var emotionString = line.Substring(0, 1);
                Emotion emotion = (Emotion)Enum.Parse(typeof(Emotion), emotionString);
                var content = line.Substring(3);
                return new StoryContent() { emotion = emotion, content = content };
            }
            catch (Exception e)
            {
                throw new AIGenerationException("StoryContentsGenerator: Failed to parse. ", e);
            }
        }
    }
}