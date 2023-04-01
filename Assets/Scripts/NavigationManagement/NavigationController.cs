using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ZundaTeller.Presentation;

namespace ZundaTeller.Navigation
{
    public class NavigationController : INavigationControl
    {
        IZundaContext context;

        public NavigationController(IZundaContext context)
        {
            this.context = context;
        }

        List<BaseController> list = new List<BaseController>();

        public BaseController Current
        {
            get
            {
                if (list.Count == 0) return null;
                return list[list.Count - 1];
            }
        }

        public IReadOnlyList<BaseController> List => list;

        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ZundaTeller");
        }

        public void Push(BaseController controller)
        {
            PushAsync(controller).Forget();
        }

        public void Switch(BaseController controller)
        {
            SwitchAsync(controller).Forget();
        }

        public void Pop()
        {
            PopAsync().Forget();
        }

        async UniTask PushAsync(BaseController controller)
        {
            if (Current != null)
            {
                await Current.Supend();
            }
            controller.InjectContext(context);
            list.Add(controller);
            await Current.Start();
        }

        async UniTask PopAsync()
        {
            if (Current == null) return;

            await Current.End();
            list.RemoveAt(list.Count - 1);
            await Current.Resume();
        }

        async UniTask SwitchAsync(BaseController controller)
        {
            if (Current == null)
            {
                await PushAsync(controller);
                return;
            }

            await Current.End();
            list.RemoveAt(list.Count - 1);

            controller.InjectContext(context);
            list.Add(controller);
            await Current.Start();
        }
    }
}