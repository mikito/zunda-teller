using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZundaTeller.Presentation
{
    [Serializable]
    public class ViewCollection
    {
        [SerializeField] List<View> views;

        public T Create<T>(string prefabName = null) where T : View
        {
            if (string.IsNullOrEmpty(prefabName)) prefabName = typeof(T).Name;
            var view = views.Find(v => v.gameObject.name == prefabName);
            return GameObject.Instantiate(view) as T;
        }
    }
}