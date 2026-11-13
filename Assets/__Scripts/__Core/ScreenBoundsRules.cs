using UnityEngine;

namespace Course.Core
{
    internal static class ScreenBoundsRules
    {
        private const float LocalEdge = 0.5f;
        private const float WrapEpsilon = 0.001f;

        public static bool IsOutside(Vector3 localPosition)
        {
            float maxDistance = Mathf.Max(
                Mathf.Abs(localPosition.x), Mathf.Abs(localPosition.y), Mathf.Abs(localPosition.z));
            return maxDistance > LocalEdge;
        }

        public static int GetAxisSide(float coordinate)
        {
            if (coordinate > LocalEdge) return 1;
            if (coordinate < -LocalEdge) return -1;
            return 0;
        }

        public static Vector3 Wrap(Vector3 localPosition)
        {
            localPosition.x = WrapAxis(localPosition.x);
            localPosition.y = WrapAxis(localPosition.y);
            return localPosition;
        }

        private static float WrapAxis(float coordinate)
        {
            return GetAxisSide(coordinate) switch
            {
                1 => -LocalEdge + WrapEpsilon,
                -1 => LocalEdge - WrapEpsilon,
                _ => coordinate
            };
        }
    }
}
