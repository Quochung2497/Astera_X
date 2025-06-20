using System;
using System.Collections.Generic;
using System.Reflection;
using Course.ScriptableObject;
using Course.Utility.Events;
 
namespace Course.Services.Achievements
{
    public class CorrelatedEventConditionState<TKey> : IConditionState
    {
        private readonly CorrelatedEventConditionSOBase<TKey> _config;
        private readonly HashSet<TKey>                       _seen = new();
        private float                                         _count;
        private Action<IEvent>                                _onFirst, _onSecond;
        private PropertyInfo                                  _p1, _p2;
        private FieldInfo                                     _f1, _f2;

        public CorrelatedEventConditionState(CorrelatedEventConditionSOBase<TKey> config)
        {
            _config = config;
        }

        public float Counter
        {
            get => _count;
            set => _count = value;
        }

        public float RequiredCount => _config.RequiredCount;

        public void PrimeMet()
        {
            _config.PrimeMet();
        }

        public void Initialize()
        {
            // restore loaded count or start 0
            _count = Counter;
            _seen.Clear();

            // bind accessors once
            var t1 = Type.GetType(_config.FirstChannel.EventTypeName)
                     ?? throw new Exception($"Type '{_config.FirstChannel.EventTypeName}' not found");
            _p1 = t1.GetProperty(_config.IdFieldName);
            _f1 = t1.GetField    (_config.IdFieldName);
            var t2 = Type.GetType(_config.SecondChannel.EventTypeName)
                     ?? throw new Exception($"Type '{_config.SecondChannel.EventTypeName}' not found");
            _p2 = t2.GetProperty(_config.IdFieldName);
            _f2 = t2.GetField    (_config.IdFieldName);

            _onFirst = e => {
                var key = _p1 != null
                          ? (TKey)_p1.GetValue(e)
                          : (TKey)_f1.GetValue(e);
                _seen.Add(key);
            };
            _config.FirstChannel.Subscribe(_onFirst);

            _onSecond = e => {
                var key = _p2 != null
                          ? (TKey)_p2.GetValue(e)
                          : (TKey)_f2.GetValue(e);
                if (_seen.Remove(key))
                {
                    if (_config.Mode == CorrelationMode.Single)
                        _config.RaiseMet();
                    else if (++_count >= _config.RequiredCount)
                        _config.RaiseMet();
                }
            };
            _config.SecondChannel.Subscribe(_onSecond);
        }

        public void Uninitialize()
        {
            // unsubscribe both
            if (_onFirst  != null) _config.FirstChannel.Unsubscribe (_onFirst);
            if (_onSecond != null) _config.SecondChannel.Unsubscribe(_onSecond);

            _onFirst  = null;
            _onSecond = null;

            // store back for Save
            Counter = _count;
        }
        
        public bool ShouldPersist 
            => _config.PersistProgress && _config.Mode == CorrelationMode.Count;
    }
}
