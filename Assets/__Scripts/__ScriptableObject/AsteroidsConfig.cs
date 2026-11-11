using System;
using System.Linq;
using Course.Level;
using UnityEngine;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(menuName = "Scriptable Objects/AsteroidsSO", fileName = "AsteroidsSO.asset")]
    public class AsteroidsConfig : UnityEngine.ScriptableObject, ILevelSpawnRules
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
        [SerializeField] private GameObject[] asteroidPrefabs;
        
        [SerializeField] private GameObject[] asteroidMobilePrefabs;
        
        #if UNITY_EDITOR
        [SerializeField] private bool useMobilePrefabs = false; // for editor testing
        #endif

        [Header("Per-Level Spawn Rules")]
        [Tooltip("For each level, assign one LevelSpawnRule, which in turn contains an array of 'SpawnRule'.")]
        [SerializeField]
        private LevelSpawnRule[] levelSpawnRules;

        private LevelSpawnRule[] _activeRemoteRules;
        private LevelSpawnRule[] _pendingRemoteRules;

        private LevelSpawnRule[] ActiveSpawnRules => _activeRemoteRules ?? levelSpawnRules;

        public bool TryGetSpawnRuleForLevel(int level, out SpawnRule outRule)
        {
            outRule = default;
            var rules = ActiveSpawnRules;
            if (rules == null || rules.Length == 0)
                return false;

            // 1) Exact match?
            var exact = rules.FirstOrDefault(lsr => lsr.levelNumber == level);
            if (exact.levelNumber == level)
            {
                outRule = exact.rule;
                return true;
            }

            // 2) If level is beyond highest, bump the counts:
            var highest = rules.OrderBy(lsr => lsr.levelNumber).Last();
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
        
        public GameObject[] GetPrefabs()
        {
#if UNITY_EDITOR
            bool onMobile = useMobilePrefabs;
#else
            bool onMobile = Application.isMobilePlatform;
#endif
            return onMobile && asteroidMobilePrefabs != null && asteroidMobilePrefabs.Length > 0
                ? asteroidMobilePrefabs
                : asteroidPrefabs;
        }
        
        public int GetGlobalMaxParentSize()
        {
            int maxSize = 1;
            var rules = ActiveSpawnRules;
            if (rules != null)
            {
                foreach (var lvlRule in rules)
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
        
        internal void QueueRemoteSpawnRules(LevelSpawnRule[] rules) => _pendingRemoteRules = rules;

        public SpawnRule? PrepareLevel(int level)
        {
            if (_pendingRemoteRules != null)
            {
                _activeRemoteRules = _pendingRemoteRules;
                _pendingRemoteRules = null;
            }

            return TryGetSpawnRuleForLevel(level, out var rule) ? rule : (SpawnRule?)null;
        }

        internal void ResetRuntimeSpawnRules()
        {
            _activeRemoteRules = null;
            _pendingRemoteRules = null;
        }
        
        [Serializable]
        public struct LevelSpawnRule
        {
            public int levelNumber;  // the level index
            public SpawnRule rule;        // the array of spawn‐rules for that level
        }
    }
}

