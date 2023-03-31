using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace ZundaTeller.ExternalService
{
    public class OpenAIChatCompletionAPI : IDisposable
    {
        const string API_URL = "https://api.openai.com/v1/chat/completions";
        string apiKey;
        JsonSerializerSettings settings = new JsonSerializerSettings();
        HttpClient httpClient;

        public OpenAIChatCompletionAPI(string apiKey)
        {
            this.httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(15);
            this.apiKey = apiKey;
            settings.NullValueHandling = NullValueHandling.Ignore;
        }

        public async Task<ResponseData> CompletionRequestAsync(RequestData requestData, CancellationToken cancellationToken)
        {
            if (requestData.stream.HasValue && requestData.stream.Value)
            {
                throw new ArgumentException("stream must be false.");
            }

            var json = JsonConvert.SerializeObject(requestData, settings);

            using var request = new HttpRequestMessage(HttpMethod.Post, API_URL);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                cancellationToken.ThrowIfCancellationRequested();
                return JsonConvert.DeserializeObject<ResponseData>(result);
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                cancellationToken.ThrowIfCancellationRequested();
                throw new WebException($"request failed. {(int)response.StatusCode} {response.StatusCode}\n{message}");
            }
        }

        public async IAsyncEnumerable<ResponseChunkData> GetCompletionRequestAsyncEnumerable(RequestData requestData, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!requestData.stream.HasValue || !requestData.stream.Value)
            {
                throw new ArgumentException("stream must be true.");
            }

            var json = JsonConvert.SerializeObject(requestData, settings);

            using var request = new HttpRequestMessage(HttpMethod.Post, API_URL);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                cancellationToken.ThrowIfCancellationRequested();

                using var reader = new StreamReader(stream, Encoding.UTF8);

                string line = null;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (string.IsNullOrEmpty(line)) continue;
                    var chunkString = line.Substring(6); // "data: "
                    if (chunkString == "[DONE]") break;
                    yield return JsonConvert.DeserializeObject<ResponseChunkData>(chunkString);
                }
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                cancellationToken.ThrowIfCancellationRequested();
                throw new WebException($"request failed. {(int)response.StatusCode} {response.StatusCode}\n{message}");
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        [System.Serializable]
        public class RequestData
        {
            public string model = "gpt-3.5-turbo";
            public List<Message> messages;
            public float? temperature = null; // [0.0 - 2.0]
            public float? top_p = null;
            public int? n = null;
            public bool? stream = null;
            public List<string> stop = null;
            public int? max_tokens = null;
            public float? presence_penalty = null;
            public float? frequency_penalty = null;
            public Dictionary<int, int> logit_bias = null;
            public string user = null;
        }

        [System.Serializable]
        public class Message
        {
            public string role;
            public string content;
        }

        [System.Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }

        [System.Serializable]
        public class Choice
        {
            public Message message;
            public string finish_reason;
            public int index;
        }

        [System.Serializable]
        public class ResponseData
        {
            public string id;
            public string @object;
            public int created;
            public string model;
            public Usage usage;
            public List<Choice> choices;
        }

        [System.Serializable]
        public class ChunkChoice
        {
            public Message delta;
            public int index;
            public object finish_reason;
        }

        [System.Serializable]
        public class ResponseChunkData
        {
            public string id;
            public string @object;
            public int created;
            public string model;
            public List<ChunkChoice> choices;
        }
    }
}