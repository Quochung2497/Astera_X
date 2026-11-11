using System;
using UnityEngine;

namespace Course.Level
{
    [Serializable]
    public struct SpawnRule
    {
        [Header("––– Parent –––")]
        [Min(1)] public int parentSize;       // e.g. “3” for a size‐3 parent
        [Min(0)] public int parentCount;      // how many parents
        [Min(1)] public int parentHealth;
        [Min(0)] public int parentPoints;
        [Min(0)] public int parentDamage;

        [Space(6)]
        [Header("––– Child –––")]
        [Min(1)] public int childSize;        // fragment size for children
        [Min(0)] public int childCount;       // how many children per parent
        [Min(1)] public int childHealth;
        [Min(0)] public int childPoints;
        [Min(0)] public int childDamage;

        [Space(6)]
        [Header("––– Grandchild –––")]
        [Min(1)] public int grandchildSize;   // fragment size for grandchildren
        [Min(0)] public int grandchildCount;  // how many grandchildren per child
        [Min(1)] public int grandchildHealth;
        [Min(0)] public int grandchildPoints;
        [Min(0)] public int grandchildDamage;
    }
}
