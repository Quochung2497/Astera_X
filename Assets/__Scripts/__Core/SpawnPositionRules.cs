using UnityEngine;

namespace Course.Core
{
    internal static class SpawnPositionRules
    {
        public static bool IsTooClose(Vector3 position, Vector3 obstacle, float minDistance) =>
            (position - obstacle).magnitude < minDistance;

        public static bool IsTooCloseSquared(Vector3 position, Vector3 obstacle, float minDistanceSquared) =>
            (position - obstacle).sqrMagnitude < minDistanceSquared;
    }
}
