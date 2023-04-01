using System;
using System.Collections.Generic;
using ZundaTeller.Configuration;

namespace ZundaTeller.Presentation
{
    public interface IZundaContext
    {
        public INavigationControl Navigation { get; }
        public IVoiceCreationService VoiceCreationService { get; }
        public IChatCompletionService ChatCompletionService { get; }
        public ZundamonSpeakController ZundamonSpeakController { get; }
        public MessagePresentation MessagePresentation { get; }
        public View ViewContext { get; }

        public ViewCollection ViewCollection { get; }
        public BuiltinVoiceCollection VoiceCollection { get; }
        public IExternalServiceTestProvider TestProvider { get; }

        public List<string> TitleCanditates { get; set; }
        public string SelectedTitle { get; set; }

        public void ExternalServiceSetup();
    }
}