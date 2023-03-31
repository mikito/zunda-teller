using Cysharp.Threading.Tasks;
using ZundaTeller.AIGeneration;
using UnityEngine;

namespace ZundaTeller.Presentation
{
    public class StoryPlayController : BaseController
    {
        const int SpeakerZundamonAmaAma = 1;

        PlayableStoryGeneration storyGeneration = null;

        public override async UniTask Start()
        {
            await base.Start();

            // バッググラウンドでのストーリーとボイス生成の逐次生成開始
            var generator = new StoryContentsGenerator(context.ChatCompletionService);
            storyGeneration = new PlayableStoryGeneration(generator, context.VoiceCreationService, context.SelectedTitle);
            storyGeneration.RunAsync().Forget();

            // 自動スリープを無効にする
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            RunAsync().Forget();
        }

        async UniTask RunAsync()
        {
            await TitleCallAsync(context.SelectedTitle);
            await UniTask.WaitUntil(() => storyGeneration.SentenceQueue.Count > 0);
            await PlayContentsAsync();
            OnStoryEnd();
        }

        async UniTask TitleCallAsync(string title)
        {
            using var voice = await context.VoiceCreationService.CreateVoiceAsync(title, cancellationTokenSource.Token);
            var titleCallView = views.Create<TitleCallView>();
            mainView = titleCallView;
            titleCallView.Title = title;
            await viewContext.Present(titleCallView);
            await UniTask.Delay(500);
            await zundamonSpeakController.SpeakAsync(voice.audioClip, Emotion.Normal, cancellationTokenSource.Token);
            await UniTask.Delay(1500);
            await titleCallView.Dismiss();
        }

        async UniTask PlayContentsAsync()
        {
            var contentView = CreateAsMainView<StoryContentView>();
            contentView.OnBackButtonClick = OnBackButtonClick;
            contentView.Title = context.SelectedTitle;

            await viewContext.Present(contentView);

            // Queueがなくなるまで再生
            while (true)
            {
                await UniTask.WaitUntil(() => storyGeneration.SentenceQueue.Count > 0);
                using var content = storyGeneration.SentenceQueue.Dequeue();

                await UniTask.WhenAll(
                    contentView.UpdateContentsWithAnimationAsync(content.sentence),
                    zundamonSpeakController.SpeakAsync(content.voice.audioClip, content.emotion, cancellationTokenSource.Token)
                );

                await UniTask.Delay(1200, cancellationToken: cancellationTokenSource.Token);

                if (storyGeneration.IsCompleted && storyGeneration.SentenceQueue.Count == 0) break;
            }

            // Ending
            await UniTask.WhenAll(
                contentView.UpdateContentsWithAnimationAsync("おしまい"),
                zundamonSpeakController.SpeakAsync(voices.Find("zundamon_ending"), Emotion.Normal, cancellationTokenSource.Token)
            );
            await UniTask.Delay(2000, cancellationToken: cancellationTokenSource.Token);
        }

        void OnBackButtonClick()
        {
            navigation.Pop();
        }

        void OnStoryEnd()
        {
            context.TitleCanditates = null;
            navigation.Pop();
        }

        public override async UniTask End()
        {
            storyGeneration?.Dispose();
            context.SelectedTitle = null;

            // 自動スリープを有効にする
            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            await base.End();
        }
    }
}