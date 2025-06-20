using UnityEngine;

namespace Course.Control
{
    public interface IShooter
    {
        void Fire(Transform turret, Transform spawnPoint);
        void Reconfigure(float speed, float lifetime, float damage);
    }   
}
