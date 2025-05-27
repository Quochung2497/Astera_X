using UnityEngine;

namespace Course.Control
{
    public class TiltWithVelocity : ITiltWithVelocity
    {
        readonly Transform _transform;
        readonly float     _maxTiltAngle;  
        readonly float     _smoothSpeed;  
        readonly float     _maxSpeed;      
        readonly Quaternion _origRot;
        Quaternion         _targetRot;

        public TiltWithVelocity(Transform transform,
            float maxTiltAngle,
            float smoothSpeed,
            float maxSpeed)
        {
            _transform     = transform;
            _maxTiltAngle  = maxTiltAngle;
            _smoothSpeed   = smoothSpeed;
            _maxSpeed      = maxSpeed;
            _origRot       = transform.localRotation;
            _targetRot     = _origRot;
        }

        public void Tilt(Vector2 dir, float currentSpeed, float dt)
        {
            // Compute how “fast” ship speed are relative to the cap (0…1)
            var speedRatio = Mathf.Clamp01(currentSpeed / _maxSpeed);

            
            // Decide whether to bank or reset
            if (dir == Vector2.zero || speedRatio == 0f)
            {
                // no input → smoothly return to original
                _targetRot = _origRot;
            }
            else
            {
                // input present → compute a fresh bank/pitch
                var roll  = -dir.x * _maxTiltAngle * speedRatio;
                var pitch =  dir.y * _maxTiltAngle * speedRatio;
                _targetRot  = _origRot * Quaternion.Euler(pitch, roll, 0);
            }

            // Always slerp toward _targetRot
            _transform.localRotation = Quaternion.Slerp(
                _transform.localRotation,
                _targetRot,
                dt * _smoothSpeed
            );
        }
    }

}
