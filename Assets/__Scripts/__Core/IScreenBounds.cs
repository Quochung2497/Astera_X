using UnityEngine;

namespace Course.Core
{
    public interface IScreenBounds
    {
        Vector3 RANDOM_ON_SCREEN_LOC { get; }
        Bounds BOUNDS { get; }
        bool OOB(Vector3 worldPos);
        int OOB_X(Vector3 worldPos);
        int OOB_Y(Vector3 worldPos);
        int OOB_Z(Vector3 worldPos);
        int OOB_(float locPos);
    }
}
