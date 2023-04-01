using UnityEngine;
using UnityEngine.UI;
using System;

namespace ZundaTeller.Presentation
{
    public class LicenseViewElement : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Text body;

        public string Title { set => title.text = value; }
        public string Body { set => body.text = value; }
    }
}