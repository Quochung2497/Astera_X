using Course.Utility.Events;
using UnityEngine;

public class spriteScaler : MonoBehaviour
{
    private Camera cam;
    private EventBinding<ViewportChangedEvent> _viewportBinding;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        _viewportBinding ??= new EventBinding<ViewportChangedEvent>(OnViewportChanged);
        EventBus<ViewportChangedEvent>.Register(_viewportBinding);
        ScaleToScreenSize(gameObject);
    }

    private void OnDisable()
    {
        if (_viewportBinding != null)
            EventBus<ViewportChangedEvent>.Deregister(_viewportBinding);
    }

    private void OnViewportChanged(ViewportChangedEvent viewport)
    {
        if (viewport.Camera == cam) ScaleToScreenSize(gameObject);
    }

    public void ScaleToScreenSize(GameObject s)
    {

            var sr = s.GetComponent<SpriteRenderer>() ;
            if (sr == null) return;

            transform.localScale = Vector3.one;

            var width = sr.sprite.bounds.size.x;
            var height = sr.sprite.bounds.size.y;

            // vertical size in world units is simply twice the orthographicSize
            float worldScreenHeight = cam.orthographicSize * 2.0f;
            // horizontal size is scaled by the pixel aspect ratio
            float worldScreenWidth  = worldScreenHeight / Screen.height * Screen.width;

            // apply new scale
            transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
    }
}
