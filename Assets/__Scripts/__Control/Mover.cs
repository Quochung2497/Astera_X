using UnityEngine;

namespace Course.Control
{
    public class Mover : IMover
    {
        public void Move(Rigidbody rb, Vector2 direction, float speed)
        {
            // clamp the vector so diagonal >1 inputs get capped
            if (direction.sqrMagnitude  > 1f)
                direction = direction.normalized;
            rb.linearVelocity = new Vector3(direction.x, direction.y, 0f) * speed;
        }
    }
}
