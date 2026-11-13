using UnityEngine;

namespace Course.Core
{
    internal static class ScreenBoundsRules
    {
        private const float LocalEdge = 0.5f;
        private const float WrapEpsilon = 0.001f;

        public static bool IsOutside(Vector3 localPosition) =>
            Mathf.Abs(localPosition.x) > LocalEdge ||
            Mathf.Abs(localPosition.y) > LocalEdge ||
            Mathf.Abs(localPosition.z) > LocalEdge;

        public static Vector3 Wrap(Vector3 localPosition)
        {
            localPosition.x = WrapAxis(localPosition.x);
            localPosition.y = WrapAxis(localPosition.y);
            return localPosition;
        }

        private static float WrapAxis(float coordinate)
        {
            if (coordinate > LocalEdge) return -LocalEdge + WrapEpsilon;
            if (coordinate < -LocalEdge) return LocalEdge - WrapEpsilon;
            return coordinate;
        }
    }
}
