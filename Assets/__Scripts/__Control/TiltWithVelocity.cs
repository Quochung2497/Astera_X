using UnityEngine;

namespace Course.Control
{
    public class TiltWithVelocity : ITiltWithVelocity
    {
        /// <summary>
        /// The transform of the object to be tilted.
        /// </summary>
        readonly Transform _transform;
        
        /// <summary>
        /// The maximum tilt angle applied to the object.
        /// </summary>
        readonly float _maxTiltAngle;  
        
        /// <summary>
        /// The speed at which the tilt transitions are smoothed.
        /// </summary>
        readonly float _smoothSpeed;  
        
        /// <summary>
        /// The maximum speed used to calculate the tilt ratio.
        /// </summary>
        readonly float _maxSpeed;      
        
        /// <summary>
        /// The original rotation of the object before any tilting.
        /// </summary>
        readonly Quaternion _origRot;
        
        /// <summary>
        /// The target rotation the object is transitioning towards.
        /// </summary>
        Quaternion _targetRot;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TiltWithVelocity"/> class.
        /// </summary>
        /// <param name="transform">The transform of the object to be tilted.</param>
        /// <param name="maxTiltAngle">The maximum tilt angle applied to the object.</param>
        /// <param name="smoothSpeed">The speed at which the tilt transitions are smoothed.</param>
        /// <param name="maxSpeed">The maximum speed used to calculate the tilt ratio.</param>
        public TiltWithVelocity(Transform transform,
            float maxTiltAngle,
            float smoothSpeed,
            float maxSpeed)
        {
            _transform = transform;
            _maxTiltAngle = maxTiltAngle;
            _smoothSpeed = smoothSpeed;
            _maxSpeed = maxSpeed;
            _origRot = transform.localRotation;
            _targetRot = _origRot;
        }
        
        /// <summary>
        /// Applies tilt to the object based on the direction, current speed, and delta time.
        /// </summary>
        /// <param name="dir">The direction vector indicating the tilt direction.</param>
        /// <param name="currentSpeed">The current speed of the object.</param>
        /// <param name="dt">The delta time used for smoothing the tilt transition.</param>
        public void Tilt(Vector2 dir, float currentSpeed, float dt)
        {
            // Compute how “fast” ship speed is relative to the cap (0…1)
            var speedRatio = Mathf.Clamp01(currentSpeed / _maxSpeed);
        
            // Decide whether to bank or reset
            if (dir == Vector2.zero || speedRatio == 0f)
            {
                // No input → smoothly return to original
                _targetRot = _origRot;
            }
            else
            {
                // Input present → compute a fresh bank/pitch
                var roll = -dir.x * _maxTiltAngle * speedRatio;
                var pitch = dir.y * _maxTiltAngle * speedRatio;
                _targetRot = _origRot * Quaternion.Euler(pitch, roll, 0);
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
