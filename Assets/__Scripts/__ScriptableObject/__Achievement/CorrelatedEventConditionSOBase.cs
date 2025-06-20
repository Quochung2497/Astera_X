using UnityEngine;


namespace Course.ScriptableObject
{
    public abstract class CorrelatedEventConditionSOBase<TKey> : ConditionSO
    { 
        [Tooltip("First event channel (e.g. wrap)")]
        public ReflectionGameEventChannelSO FirstChannel;
        [Tooltip("Second event channel (e.g. hit)")]
        public ReflectionGameEventChannelSO SecondChannel;
        [Tooltip("Name of the field or property that links them (e.g. \"BulletId\").")]
        public string IdFieldName;
        
        [Header("Fires once or after N matches")]
        public CorrelationMode Mode = CorrelationMode.Single;
        [Tooltip("Number of wrap→hit correlations needed")]
        public float RequiredCount = 1;
    }
    
    public enum CorrelationMode { Single, Count }
}
