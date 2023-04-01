using UnityEngine;

namespace ZundaTeller.Presentation
{
    [ExecuteInEditMode]
    public class ScreenSpaceLayoutAdjuster : MonoBehaviour
    {
        [SerializeField] float x;

        void Update()
        {
            var pos = Camera.main.ViewportToWorldPoint(new Vector3(x, 0, transform.position.z));
            transform.position = new Vector3(pos.x, transform.position.y, transform.position.z);
        }
    }
}
