using UnityEngine;

namespace Course.Control
{
    public interface IMover
    {
        void Move(Rigidbody rb, Vector2 direction, float speed);

        void Stop();
    }
}
