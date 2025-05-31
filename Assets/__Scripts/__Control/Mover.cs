using UnityEngine;

namespace Course.Control
{
    public class Mover : IMover
    {
        private Rigidbody _rigidbody;

        #region IMover implementation
        /// <summary>
        /// Moves the Rigidbody in the specified direction at the given speed.
        /// Normalizes the direction vector if its magnitude exceeds 1 to ensure consistent movement.
        /// </summary>
        /// <param name="rb">The Rigidbody to move.</param>
        /// <param name="direction">The direction of movement as a 2D vector.</param>
        /// <param name="speed">The speed of movement.</param>
        public void Move(Rigidbody rb, Vector2 direction, float speed)
        {
            // clamp the vector so diagonal >1 inputs get capped
            if (direction.sqrMagnitude  > 1f)
                direction = direction.normalized;
            _rigidbody = rb;
            _rigidbody.linearVelocity = new Vector3(direction.x, direction.y, 0f) * speed;
        }
        
        /// <summary>
        /// Stops the Rigidbody by setting its linear and angular velocity to zero.
        /// </summary>
        public void Stop()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        #endregion
    }
}
