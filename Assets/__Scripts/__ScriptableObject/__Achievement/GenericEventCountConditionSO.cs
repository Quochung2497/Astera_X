using System;
using System.Reflection;
using Course.Utility.Events;
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

        private float          _counter;
        private Action<IEvent> _handler;
        private PropertyInfo   _prop;

        public override void Initialize()
        {
            _counter = 0;

            if (Mode == ConditionMode.Threshold)
            {
                // Use reflection to grab the named property on the event type
                var evtType = Type.GetType(EventChannel.EventTypeName)
                           ?? throw new Exception($"Event type '{EventChannel.EventTypeName}' not found");
                _prop = evtType.GetProperty(PropertyName)
                     ?? throw new Exception($"Property '{PropertyName}' not found on {evtType.Name}");
            }

            // Build the unified handler
            _handler = e =>
            {
                if (Mode == ConditionMode.Count)
                {
                    if (++_counter >= RequiredCount)
                        RaiseMet();
                }
                else // Threshold
                {
                    // Reflectively read the numeric property
                    var valObj = _prop.GetValue(e);
                    var val    = Convert.ToSingle(valObj);

                    if (val >= RequiredCount)
                        RaiseMet();
                }
            };

            EventChannel.Subscribe(_handler);
        }

        public override void Uninitialize()
        {
            EventChannel.Unsubscribe(_handler);
            _handler = null;
            _prop    = null;
        }
    }
    
    public enum ConditionMode { Count, Threshold }
}
