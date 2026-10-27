using UnityEngine;

namespace Course.Attribute
{
    public interface IAsteroid
    {
        int size { get; }
        int damage { get; }
        Transform transform { get; }
        GameObject gameObject { get; }

        void InitAsteroid();
        void SetSize(int amount);
        void SetPointValue(int pts);
        void SetDamageValue(int amount);
        void Setup(int hp);

        void SpawnDescendants(
            int childSize,
            int childCount,
            int childHealth,
            int childPoints,
            int childDamage,
            int grandchildSize,
            int grandchildCount,
            int grandchildHealth,
            int grandchildPoints,
            int grandchildDamage);

        bool TryHitByBullet(float amount);
        bool TryDestroyFromPlayerCollision();
    }
}