using UnityEngine;

namespace Course.Control.Asteroid
{
    public interface IAsteroidBehaviour
    {
        void SpawnChildren(int sizeLevel,Transform parentTransform, string parentName);
        VelocityInfo CalculateInitialVelocity(Vector3 currentPosition, int size);
        void OnHit(Attribute.Asteroid self, Collision collision);
    }
}

