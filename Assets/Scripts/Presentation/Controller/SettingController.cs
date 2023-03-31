using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using ZundaTeller.Configuration;

namespace ZundaTeller.Presentation
{
    public class SettingController : BaseController
    {
        SettingView settingView;
        VoicevoxInregrationType voicevoxInregrationType;

        public async override UniTask Start()
        {
            await base.Start();

            settingView = CreateAsMainView<SettingView>();
            settingView.OpenAIAPIKeyText = Preferences.OpenAIAPIKey;
            settingView.VoicevoxUrlText = Preferences.VoicevoxEngineHost;
            settingView.VoicevoxWebAPIKeyText = Preferences.VoicevoxWebAPIKey;

            settingView.OnBack = () => navigation.Pop();
            settingView.OnCredit = OnCredit;
            settingView.OnOk = OnOK;

            voicevoxInregrationType = Preferences.VoicevoxInregrationType;
            settingView.SetVoicevoxInregrationType(voicevoxInregrationType, false);
            settingView.OnVoicevoxiIntegrationTypeChange = OnVoicevoxToggleChange;


            settingView.OnOpenAIInfo = () => Application.OpenURL("https://github.com/mikito/zunda-teller/wiki/OpenAI-API%E3%81%A8%E3%81%AE%E9%80%A3%E6%90%BA");
            settingView.OnVoicevoxInfo = () => Application.OpenURL("https://github.com/mikito/zunda-teller/wiki/VOICEVOX%E3%81%A8%E3%81%AE%E9%80%A3%E6%90%BA");

            settingView.BackButtonHidden = navigation.List.Count == 1;

            zundamonSpeakController.SpeakAsync(voices.Find("zundamon_setting"), Emotion.Normal).Forget();
            await viewContext.Present(settingView);
        }

        void OnVoicevoxToggleChange(VoicevoxInregrationType integrationType)
        {
            if (voicevoxInregrationType == integrationType) return;
            voicevoxInregrationType = integrationType;
            settingView.SetVoicevoxInregrationType(integrationType, true);
        }

        void OnOK()
        {
            if (string.IsNullOrEmpty(settingView.OpenAIAPIKeyText))
            {
                context.MessagePresentation.PresentOK("OpenAIの設定\nが空なのだ", "みなおす").Forget();
                return;
            }

            if (voicevoxInregrationType == VoicevoxInregrationType.VoicevoxWebAPI && string.IsNullOrEmpty(settingView.VoicevoxWebAPIKeyText))
            {
                context.MessagePresentation.PresentOK("VOICEVOXの設定が\nが空なのだ", "みなおす").Forget();
                return;
            }

            if (voicevoxInregrationType == VoicevoxInregrationType.LocalVoicevoxEngine && string.IsNullOrEmpty(settingView.VoicevoxUrlText))
            {
                context.MessagePresentation.PresentOK("VOICEVOXの設定が\nが空なのだ", "みなおす").Forget();
                return;
            }

            CheckAndSaveAsync().Forget();
        }

        async UniTask CheckAndSaveAsync()
        {
            // Request Test 
            var messageView = await context.MessagePresentation.PresentMessage("チェック中...");

            var testProvider = context.TestProvider;
            var testable = new List<ITestable>();
            testable.Add(testProvider.ProvideTestableForChatCompletion(settingView.OpenAIAPIKeyText));
            testable.Add(voicevoxInregrationType switch
            {
                VoicevoxInregrationType.VoicevoxWebAPI => testProvider.ProvideTestableForAPIVoiceCreation(settingView.VoicevoxWebAPIKeyText),
                VoicevoxInregrationType.LocalVoicevoxEngine => testProvider.ProvideTestableForLocalEngineVoiceCreation(settingView.VoicevoxUrlText),
                _ => throw new Exception("invalid voicevox integration type")
            });
            var testResults = await UniTask.WhenAll(testable.Select(t => t.TestAsync(cancellationTokenSource.Token)));

            await messageView.Dismiss();

            //  Present Error Message
            var openAITestResult = testResults[0];
            var voicevoxTestResult = testResults[1];

            if (!openAITestResult.isSuccess)
            {
                context.MessagePresentation.PresentOK("OpenAI API\nとの通信に失敗したのだ。\nもう一度設定を見直すのだ。", "わかった").Forget();
                return;
            }

            if (!voicevoxTestResult.isSuccess)
            {
                string message = null;

                var voiceCreationException = voicevoxTestResult.exception as VoiceCreationException;
                if (voiceCreationException != null && voiceCreationException.Code == VoiceCreationErrorCode.LimitationExceeded)
                {
                    message = "VOICEVOXサーバー\nの利用上限に達しているのだ。別の設定に変更するか、\n上限が復活するまでしばらく待つのだ。";
                }
                else
                {
                    message = "VOICEVOXサーバー\nとの通信に失敗したのだ。\nもう一度設定を見直すのだ。";
                }

                context.MessagePresentation.PresentOK(message, "わかった").Forget();
                return;
            }

            // Save if success
            Preferences.OpenAIAPIKey = settingView.OpenAIAPIKeyText;
            Preferences.VoicevoxInregrationType = voicevoxInregrationType;
            switch (voicevoxInregrationType)
            {
                case VoicevoxInregrationType.VoicevoxWebAPI: Preferences.VoicevoxWebAPIKey = settingView.VoicevoxWebAPIKeyText; break;
                case VoicevoxInregrationType.LocalVoicevoxEngine: Preferences.VoicevoxEngineHost = settingView.VoicevoxUrlText; break;
                default: throw new Exception("invalid voicevox integration type");
            };

            // Complete 
            Preferences.IsRemoteServiceChecked = true;

            context.ExternalServiceSetup();

            if (navigation.List.Count == 1)
            {
                navigation.Switch(new TitleSelectionController());
            }
            else
            {
                navigation.Pop();
            }
        }

        void OnCredit()
        {
            navigation.Push(new LicenseController());
        }
    }
}