using Course.Utility;
using UnityEngine;

namespace Course.Core
{
    [RequireComponent(typeof(Collider))]
    public class OffScreenWrapper : MonoBehaviour, IOffScreenWrapper
    {
        private Collider _collider;
        const float _epsilon = 0.001f; // to avoid floating point errors

        
        // public void Wrap(Vector2 screenMin, Vector2 screenMax)
        // {
        //     var b      = _collider.bounds;
        //     var center = b.center;
        //     var ext    = b.extents;
        //
        //     // LEFT → just inside RIGHT
        //     if (b.max.x < screenMin.x)
        //         center.x = screenMax.x - ext.x - _epsilon;
        //
        //     // RIGHT → just inside LEFT
        //     else if (b.min.x > screenMax.x)
        //         center.x = screenMin.x + ext.x + _epsilon;
        //
        //     // BOTTOM → just inside TOP
        //     if (b.max.y < screenMin.y)
        //         center.y = screenMax.y - ext.y - _epsilon;
        //
        //     // TOP → just inside BOTTOM
        //     else if (b.min.y > screenMax.y)
        //         center.y = screenMin.y + ext.y + _epsilon;
        //
        //     transform.position = center;
        // }
        
        public void Wrap(ScreenBounds bounds)
        {
            var local = bounds.transform.InverseTransformPoint(transform.position);
        
            // If past +0.5 or –0.5, flip & nudge in by epsilon
            if (local.x >  0.5f) local.x = -0.5f + _epsilon;
            else if (local.x < -0.5f) local.x =  0.5f - _epsilon;
        
            if (local.y >  0.5f) local.y = -0.5f + _epsilon;
            else if (local.y < -0.5f) local.y =  0.5f - _epsilon;
        
            transform.position = bounds.transform
                .TransformPoint(local);
        }
        
        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }
    }
}

