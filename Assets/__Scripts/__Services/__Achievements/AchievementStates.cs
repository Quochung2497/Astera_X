using System;
using Course.ScriptableObject;
using Course.Utility.Events;
using UnityEngine;

namespace Course.Services.Achievements
{
    public class AchievementStates
    {
        public AchievementSO Definition { get; }
        public bool IsUnlocked        { get; private set; }

        private Action _onUnlocked;

        public AchievementStates(AchievementSO def, bool alreadyUnlocked, Action onUnlocked)
        {
            Definition  = def;
            IsUnlocked  = alreadyUnlocked;
            _onUnlocked = onUnlocked;
        }

        public void Initialize()
        {
            if (IsUnlocked) return;
            Definition.Condition.OnConditionMet += HandleConditionMet;
            Definition.Condition.Initialize();
        }

        // public void Uninitialize()
        // {
        //     if (!IsUnlocked)
        //     {
        //         Definition.Condition.OnConditionMet -= HandleConditionMet;
        //         Definition.Condition.Uninitialize();
        //     }
        // }

        private void HandleConditionMet()
        {
            if (IsUnlocked) return;
            IsUnlocked = true;
            Definition.Condition.OnConditionMet -= HandleConditionMet;
            Definition.Condition.Uninitialize();
            _onUnlocked?.Invoke();
        }
    }
}

