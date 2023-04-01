using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ZundaTeller.Presentation
{
    public class MessagePresentation
    {
        ViewCollection views;

        public MessagePresentation(ViewCollection views)
        {
            this.views = views;
        }

        public async UniTask<MessageView> PresentMessage(string message)
        {
            var canvas = views.Create<View>("OverlayCanvas");
            var messageView = views.Create<MessageView>();
            messageView.Message = message;
            messageView.OnDismiss = () => canvas.Dismiss().Forget();
            await canvas.Present(messageView);
            return messageView;
        }

        public async UniTask<MessageView> PresentOK(string message, string okTitle = "はい", Action okClick = null)
        {
            var canvas = views.Create<View>("OverlayCanvas");
            var messageView = views.Create<MessageView>();
            messageView.Message = message;
            messageView.AddActionAsOK(okTitle, okClick);
            messageView.OnDismiss = () => canvas.Dismiss().Forget();
            await canvas.Present(messageView);
            return messageView;
        }

        public async UniTask<MessageView> PresentOKCancel(
            string message,
            string okTitle = "はい",
            string cancelTitle = "いいえ",
            Action okClick = null,
            Action cancelClick = null)
        {
            var canvas = views.Create<View>("OverlayCanvas");
            var messageView = views.Create<MessageView>();
            messageView.Message = message;
            messageView.AddActionAsOK(okTitle, okClick);
            messageView.AddActionAsCancel(okTitle, cancelClick);
            messageView.OnDismiss = () => canvas.Dismiss().Forget();
            await canvas.Present(messageView);
            return messageView;
        }
    }
}