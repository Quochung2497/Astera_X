using UnityEngine;

namespace Course.Control
{
    public class Rotator : IRotator
    {
        /// <summary>
        /// The angle offset applied to the calculated rotation angle.
        /// Used to adjust the orientation of the target.
        /// </summary>
        private readonly float _angleOffset;

        /// <summary>
        /// Stores the last calculated world position based on the screen position.
        /// </summary>
        private Vector3 _lastWorld;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rotator"/> class.
        /// </summary>
        /// <param name="angleOffset">The angle offset to apply to the rotation. Default is -90 degrees.</param>
        public Rotator(float angleOffset = -90f)
        {
            _angleOffset = angleOffset;
        }

        /// <summary>
        /// Aims the target transform towards a position derived from the screen position.
        /// Calculates the rotation angle based on the screen position and applies the angle offset.
        /// </summary>
        /// <param name="target">The transform of the object to rotate.</param>
        /// <param name="screenPos">The screen position to aim towards.</param>
        /// <param name="cam">The camera used to convert the screen position to a world position.</param>
        public void Aim(Transform target, Vector2 screenPos, Camera cam)
        {
            // Calculate the distance from the camera to the target along the z-axis.
            var zDist = target.position.z - cam.transform.position.z;

            // Convert the screen position to a world position using the camera.
            _lastWorld = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDist));

            // Calculate the 2D direction vector from the target to the world position.
            var dir2D = new Vector2(
                _lastWorld.x - target.position.x,
                _lastWorld.y - target.position.y
            );

            // Calculate the raw angle in degrees using the direction vector.
            var raw = Mathf.Atan2(dir2D.y, dir2D.x) * Mathf.Rad2Deg;

            // Apply the angle offset to the calculated angle.
            var angle = raw + _angleOffset;

            // Set the rotation of the target transform based on the calculated angle.
            target.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }    
}

