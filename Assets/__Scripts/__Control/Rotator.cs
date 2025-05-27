using UnityEngine;

namespace Course.Control
{
    public class Rotator : IRotator
    {
        private readonly float _angleOffset;
        private Vector3        _lastWorld;

        public Rotator(float angleOffset = -90f)
        {
            _angleOffset = angleOffset;
        }

        public void Aim(Transform target, Vector2 screenPos, Camera cam)
        {
            var zDist = target.position.z - cam.transform.position.z;
            _lastWorld = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDist));

            var dir2D = new Vector2(
                _lastWorld.x - target.position.x,
                _lastWorld.y - target.position.y
            );
            var raw   = Mathf.Atan2(dir2D.y, dir2D.x) * Mathf.Rad2Deg;
            var angle = raw + _angleOffset;

            target.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }    
}

