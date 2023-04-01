using System;

namespace ZundaTeller
{
    public class ChatCompletionException : Exception
    {
        public ChatCompletionException(string message) : base(message) { }
        public ChatCompletionException(string message, Exception innerException) : base(message, innerException) { }
    }
}