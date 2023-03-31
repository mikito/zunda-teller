using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.EventSystems;

namespace ZundaTeller.Presentation
{
    public static class ButtonEventExtension
    {
        static int lastClickFrameCount;

        // 同時押し対策
        public static void AddExclusiveClickListener(this Button button, Action onClick)
        {
            button.onClick.AddListener(() =>
            {
                int count = Time.frameCount;
                if (lastClickFrameCount == count) return;
                lastClickFrameCount = count;
                onClick?.Invoke();
            });
        }
    }

    [RequireComponent(typeof(PlayableDirector))]
    public class View : MonoBehaviour
    {
        PlayableDirector director = null;

        [SerializeField] PlayableAsset appearAnimation;
        [SerializeField] PlayableAsset disappearAnimation;

        public Action OnPresent;
        public Action OnDismiss;

        protected virtual void Awake()
        {
            director = GetComponent<PlayableDirector>();
        }

        public virtual async UniTask AnimateAppearAsync()
        {
            director.playableAsset = appearAnimation;
            director.Play();
            await UniTask.WaitWhile(() => director.state == PlayState.Playing, cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        public virtual async UniTask AnimateDisappearAsync()
        {
            director.playableAsset = disappearAnimation;
            director.Play();
            await UniTask.WaitWhile(() => director.state == PlayState.Playing, cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        // 単に子に追加して表示を行う簡易なViewプレゼンテーション
        public async UniTask Present(View view)
        {
            view.transform.SetParent(transform, false);
            view.UserInteraction = false;
            await view.AnimateAppearAsync();
            view.UserInteraction = true;
            OnPresent?.Invoke();
        }

        // 現在のCanvasとカメラ構成だけで有効な簡易コード
        public async UniTask PresentAsPopupAtWorldAnchor(View view, Transform anchor)
        {
            view.transform.SetParent(transform, false);
            view.transform.position = new Vector3(anchor.position.x, anchor.position.y, transform.position.z);
            view.UserInteraction = false;
            await view.AnimateAppearAsync();
            view.UserInteraction = true;
        }

        public async UniTask Dismiss()
        {
            UserInteraction = false;
            await AnimateDisappearAsync();
            UserInteraction = true;
            OnDismiss?.Invoke();
            Destroy(this.gameObject);
        }

        RectTransform raycastConsume = null;

        public bool UserInteraction
        {
            set
            {
                if (raycastConsume == null)
                {
                    var obj = new GameObject("RaycastConsume");
                    obj.transform.SetParent(transform, false);
                    var image = obj.AddComponent<Image>();
                    image.sprite = null;
                    image.color = Color.clear;
                    obj.AddComponent<EventTrigger>();
                    raycastConsume = obj.GetComponent<RectTransform>();
                    raycastConsume.anchorMin = new Vector2(0, 0);
                    raycastConsume.anchorMax = new Vector2(1, 1);
                    raycastConsume.sizeDelta = new Vector2(0, 0);
                }
                raycastConsume.SetAsLastSibling();
                raycastConsume.gameObject.SetActive(!value);
            }
            get
            {
                return (raycastConsume == null || !raycastConsume.gameObject.activeSelf);
            }
        }
    }
}