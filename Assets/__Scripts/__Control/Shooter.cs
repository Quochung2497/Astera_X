using System;
using Course.Attribute.Bullet;
using Course.Core;
using Course.ScriptableObject;
using UnityEngine;

namespace Course.Control
{
    public class Shooter: IShooter
    {
        /// <summary>
        /// Factory interface for creating and managing bullet instances.
        /// </summary>
        private readonly IFactory<Bullet> _bulletFactory;
        private float _speed, _lifetime, _damage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shooter"/> class.
        /// </summary>
        /// <param name="bulletFactory">The factory responsible for creating bullet instances.</param>
        public Shooter(IFactory<Bullet> bulletFactory,
            float speed,
            float lifetime,
            float damage)
        {
            _bulletFactory  = bulletFactory;
            _speed    = speed;
            _lifetime = lifetime;
            _damage   = damage;
        }
        
        public void Reconfigure(float speed, float lifetime, float damage)
        {
            _speed    = speed;
            _lifetime = lifetime;
            _damage   = damage;
        }

        /// <summary>
        /// Fires a bullet from the specified spawn point.
        /// Sets the bullet's position and rotation based on the spawn point and launches it in the turret's upward direction.
        /// </summary>
        /// <param name="turret">The transform of the turret used to determine the launch direction.</param>
        /// <param name="spawnPoint">The transform of the spawn point where the bullet is instantiated.</param>
        public void Fire(Transform turret, Transform spawnPoint)
        {
            // Retrieve a bullet instance from the factory.
            var bullet = _bulletFactory.Get();

            bullet.Initialize(_speed, _lifetime, _damage);
            
            // Set the bullet's position and rotation to match the spawn point.
            bullet.transform.position = spawnPoint.position;
            bullet.transform.rotation = spawnPoint.rotation;
            
            // Launch the bullet in the turret's upward direction.
            bullet.Launch(turret.up);
        }
    }   
}

