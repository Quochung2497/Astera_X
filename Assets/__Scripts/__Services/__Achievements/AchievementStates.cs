using System;
using Course.ScriptableObject;

namespace Course.Services.Achievements
{
    public class AchievementStates
    {
        public AchievementSO Definition { get; }
        public bool          IsUnlocked { get; private set; }

        private readonly IConditionState _state;
        private readonly Action          _onUnlocked;

        public AchievementStates(
            AchievementSO def,
            bool alreadyUnlocked,
            Action onUnlocked
        ){
            Definition  = def;
            IsUnlocked  = alreadyUnlocked;
            _onUnlocked = onUnlocked;

            // pick the right ConditionState impl:
            if (def.Condition is GenericEventCountConditionSO gen)
                _state = new GenericEventCountConditionState(gen);
            else
            {
                // for correlated, grab the TKey at runtime via reflection:
                var baseType = def.Condition.GetType().BaseType;
                var keyType  = baseType.GetGenericArguments()[0];
                var csType   = typeof(CorrelatedEventConditionState<>)
                               .MakeGenericType(keyType);
                _state = (IConditionState)Activator.CreateInstance(
                    csType,
                    new object[]{ def.Condition }
                );
            }
        }

        /// <summary>
        /// Called by your manager *after* loading saved Counter value:
        /// </summary>
        public void LoadCounter(float savedValue)
        {
            if (!Definition.Condition.PersistProgress) return;
            _state.Counter = savedValue;
        }

        public void Initialize()
        {
            if (IsUnlocked) 
            {
                Definition.Condition.PrimeMet();
                return;
            }

            // wire the SO’s OnConditionMet so we know when to unlock
            Definition.Condition.OnConditionMet += HandleMet;

            // now hook up the event subscriptions & counting
            _state.Initialize();
        }

        public void Uninitialize()
        {
            // tears down subscriptions & captures final Counter
            _state.Uninitialize();
            Definition.Condition.OnConditionMet -= HandleMet;
        }

        private void HandleMet()
        {
            if (IsUnlocked) return;
            IsUnlocked = true;
            Uninitialize();
            _onUnlocked?.Invoke();
        }

        /// <summary>
        /// Before saving, snapshot the counter into your save DTO.
        /// </summary>
        public float GetCounter() => _state.Counter;

        public float GetRequiredCount() => _state.RequiredCount;
        
        public bool ShouldPersist => _state.ShouldPersist;
    }
}
