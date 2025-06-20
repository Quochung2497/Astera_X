using System;
using System.Reflection;
using Course.ScriptableObject;
using Course.Utility.Events;
 
namespace Course.Services.Achievements
{
    public class GenericEventCountConditionState : IConditionState
    {
        private readonly GenericEventCountConditionSO _config;
        private float                                 _counter;
        private Action<IEvent>                        _handler;
        private PropertyInfo                          _prop;      // for Threshold mode

        public GenericEventCountConditionState(GenericEventCountConditionSO config)
        {
            _config = config;
        }

        public float Counter
        {
            get => _counter;
            set => _counter = value;
        }

        public float RequiredCount => _config.RequiredCount;

        public void PrimeMet()
        {
            _config.PrimeMet();
        }

        public void Initialize()
        {
            // start at either loaded value (in Counter) or zero
            _counter = Counter;

            // if threshold mode, grab the property
            if (_config.Mode == ConditionMode.Threshold)
            {
                var t = Type.GetType(_config.EventChannel.EventTypeName)
                        ?? throw new Exception($"Type '{_config.EventChannel.EventTypeName}' not found");
                _prop = t.GetProperty(_config.PropertyName)
                      ?? throw new Exception($"Property '{_config.PropertyName}' not found on {t.Name}");
            }

            // build unified handler
            _handler = e =>
            {
                if (_config.Mode == ConditionMode.Count)
                {
                    if (++_counter >= _config.RequiredCount)
                        _config.RaiseMet();
                }
                else
                {
                    var val = Convert.ToSingle(_prop.GetValue(e));
                    if (val >= _config.RequiredCount)
                        _config.RaiseMet();
                }
            };

            // subscribe
            _config.EventChannel.Subscribe(_handler);
        }

        public void Uninitialize()
        {
            if (_handler != null)
                _config.EventChannel.Unsubscribe(_handler);
            _handler = null;
            _prop    = null;

            // store back for Save
            Counter = _counter;
        }
        
        public bool ShouldPersist 
            => _config.PersistProgress && _config.Mode == ConditionMode.Count;
    }
}
