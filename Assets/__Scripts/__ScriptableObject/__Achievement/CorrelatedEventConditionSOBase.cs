using System;
using System.Collections.Generic;
using System.Reflection;
using Course.Utility.Events;
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
        public int RequiredCount = 1;


        private readonly HashSet<TKey> _seen = new();
        private Action<IEvent> _onFirst, _onSecond;
        private PropertyInfo _p1, _p2;
        private FieldInfo    _f1, _f2;
        private int _count;

        public override void Initialize()
        {
            _seen.Clear();
            _count = 0;

            BindAccessor(FirstChannel.EventTypeName,  out _p1, out _f1);
            BindAccessor(SecondChannel.EventTypeName, out _p2, out _f2);

            _onFirst = e => {
                var key = ReadKey(e, _p1, _f1);
                _seen.Add(key);
            };
            FirstChannel.Subscribe(_onFirst);

            _onSecond = e => {
                var key = ReadKey(e, _p2, _f2);
                if (_seen.Remove(key))
                    HandleMatch();
            };
            SecondChannel.Subscribe(_onSecond);
        }

        public override void Uninitialize()
        {
            FirstChannel.Unsubscribe(_onFirst);
            SecondChannel.Unsubscribe(_onSecond);
            _seen.Clear();
        }

        private void HandleMatch()
        {
            if (Mode == CorrelationMode.Single)
            {
                RaiseMet();
            }
            else // Count
            {
                if (++_count >= RequiredCount)
                    RaiseMet();
            }
        }

        private void BindAccessor(string typeName, out PropertyInfo pi, out FieldInfo fi)
        {
            var t = Type.GetType(typeName)
                    ?? throw new Exception($"Type '{typeName}' not found");
            pi = t.GetProperty(IdFieldName);
            fi = t.GetField(IdFieldName);
            if (pi == null && fi == null)
                throw new Exception($"No member '{IdFieldName}' on {t.Name}");
        }

        private TKey ReadKey(IEvent e, PropertyInfo pi, FieldInfo fi)
        {
            var raw = (pi != null)
                ? pi.GetValue(e)
                : fi.GetValue(e);
            return (TKey)raw;
        }
    }
    
    public enum CorrelationMode { Single, Count }
}
