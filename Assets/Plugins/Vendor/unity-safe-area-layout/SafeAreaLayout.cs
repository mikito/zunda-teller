using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UISupport
{
    [ExecuteInEditMode]
    public class SafeAreaLayout : UIBehaviour
    {
        Rect appliedRect;
        bool processing;

        protected override void Start()
        {
            base.Start();
            UpdateLayout();
        }

        void ApplySafeArea(Rect safeArea)
        {
            processing = true;

            var rectTransform = (RectTransform)transform;

            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;

            processing = false;
        }

        void UpdateLayout()
        {
            var safeArea = SafeAreaResolver.Shared.GetSafeArea();

            if (safeArea != appliedRect)
            {
                ApplySafeArea(safeArea);
                appliedRect = safeArea;
            }
        }

        override protected void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!processing) UpdateLayout();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = new Color32(0, 0, 255, 210);
            var rectTransform = (RectTransform)transform;
            var worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);
            Gizmos.DrawLine(worldCorners[0], worldCorners[1]);
            Gizmos.DrawLine(worldCorners[1], worldCorners[2]);
            Gizmos.DrawLine(worldCorners[2], worldCorners[3]);
            Gizmos.DrawLine(worldCorners[3], worldCorners[0]);
        }
#endif
    }

    public interface ISafeAreaResolver
    {
        Rect GetSafeArea();
    }

    public class SafeAreaResolver : ISafeAreaResolver
    {
        static ISafeAreaResolver sharedResolver;

        public static ISafeAreaResolver Shared
        {
            get
            {
                if (sharedResolver == null)
                {
#if UNITY_EDITOR
                    sharedResolver = new SafeAreaEmulator();
#else

                    sharedResolver = new SafeAreaResolver();
#endif
                }
                return sharedResolver;
            }
        }

        public Rect GetSafeArea()
        {
            return Screen.safeArea;
        }
    }

#if UNITY_EDITOR
    public class SafeAreaEmulator : ISafeAreaResolver
    {
        public class SafeAreaDefinition
        {
            public string name;
            public int width;
            public int height;
            public Rect safeArea;
        }

        static List<SafeAreaDefinition> safeAreas = new List<SafeAreaDefinition>()
        {
            new SafeAreaDefinition() {name = "X/XS Portrait", width = 1125, height = 2436, safeArea = new Rect(0, 102, 1125, 2202)},
            new SafeAreaDefinition() {name = "X/XS Landscape", width = 2436, height = 1125, safeArea = new Rect(132, 63, 2172, 1062)},

            new SafeAreaDefinition() {name = "XR Portrait", width = 828, height = 1792, safeArea = new Rect(0, 102, 828, 1558)},
            new SafeAreaDefinition() {name = "XR Landsccape", width = 1792, height = 828, safeArea = new Rect(132, 63, 1528, 763)},

            new SafeAreaDefinition() {name = "XsMax Portrait", width = 1242, height = 2688, safeArea = new Rect(0, 102, 1242, 2454)},
            new SafeAreaDefinition() {name = "XsMax Landsccape", width = 2688, height = 1242, safeArea = new Rect(132, 63, 2424, 1179)},

            new SafeAreaDefinition() {name = "iPadPro11 Portrait", width = 1668, height = 2388, safeArea = new Rect(0, 40, 1668, 2348)},
            new SafeAreaDefinition() {name = "iPadPro11 Landscape", width = 2388, height = 1668, safeArea = new Rect(0, 40, 2388, 1628)},

            new SafeAreaDefinition() {name = "iPadPro12.9(2018) Portrait", width = 2048, height = 2732, safeArea = new Rect(0, 40, 2048, 2692)},
            new SafeAreaDefinition() {name = "iPadPro12.9(2018) Landscape", width = 2732, height = 2048, safeArea = new Rect(0, 40, 2732, 2008)},
        };

        public Rect GetSafeArea()
        {
            // Use the actual safe area rect for the device simulator
            if(Screen.safeArea.width != Screen.width || Screen.safeArea.height != Screen.height)
            {
                return Screen.safeArea;
            }

            // for Game View
            var safeArea = safeAreas.Find(a => a.width == Screen.width && a.height == Screen.height);

            if (safeArea == null)
            {
                return new Rect(0, 0, Screen.width, Screen.height);
            }

            return safeArea.safeArea;
        }
    }
#endif
}