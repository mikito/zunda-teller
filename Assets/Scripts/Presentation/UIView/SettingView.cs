using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using ZundaTeller.Configuration;

namespace ZundaTeller.Presentation
{
    public class SettingView : View
    {
        [SerializeField] Button backButton;
        [SerializeField] Button okButton;
        [SerializeField] Button creditButton;

        [SerializeField] Button openAIInfoButton;
        [SerializeField] Button voicevoxInfoButton;

        [SerializeField] InputField openAIAPIKeyField;
        [SerializeField] InputField voicevoxWebAPIKeyFiled;
        [SerializeField] InputField voicevoxURLField;

        [SerializeField] Toggle useVoicevoxWebAPIToggle;
        [SerializeField] Toggle useLocalVoicevoxEngineToggle;

        [SerializeField] Animator voicevoxWebAPISettingAnimator;
        [SerializeField] Animator voicevoxLocalVoicevoxSettingAnimator;

        public Action OnBack;
        public Action OnOk;
        public Action OnCredit;

        public Action OnOpenAIInfo;
        public Action OnVoicevoxInfo;

        public Action<VoicevoxInregrationType> OnVoicevoxiIntegrationTypeChange;

        public string OpenAIAPIKeyText { get => openAIAPIKeyField.text; set => openAIAPIKeyField.text = value; }
        public string VoicevoxUrlText { get => voicevoxURLField.text; set => voicevoxURLField.text = value; }
        public string VoicevoxWebAPIKeyText { get => voicevoxWebAPIKeyFiled.text; set => voicevoxWebAPIKeyFiled.text = value; }

        bool IsUseVoicevoxWebAPI
        {
            get => useVoicevoxWebAPIToggle.isOn;
            set => useVoicevoxWebAPIToggle.isOn = value;
        }

        bool IsUseLocalVoicevoxEngine
        {
            get => useLocalVoicevoxEngineToggle.isOn;
            set => useLocalVoicevoxEngineToggle.isOn = value;
        }

        public bool BackButtonHidden { set => backButton.gameObject.SetActive(!value); }

        protected override void Awake()
        {
            base.Awake();

            okButton.AddExclusiveClickListener(() => OnOk?.Invoke());
            backButton.AddExclusiveClickListener(() => OnBack?.Invoke());
            creditButton.AddExclusiveClickListener(() => OnCredit?.Invoke());

            openAIInfoButton.AddExclusiveClickListener(() => OnOpenAIInfo?.Invoke());
            voicevoxInfoButton.AddExclusiveClickListener(() => OnVoicevoxInfo?.Invoke());

            useVoicevoxWebAPIToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) OnVoicevoxiIntegrationTypeChange?.Invoke(VoicevoxInregrationType.VoicevoxWebAPI);
            });

            useLocalVoicevoxEngineToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) OnVoicevoxiIntegrationTypeChange?.Invoke(VoicevoxInregrationType.LocalVoicevoxEngine);
            });
        }

        public void SetVoicevoxInregrationType(VoicevoxInregrationType integrationType, bool animate)
        {
            IsUseVoicevoxWebAPI = integrationType == VoicevoxInregrationType.VoicevoxWebAPI;
            IsUseLocalVoicevoxEngine = integrationType == VoicevoxInregrationType.LocalVoicevoxEngine;
            ToggleVoicevoxWebAPISetting(integrationType != VoicevoxInregrationType.VoicevoxWebAPI, animate);
            ToggleVoicevoxLocalEngineSetting(integrationType != VoicevoxInregrationType.LocalVoicevoxEngine, animate);
        }

        void ToggleVoicevoxWebAPISetting(bool hidden, bool animate)
        {
            if (animate)
            {
                voicevoxWebAPISettingAnimator.gameObject.SetActive(true);
                voicevoxWebAPISettingAnimator.SetTrigger(hidden ? "hide" : "show");
                StartCoroutine(ChangeActiveDelay(voicevoxWebAPISettingAnimator.gameObject, 0.3f, hidden)); // 雑だけどこれで
            }
            else
            {
                voicevoxWebAPISettingAnimator.gameObject.SetActive(!hidden);
            }
        }

        void ToggleVoicevoxLocalEngineSetting(bool hidden, bool animate)
        {
            if (animate)
            {
                voicevoxLocalVoicevoxSettingAnimator.gameObject.SetActive(true);
                voicevoxLocalVoicevoxSettingAnimator.SetTrigger(hidden ? "hide" : "show");
                StartCoroutine(ChangeActiveDelay(voicevoxLocalVoicevoxSettingAnimator.gameObject, 0.3f, hidden)); // 雑だけどこれで
            }
            else
            {
                voicevoxLocalVoicevoxSettingAnimator.gameObject.SetActive(!hidden);
            }
        }

        IEnumerator ChangeActiveDelay(GameObject gameObject, float delay, bool hidden)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(!hidden);
        }
    }
}