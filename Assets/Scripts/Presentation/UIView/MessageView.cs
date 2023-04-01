using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

namespace ZundaTeller.Presentation
{
    public class MessageView : View
    {
        [SerializeField] Button okButtonTemplete;
        [SerializeField] Button cancelButtonTemplete;
        [SerializeField] Text message;

        protected override void Awake()
        {
            base.Awake();
            okButtonTemplete.gameObject.SetActive(false);
            cancelButtonTemplete.gameObject.SetActive(false);
        }

        public string Message { set => message.text = value; }

        void AddAction(string title, Action action, Button templete)
        {
            var button = Instantiate(templete);
            button.GetComponentInChildren<Text>().text = title;
            button.AddExclusiveClickListener(() =>
            {
                Dismiss().Forget();
                action?.Invoke();
            });
            button.transform.SetParent(okButtonTemplete.transform.parent, false);
            button.gameObject.SetActive(true);

        }

        public void AddActionAsOK(string title, Action action)
        {
            AddAction(title, action, okButtonTemplete);
        }

        public void AddActionAsCancel(string title, Action action)
        {
            AddAction(title, action, cancelButtonTemplete);
        }
    }
}