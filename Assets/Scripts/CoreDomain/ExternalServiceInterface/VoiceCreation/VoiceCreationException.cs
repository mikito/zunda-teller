using System;

namespace ZundaTeller
{
    public enum VoiceCreationErrorCode
    {
        Unknown,
        AuthenticationFailed,
        LimitationExceeded,
        ProcessingFailed,
    }

    public class VoiceCreationException : Exception
    {
        public VoiceCreationErrorCode Code;

        public VoiceCreationException(VoiceCreationErrorCode code) : base(code.ToString())
        {
            this.Code = code;
        }

        public VoiceCreationException(VoiceCreationErrorCode code, string message) : base(message)
        {
            this.Code = code;
        }

        public VoiceCreationException(VoiceCreationErrorCode code, Exception inner) : base(code.ToString(), innerException: inner)
        {
            this.Code = code;
        }
    }
}