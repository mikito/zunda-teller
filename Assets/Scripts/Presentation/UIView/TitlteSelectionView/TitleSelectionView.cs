using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace ZundaTeller.Presentation
{
    public class TitleSelectionView : View
    {
        [SerializeField] TitleSelectionViewElement templete;
        [SerializeField] Button otherButton;
        [SerializeField] Button regenerateButton;
        [SerializeField] Button settingButton;

        public Action OnOther;
        public Action OnReGenerate;
        public Action OnSetting;

        protected override void Awake()
        {
            base.Awake();
            templete.gameObject.SetActive(false);
            otherButton.AddExclusiveClickListener(() => OnOther?.Invoke());
            regenerateButton.AddExclusiveClickListener(() => OnReGenerate?.Invoke());
            settingButton.AddExclusiveClickListener(() => OnSetting?.Invoke());
        }

        public void AddElement(string title, Action<string> onSelection)
        {
            var element = Instantiate(templete);
            element.transform.SetParent(templete.transform.parent, false);
            element.gameObject.SetActive(true);
            element.title.text = title;
            element.button.AddExclusiveClickListener(() =>
            {
                onSelection(title);
                Dismiss().Forget();
            });
        }
    }
}