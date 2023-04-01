using UnityEngine;
using UnityEngine.UI;

namespace ZundaTeller.Presentation
{
    public class TitleCallView : View
    {
        [SerializeField] Text titleText;
        public string Title { set => titleText.text = value; }
    }
}