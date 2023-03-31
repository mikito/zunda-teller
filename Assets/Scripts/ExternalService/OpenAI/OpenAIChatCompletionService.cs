using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZundaTeller.ExternalService
{
    public class OpenAIChatCompletionService : IChatCompletionService
    {
        OpenAIChatCompletionAPI chatCompletionAPI;

        public OpenAIChatCompletionService(string apiKey)
        {
            chatCompletionAPI = new OpenAIChatCompletionAPI(apiKey);
        }

        public async UniTask<ChatCompletionServiceMessage> CompletionAsync(List<ChatCompletionServiceMessage> messages, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await chatCompletionAPI.CompletionRequestAsync(
                    new OpenAIChatCompletionAPI.RequestData() { messages = messages.Select(ConvertToAPIMessage).ToList() },
                    cancellationToken
                );

                var message = response.choices[0].message;
                return new ChatCompletionServiceMessage() { role = ConvertToServiceRole(message.role), content = message.content };
            }
            catch (Exception e)
            {
                throw new ChatCompletionException("chat completion failed.", e);
            }
        }

        public async IAsyncEnumerable<string> GetCompletionLineAsyncEnumerable(List<ChatCompletionServiceMessage> messages, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var request = new OpenAIChatCompletionAPI.RequestData()
            {
                messages = messages.Select(ConvertToAPIMessage).ToList(),
                stream = true
            };

            string buffer = "";

            var enumerable = chatCompletionAPI.GetCompletionRequestAsyncEnumerable(request, cancellationToken);
            await using var enumerator = enumerable.GetAsyncEnumerator(cancellationToken);

            while (true)
            {
                try
                {
                    if (!await enumerator.MoveNextAsync()) break;
                    var chunk = enumerator.Current;
                    buffer += chunk.choices[0].delta.content;
                }
                catch (Exception e)
                {
                    throw new ChatCompletionException("chat completion failed.", e);
                }

                while (FetchLine(ref buffer, out var line))
                {
                    if (string.IsNullOrEmpty(line)) continue; // case: \n\n
                    yield return line;
                }
            }

            if (string.IsNullOrEmpty(buffer)) yield return buffer;
        }

        bool FetchLine(ref string stringBuffer, out string line)
        {
            int index = stringBuffer.IndexOf("\n");
            if (index >= 0)
            {
                line = stringBuffer.Substring(0, index);
                stringBuffer = stringBuffer.Substring(index + 1);
                return true;
            }
            else
            {
                line = null;
                return false;
            }
        }

        OpenAIChatCompletionAPI.Message ConvertToAPIMessage(ChatCompletionServiceMessage message)
        {
            return new OpenAIChatCompletionAPI.Message() { role = ConvertToAPIRoleString(message.role), content = message.content };
        }

        string ConvertToAPIRoleString(ChatCompletionServiceRole role)
        {
            switch (role)
            {
                case ChatCompletionServiceRole.System:
                    return "system";
                case ChatCompletionServiceRole.Uesr:
                    return "user";
                case ChatCompletionServiceRole.Assistant:
                    return "assistant";
                default:
                    throw new ArgumentException($"Unknown role: {role}");
            }
        }

        ChatCompletionServiceRole ConvertToServiceRole(string str)
        {
            switch (str)
            {
                case "system":
                    return ChatCompletionServiceRole.System;
                case "user":
                    return ChatCompletionServiceRole.Uesr;
                case "assistant":
                    return ChatCompletionServiceRole.Assistant;
                default:
                    throw new ArgumentException($"Unknown role string: {str}");
            }
        }

        public void Dispose()
        {
            chatCompletionAPI?.Dispose();
        }

        public async UniTask<TestResult> TestAsync(CancellationToken cancellationToken)
        {
            try
            {
                var request = new OpenAIChatCompletionAPI.RequestData()
                {
                    messages = new List<OpenAIChatCompletionAPI.Message>()
                    {
                        new OpenAIChatCompletionAPI.Message(){role = "user", content = "Respond with just 'OK'"}
                    }
                };
                await chatCompletionAPI.CompletionRequestAsync(request, cancellationToken);
                return TestResult.Success;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception e)
            {
                return TestResult.CreateAsFail(new ChatCompletionException("chat completion test is failed.", e));
            }
        }
    }
}