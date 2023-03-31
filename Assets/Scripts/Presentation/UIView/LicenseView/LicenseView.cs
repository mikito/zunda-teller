using UnityEngine;
using UnityEngine.UI;
using System;

namespace ZundaTeller.Presentation
{
    public class LicenseView : View
    {
        [SerializeField] Button backButton;
        [SerializeField] LicenseViewElement templete;

        public Action OnBack;

        protected override void Awake()
        {
            base.Awake();
            templete.gameObject.SetActive(false);
            backButton.AddExclusiveClickListener(() => OnBack?.Invoke());
        }

        public void AddElement(string title, string body)
        {
            var element = Instantiate(templete);
            element.transform.SetParent(templete.transform.parent, false);
            element.gameObject.SetActive(true);
            element.Title = title;
            element.Body = body;
        }
    }
}