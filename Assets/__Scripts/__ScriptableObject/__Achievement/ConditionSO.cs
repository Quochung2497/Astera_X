using System;
using UnityEngine;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(menuName = "Achievements/ConditionSO Base")]
    public abstract class ConditionSO : UnityEngine.ScriptableObject
    {
        [Tooltip("When true, this condition’s Counter/MatchCount will be saved between sessions")]
        public bool PersistProgress = false;
        /// <summary>
        /// Fired when the condition is satisfied.
        /// </summary>
        public event Action OnConditionMet;
        
        /// <summary>Has this condition already fired?</summary>
        [NonSerialized] 
        private bool _hasMet;

        /// <summary> Call to mark this condition satisfied. </summary>
        public void RaiseMet()
        {
            if (_hasMet) return;     // ← guard so it only ever fires once
            _hasMet = true;
            OnConditionMet?.Invoke();
        }

        /// <summary> Used by your load routine to prime the already‐met flag. </summary>
        public void PrimeMet() => _hasMet = true;
    }
}

