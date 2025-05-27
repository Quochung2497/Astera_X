using Course.Attribute.Bullet;
using Course.Core;
using Course.ObjectPool;
using UnityEngine;
using UnityEngine.Pool;

namespace Course.Control
{
    public class Shooter: IShooter
    {
        private readonly IFactory<Bullet> _bulletFactory;

        public Shooter(IFactory<Bullet> bulletFactory)
        {
            _bulletFactory = bulletFactory;
        }
        
        public void Fire(Transform turret, Transform spawnPoint)
        {
            var bullet = _bulletFactory.Get();
            bullet.transform.position = spawnPoint.position;
            bullet.transform.rotation = spawnPoint.rotation;

            bullet.Launch(turret.up);
        }
    }   
}

