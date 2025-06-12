using UnityEngine;

namespace Course.Control.Asteroid
{
    public interface IAsteroidBehaviour
    {
        void SpawnDescendent(
            int childSize,
            int childCount,
            int childHealth,
            int childPoints,
            int childDamage,

            int grandchildSize,
            int grandchildCount,
            int grandchildHealth,
            int grandchildPoints,
            int grandchildDamage,

            Transform parentTransform,
            string parentName);
        VelocityInfo CalculateInitialVelocity(Vector3 currentPosition, int size);
        void OnHit(Attribute.Asteroid self, Collision collision);
    }
}

