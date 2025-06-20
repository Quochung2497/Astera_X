using UnityEngine;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(menuName = "Achievements/Conditions/GenericEventCountCondition")]
    public class GenericEventCountConditionSO : ConditionSO
    {
        [Tooltip("Reflection‐driven channel that hooks EventBus<T> by type name")]
        public ReflectionGameEventChannelSO EventChannel;

        [Header("Count Mode (fires after N events)")]
        public float RequiredCount = 1;

        [Header("Threshold Mode (fires when a payload value ≥ threshold)")]
        public string PropertyName;

        [Tooltip("Choose how to evaluate this condition")]
        public ConditionMode Mode = ConditionMode.Count;
    }
    
    public enum ConditionMode { Count, Threshold }
}
