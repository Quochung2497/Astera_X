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

    public void ScaleToScreenSize(GameObject target)
    {
        if (target == null || cam == null || !cam.orthographic) return;

        var renderer = target.GetComponent<SpriteRenderer>();
        if (renderer == null || renderer.sprite == null) return;

        var targetTransform = target.transform;
        var previousScale = targetTransform.localScale;
        targetTransform.localScale = Vector3.one;
        var baseSize = renderer.bounds.size;
        if (baseSize.x <= 0f || baseSize.y <= 0f)
        {
            targetTransform.localScale = previousScale;
            return;
        }

        var height = cam.orthographicSize * 2f;
        var width = height * cam.aspect;
        var scale = Mathf.Max(width / baseSize.x, height / baseSize.y);
        targetTransform.localScale = new Vector3(scale, scale, previousScale.z);

        var offset = cam.transform.position - renderer.bounds.center;
        var position = targetTransform.position;
        position.x += offset.x;
        position.y += offset.y;
        targetTransform.position = position;
    }
}
