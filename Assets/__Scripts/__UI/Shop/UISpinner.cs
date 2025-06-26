using UnityEngine;

namespace Course.UI.Shop
{
    public class UISpinner : MonoBehaviour
    {
        [SerializeField] private float speed = 180f;
        void Update()
        {
            transform.Rotate(0,0,speed * Time.unscaledDeltaTime);
        }
    }
}

