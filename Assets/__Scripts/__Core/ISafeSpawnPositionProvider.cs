using UnityEngine;

namespace Course.Core
{
    public interface ISafeSpawnPositionProvider
    {
        Vector3 GetSafeSpawnPosition(
            float minDistAwayFromPlayer,
            float minDistAwayFromAsteroids,
            int maxAttempts);
    }
}
