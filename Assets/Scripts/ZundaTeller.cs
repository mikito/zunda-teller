using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using ZundaTeller.Presentation;
using ZundaTeller.Navigation;
using ZundaTeller.AIGeneration;
using ZundaTeller.ExternalService;
using ZundaTeller.Configuration;

namespace ZundaTeller.Engine
{
    public class ZundaTeller : MonoBehaviour, IZundaContext
    {
        [SerializeField] View rootView;
        [SerializeField] ViewCollection viewCollection;
        [SerializeField] BuiltinVoiceCollection voiceCollection;
        [SerializeField] ZundamonView zundamonView;

        INavigationControl navigationControl;
        IChatCompletionService chatCompletionService;
        IVoiceCreationService voiceCreationService;
        ZundamonSpeakController zundamonController;
        MessagePresentation messagePresentation;
        IExternalServiceTestProvider testProvider = new ExternalServiceTestProvider();

        // Store context data simple
        List<string> titleCanditates = null;
        string selectedTitle = null;

        // IZundaContext
        public INavigationControl Navigation => navigationControl;
        public IChatCompletionService ChatCompletionService => chatCompletionService;
        public IVoiceCreationService VoiceCreationService => voiceCreationService;
        public ZundamonSpeakController ZundamonSpeakController => zundamonController;
        public IExternalServiceTestProvider TestProvider => testProvider;
        public MessagePresentation MessagePresentation => messagePresentation;
        public View ViewContext => rootView;
        public ViewCollection ViewCollection => viewCollection;
        public BuiltinVoiceCollection VoiceCollection => voiceCollection;
        public List<string> TitleCanditates { get => titleCanditates; set => titleCanditates = value; }
        public string SelectedTitle { get => selectedTitle; set => selectedTitle = value; }

        ErrorHandler errorHandler;

        public void ExternalServiceSetup()
        {
            chatCompletionService?.Dispose();
            chatCompletionService = new OpenAIChatCompletionService(Preferences.OpenAIAPIKey);

            voiceCreationService?.Dispose();

            voiceCreationService = Preferences.VoicevoxInregrationType switch
            {
                VoicevoxInregrationType.VoicevoxWebAPI => new VoicevoxWebAPIVoiceCreationService(Preferences.VoicevoxWebAPIKey),
                VoicevoxInregrationType.LocalVoicevoxEngine => new VoicevoxEngineVoiceCreationService(Preferences.VoicevoxEngineHost),
                _ => throw new Exception("invalid voicevox integration type")
            };
        }

        void Awake()
        {
            zundamonController = new ZundamonSpeakController(zundamonView);
            messagePresentation = new MessagePresentation(viewCollection);
            navigationControl = new NavigationController(this);
            errorHandler = new ErrorHandler(messagePresentation, navigationControl);

            UniTaskScheduler.UnobservedTaskException += errorHandler.OnError;
        }

        void Start()
        {
            if (!Preferences.IsRemoteServiceChecked)
            {
                navigationControl.Push(new SettingController());
            }
            else
            {
                ExternalServiceSetup();
                navigationControl.Push(new TitleSelectionController());
            }
        }

        void OnDestroy()
        {
            UniTaskScheduler.UnobservedTaskException -= errorHandler.OnError;
            navigationControl.Current?.End();
            chatCompletionService?.Dispose();
            voiceCreationService?.Dispose();
        }

        class ErrorHandler
        {
            bool errorDisplaying;
            MessagePresentation messagePresentation;
            INavigationControl navigation;

            public ErrorHandler(MessagePresentation messagePresentation, INavigationControl navigation)
            {
                this.messagePresentation = messagePresentation;
                this.navigation = navigation;
            }

            public void OnError(Exception exception)
            {
                Debug.LogError(exception);

                if (!errorDisplaying)
                {
                    errorDisplaying = true;
                    string message = "エラーが起きたのだ";

                    if (exception is AIGenerationException)
                    {
                        message = "ものがたりの生成でエラーが起きたのだ";
                    }
                    else if (exception is VoiceCreationException)
                    {
                        var ve = exception as VoiceCreationException;
                        message = ve.Code switch
                        {
                            VoiceCreationErrorCode.AuthenticationFailed => "ボイス生成で認証エラーが起きたのだ",
                            VoiceCreationErrorCode.ProcessingFailed => "ボイス生成処理でエラーが起きたのだ",
                            VoiceCreationErrorCode.LimitationExceeded => "ボイス生成の上限を超えたのだ",
                            VoiceCreationErrorCode.Unknown => "ボイス生成で不明なエラーが起きたのだ",
                            _ => "ボイス生成で不明なエラーが起きたのだ"
                        };
                    }

                    messagePresentation.PresentOK(message, "リスタート", () =>
                    {
                        Preferences.IsRemoteServiceChecked = false;
                        navigation.Restart();
                    }).Forget();
                }
            }
        }

        class ExternalServiceTestProvider : IExternalServiceTestProvider
        {
            public ITestable ProvideTestableForChatCompletion(string apiKey)
            {
                return new OpenAIChatCompletionService(apiKey);
            }

            public ITestable ProvideTestableForAPIVoiceCreation(string apiKey)
            {
                return new VoicevoxWebAPIVoiceCreationService(apiKey);
            }

            public ITestable ProvideTestableForLocalEngineVoiceCreation(string host)
            {
                return new VoicevoxEngineVoiceCreationService(host);
            }
        }
    }
}