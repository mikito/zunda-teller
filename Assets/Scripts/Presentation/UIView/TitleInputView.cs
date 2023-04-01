using UnityEngine;
using UnityEngine.UI;
using System;

namespace ZundaTeller.Presentation
{
    public class TitleInputView : View
    {
        [SerializeField] Button backButton;
        [SerializeField] Button okButton;
        [SerializeField] InputField inputField;

        public Action OnBack;
        public Action OnOk;

        public string Text { get => inputField.text; set => inputField.text = value; }

        protected override void Awake()
        {
            base.Awake();
            okButton.AddExclusiveClickListener(() => OnOk?.Invoke());
            backButton.AddExclusiveClickListener(() => OnBack?.Invoke());
        }
    }
}