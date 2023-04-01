using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using ZundaTeller.AIGeneration;

namespace ZundaTeller.Presentation
{
    public class TitleSelectionController : BaseController
    {
        const int TitleGenerationCount = 4;

        public override async UniTask Start()
        {
            await base.Start();
            RunAsync().Forget();
        }

        async UniTask RunAsync()
        {
            if (context.TitleCanditates == null)
            {
                await TitleGenerationAsync();
            }

            await PresentTitles();
        }

        async UniTask LabelAnimation(UnityEngine.UI.Text text)
        {
            var token = text.GetCancellationTokenOnDestroy();
            int i = 0;
            while (text != null)
            {
                text.text = "考え中...".Substring(0, 3 + (i++ % 4));
                await UniTask.Delay(200, cancellationToken: token);
            }
        }

        async UniTask TitleGenerationAsync()
        {
            var generator = new StoryTitleGenerator(context.ChatCompletionService);
            var thinkingLabel = CreateAsMainView<View>("ThinkingPopupView");
            zundamonSpeakController.SpeakAsync(voices.Find("zundamon_thinking"), Emotion.Thinking).Forget();

            LabelAnimation(thinkingLabel.GetComponentInChildren<UnityEngine.UI.Text>()).Forget();

            await viewContext.PresentAsPopupAtWorldAnchor(thinkingLabel, zundamonSpeakController.View.PopupPosition);
            var storyTitles = await generator.GenerateAsync(TitleGenerationCount, cancellationTokenSource.Token);
            context.TitleCanditates = storyTitles.Select(s => s.title).ToList();
            await thinkingLabel.Dismiss();
        }

        async UniTask PresentTitles()
        {
            var selectionView = CreateAsMainView<TitleSelectionView>();
            selectionView.OnOther = OnOtherButton;
            selectionView.OnReGenerate = OnReGenerate;
            selectionView.OnSetting = OnSetting;

            foreach (var title in context.TitleCanditates)
            {
                selectionView.AddElement(title, OnTitleSelect);
            }

            zundamonSpeakController.SpeakAsync(voices.Find("zundamon_selection"), Emotion.Normal).Forget();
            await viewContext.Present(selectionView);
        }

        void OnTitleSelect(string selected)
        {
            context.SelectedTitle = selected;
            navigation.Push(new StoryPlayController());
        }

        void OnOtherButton()
        {
            navigation.Push(new TitleInputController());
        }

        void OnReGenerate()
        {
            context.TitleCanditates = null;
            UniTask.Create(async () =>
            {
                await mainView.Dismiss();
                RunAsync().Forget();
            });
        }

        void OnSetting()
        {
            navigation.Push(new SettingController());
        }
    }
}
