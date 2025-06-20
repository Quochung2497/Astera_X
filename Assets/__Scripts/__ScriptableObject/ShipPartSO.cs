using System;
using Course.Attribute;
using Course.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(fileName = "Ship Part", menuName = "Scriptable Objects/Ship Part")]
    public class ShipPartSO : UnityEngine.ScriptableObject
    {
        [field: SerializeField]
        public PartType partType { get; private set; }               // Body vs Turret
        [field: SerializeField]
        public int       Index { get; private set; }                  // unique within that PartType

        // [Tooltip("Which achievement must be unlocked to enable this part")]
        // [field: SerializeField]
        // public AchievementSO UnlockAchievement { get; private set; }

        [Header("Visuals")] [Header("Prefab to spawn")] 
        [SerializeField]
        private ShipPart Prefab;

        [field: SerializeField] 
        public Sprite      Icon { get; private set; }
        
        [field: SerializeField] 
        public string displayName { get; private set; }

        [Header("Stat modifiers")]
        [field: SerializeField]
        public StatModifier[] modifiers { get; private set; }

        public GameObject GetPrefab() => Prefab.gameObject; 
        
        [Serializable]
        public struct StatModifier
        {
            public Stats stats;          // e.g. Stats.Speed
            [Range(0,1)] public float percentBonus;  
        }
    }
}
