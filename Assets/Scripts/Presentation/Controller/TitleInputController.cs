using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ZundaTeller.Presentation
{
    public class TitleInputController : BaseController
    {
        TitleInputView inputView;
        string inputText;

        public async override UniTask Start()
        {
            await base.Start();

            inputView = CreateAsMainView<TitleInputView>();
            inputView.OnBack = () => navigation.Pop();
            inputView.OnOk = OnOK;

            // Navigationによるコンテキストを保持している場合（Backしてきた場合）は表示する
            if (inputText != null)
            {
                inputView.Text = inputText;
            }

            zundamonSpeakController.SpeakAsync(voices.Find("zundamon_title_input"), Emotion.Normal).Forget();
            await viewContext.Present(inputView);
        }

        void OnOK()
        {
            if (string.IsNullOrEmpty(inputView.Text))
            {
                context.MessagePresentation.PresentOK("タイトルが空なのだ").Forget(); ;
                return;
            }

            inputText = inputView.Text;
            context.SelectedTitle = inputView.Text;

            navigation.Push(new StoryPlayController());
        }
    }
}