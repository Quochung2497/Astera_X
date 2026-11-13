using UnityEngine;

namespace Course.Core
{
    public interface IScreenBounds
    {
        Vector3 RANDOM_ON_SCREEN_LOC { get; }
        bool OOB(Vector3 worldPos);
    }
}
