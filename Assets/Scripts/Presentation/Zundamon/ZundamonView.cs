using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZundaTeller.Presentation
{
    public class ZundamonView : MonoBehaviour
    {
        [SerializeField] Animator animator = null;
        [SerializeField] Transform popupPosition = null;
        [SerializeField] AudioSource audioSource = null;

        public bool Speak { set => animator.SetBool("speak", value); }
        public Emotion Emotion { set => animator.SetTrigger($"Emotion.{value.ToString()}"); }
        public Transform PopupPosition => popupPosition;
        public AudioSource AudioSource => audioSource;
    }
}