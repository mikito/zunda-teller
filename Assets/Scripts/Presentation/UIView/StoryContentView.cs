using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using System.Collections;

namespace ZundaTeller.Presentation
{
    public class StoryContentView : View
    {
        [SerializeField] Text contents;
        [SerializeField] Text title;
        [SerializeField] Button button;

        public string Contents { set => contents.text = value; }
        public string Title { set => title.text = value; }
        public Action OnBackButtonClick;

        protected override void Awake()
        {
            base.Awake();
            button.AddExclusiveClickListener(() => OnBackButtonClick?.Invoke());
            Contents = "";
            Title = "";
        }

        public UniTask UpdateContentsWithAnimationAsync(string text)
        {
            return TypeWrite(text).ToUniTask(this);
        }

        IEnumerator TypeWrite(string text)
        {
            for (int i = 0; i <= text.Length; i++)
            {
                contents.text = text.Substring(0, i);
                yield return new WaitForSeconds(0.16f);
            }
        }
    }
}