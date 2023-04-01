using System;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ZundaTeller.Presentation
{
    public class LicenseController : BaseController
    {
        public async override UniTask Start()
        {
            await base.Start();

            var view = CreateAsMainView<LicenseView>();
            view.OnBack = () => navigation.Pop();

            var texts = Resources.LoadAll<TextAsset>("Licenses").OrderBy(t => t.name);

            foreach (var t in texts)
            {
                view.AddElement(t.name.Substring(3), t.text);
            }

            zundamonSpeakController.SpeakAsync(voices.Find("zundamon_licence")).Forget();
            await viewContext.Present(view);
        }
    }
}