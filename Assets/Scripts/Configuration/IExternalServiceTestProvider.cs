namespace ZundaTeller.Configuration
{
    public interface IExternalServiceTestProvider
    {
        ITestable ProvideTestableForChatCompletion(string apiKey);
        ITestable ProvideTestableForAPIVoiceCreation(string apiKey);
        ITestable ProvideTestableForLocalEngineVoiceCreation(string host);
    }
}