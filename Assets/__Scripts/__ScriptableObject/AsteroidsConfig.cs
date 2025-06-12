using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(menuName = "Scriptable Objects/AsteroidsSO", fileName = "AsteroidsSO.asset")]
    public class AsteroidsConfig : UnityEngine.ScriptableObject
    {
        /// <summary>
        /// Minimum velocity for asteroid movement.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public float minVel { get; private set; } = 5;
    
        /// <summary>
        /// Maximum velocity for asteroid movement.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public float maxVel { get; private set; } = 10;
    
        /// <summary>
        /// Maximum angular velocity for asteroid rotation.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public float maxAngularVel { get; private set; } = 10;
    
        /// <summary>
        /// Scale factor for asteroid size.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public float asteroidScale { get; private set; } = 0.75f;
        
        /// <summary>
        /// Array of asteroid prefabs used for instantiation.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public GameObject[] asteroidPrefabs { get; private set; }

        [Header("Per-Level Spawn Rules")]
        [Tooltip("For each level, assign one LevelSpawnRule, which in turn contains an array of 'SpawnRule'.")]
        [SerializeField]
        private LevelSpawnRule[] levelSpawnRules;
        
        public bool TryGetSpawnRuleForLevel(int level, out SpawnRule outRule)
        {
            outRule = default;
            if (levelSpawnRules == null || levelSpawnRules.Length == 0)
                return false;

            // 1) Exact match?
            var exact = levelSpawnRules.FirstOrDefault(lsr => lsr.levelNumber == level);
            if (exact.levelNumber == level)
            {
                outRule = exact.rule;
                return true;
            }

            // 2) If level is beyond highest, bump the counts:
            var highest = levelSpawnRules.OrderBy(lsr => lsr.levelNumber).Last();
            if (level > highest.levelNumber)
            {
                outRule = highest.rule;   // copy
                outRule.parentCount     += 1;
                outRule.childCount      += 1;
                outRule.grandchildCount += 1;
                return true;
            }

            // 3) No rule
            return false;
        }
        
        public int GetGlobalMaxParentSize()
        {
            int maxSize = 1;
            if (levelSpawnRules != null)
            {
                foreach (var lvlRule in levelSpawnRules)
                {
                    if (lvlRule.rule.parentSize > maxSize)
                        maxSize = lvlRule.rule.parentSize;
                }
            }
            return Mathf.Max(1, maxSize);
        }
        
        private void OnValidate()
        {
            if (levelSpawnRules == null) return;
            for (int i = 0; i < levelSpawnRules.Length; i++)
            {
                levelSpawnRules[i].levelNumber = i;
            }
        }
        
        [Serializable]
        public struct LevelSpawnRule
        {
            public int levelNumber;  // the level index
            public SpawnRule rule;        // the array of spawn‐rules for that level
        }
    
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
}

