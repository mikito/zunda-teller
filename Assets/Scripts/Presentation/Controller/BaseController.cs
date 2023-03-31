using System;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ZundaTeller.Presentation
{
    public abstract class BaseController
    {
        protected IZundaContext context;
        protected INavigationControl navigation;
        protected View viewContext;
        protected ViewCollection views;
        protected BuiltinVoiceCollection voices;
        protected ZundamonSpeakController zundamonSpeakController;

        protected CancellationTokenSource cancellationTokenSource = null;
        protected View mainView = null;

        public void InjectContext(IZundaContext context)
        {
            this.context = context;
            this.navigation = context.Navigation;
            this.zundamonSpeakController = context.ZundamonSpeakController;
            this.viewContext = context.ViewContext;
            this.views = context.ViewCollection;
            this.voices = context.VoiceCollection;
        }

        protected T CreateAsMainView<T>(string prefabName = null) where T : View
        {
            if (mainView != null) mainView.Dismiss().Forget();
            T view = views.Create<T>(prefabName) as T;
            mainView = view;
            return view;
        }

        public virtual UniTask Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            return UniTask.CompletedTask;
        }

        public virtual async UniTask End()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            if (mainView != null) await mainView.Dismiss();
        }

        public virtual UniTask Resume()
        {
            return Start();
        }

        public virtual UniTask Supend()
        {
            return End();
        }
    }
}