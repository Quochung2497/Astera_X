using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Course.Attribute;
using Course.ScriptableObject;
using Course.Services.SaveService;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Services.Achievements
{
    public class AchievementManager : MonoBehaviour, IAchievementManager, ISavable
    {
        [Header("Definitions")]
        [Tooltip("All your AchievementSO assets, in the order you want to show them.")]
        [SerializeField] private List<AchievementSO> _definitions;
        
        // Private Fields
        private List<AchievementStates>   _states;
        private Queue<AchievementStates>  _pending = new Queue<AchievementStates>();
        private bool _isShowing = false;
        private bool _hasFiredHighScore = false;
        
        [Inject]
        private IScoreBehaviour _scoreBehaviour;

        #region IAchievementManager Implementation

        public event Action<AchievementSO> OnShowAchievement;
        public event Action<string, float> OnHighScore;
        public event Action<AchievementSO> OnUnlocked; 
        
        public float HighScore { get; private set; } = 5000f;
        
        public void SetHighScore(int candidate)
        {
            if (candidate > HighScore)
                HighScore = candidate;
        }
        
        public void Acknowledge()
        {
            _isShowing = false;
            ProcessQueue();
        }
        
        public bool DidHitNewHighThisSession => _hasFiredHighScore;
 
        #endregion

        #region ISavable Implementation

        object ISavable.GetDefault() => new AchievementProgressData();
        
        async Task ISavable.Load(ISaveService svc)
        {
            // 1) Load or default
            var defaultData = (AchievementProgressData)(this as ISavable).GetDefault();
            var data = await svc.LoadData("AchievementProgress", defaultData, this);

            HighScore = data.HighScore;
            
            if (data.UnlockedIds == null)
                data.UnlockedIds = new List<string>();

            // 2) Build lookup from the single list
            var counterDict = data.Counters.ToDictionary(e => e.key, e => e.value);

            // 3) Build states & inject saved
            _states = new List<AchievementStates>();
            foreach (var def in _definitions)
            {
                bool wasUnlocked = data.UnlockedIds.Contains(def.Title);
                AchievementStates state = null;
                state = new AchievementStates(def, wasUnlocked, () => Enqueue(state));

                if (state.ShouldPersist)
                {
                    if (counterDict.TryGetValue(def.Title, out var saved))
                        state.LoadCounter(saved);
                }

                // Debug.Log(
                //     $"'{def.Title}' → unlocked={wasUnlocked}, " +
                //     $"progress={state.GetCounter()}/{state.GetRequiredCount()}"
                // );

                _states.Add(state);
            }

            // 4) Hook them up
            foreach (var s in _states)
                s.Initialize();
            foreach (var s in _states.Where(s => s.IsUnlocked))
                OnUnlocked?.Invoke(s.Definition);
        }

        async Task ISavable.Save(ISaveService svc)
        {
            if (_states == null)
            {
                Debug.LogWarning("[AchievementManager] Save called before Load; skipping");
                return;
            }

            var data = new AchievementProgressData();

            // 1) unlock flags
            foreach (var s in _states)
                if (s.IsUnlocked)
                    data.UnlockedIds.Add(s.Definition.Title);

            // 2) all counters in one list
            foreach (var s in _states)
            {
                if (!s.ShouldPersist) 
                    continue;

                data.Counters.Add(new CounterEntry {
                    key   = s.Definition.Title,
                    value = s.GetCounter()
                });
            }
            // now write HighScore
            data.HighScore = HighScore;

            // 3) write it out
            try
            {
                await svc.SaveData("AchievementProgress", data, this);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AchievementManager] SaveData failed: {ex}");
            }
        }

        #endregion

        #region Private Methods

        private void Enqueue(AchievementStates state)
        {
            _pending.Enqueue(state);
            OnUnlocked?.Invoke(state.Definition);
            ProcessQueue();
        }
        
        private void ProcessQueue()
        {
            if (_isShowing || _pending.Count == 0) return;
        
            _isShowing = true;
            var next = _pending.Dequeue();
        
            OnShowAchievement?.Invoke(next.Definition);
        }
        
        private void HandleScoreChanged()
        {
            float current = _scoreBehaviour.Value;
            if (current > HighScore)
            {
                SetHighScore((int)current);

                if (!_hasFiredHighScore)
                {
                    _hasFiredHighScore = true;
                    OnHighScore?.Invoke("New High Score!", current);
                }
            }
        }
        
        private void OnEnable()
        {
            if (_scoreBehaviour == null) return;
            _scoreBehaviour.OnValueChanged += HandleScoreChanged;
        }

        private void OnDisable()
        {
            if (_scoreBehaviour == null) return;
            _scoreBehaviour.OnValueChanged -= HandleScoreChanged;
        }

        #endregion
    }
    
                
    [Serializable]
    public class AchievementProgressData
    {
        // which achievements are unlocked
        public List<string> UnlockedIds = new List<string>();
        public List<CounterEntry> Counters = new List<CounterEntry>();
        public float HighScore = 5000f;
    }
    
    [Serializable]
    public struct CounterEntry
    {
        public string key;
        public float  value;
    }
}

