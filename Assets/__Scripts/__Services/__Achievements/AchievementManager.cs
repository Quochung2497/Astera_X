using System;
using System.Collections;
using System.Collections.Generic;
using Course.ScriptableObject;
using Course.Utility;
using Course.Utility.Events;
using UnityEngine;

namespace Course.Services.Achievements
{
    public class AchievementManager : PrivateSingleton<AchievementManager>
{
    [Header("Definitions")]
    [Tooltip("All your AchievementSO assets, in the order you want to show them.")]
    [SerializeField] private List<AchievementSO> _definitions;

    private List<AchievementStates>   _states;
    private Queue<AchievementStates>  _pending = new Queue<AchievementStates>();
    private bool _isShowing = false;
    
    
    public event Action<AchievementSO> OnShowAchievement;

    public void Acknowledge()
    {
        _isShowing = false;
        ProcessQueue();
    }
    
    protected override void Awake()
    {
        base.Awake();
        // 1) Load saved IDs
        HashSet<string> saved = LoadUnlockedIds();

        // 2) Wrap each SO in its runtime state
        _states = new List<AchievementStates>();
        foreach (var def in _definitions)
        {
            bool wasUnlocked = saved.Contains(def.Title);

            // Declare the variable up front
            AchievementStates state = null;

            // Now assign it—but we can safely capture it in the lambda
            state = new AchievementStates(
                def,
                wasUnlocked,
                onUnlocked: () => Enqueue(state)
            );

            _states.Add(state);
        }

        // 3) Initialize subscriptions
        foreach (var s in _states)
            s.Initialize();
    }


    private void Enqueue(AchievementStates state)
    {
        _pending.Enqueue(state);
        ProcessQueue();
        SaveUnlockedId(state.Definition.Title);
    }

    private void ProcessQueue()
    {
        if (_isShowing || _pending.Count == 0) return;

        _isShowing = true;
        var next = _pending.Dequeue();

        OnShowAchievement?.Invoke(next.Definition);
    }

    // ----- Persistence stubs -----

    private HashSet<string> LoadUnlockedIds()
    {
        // TODO: Implement your own load logic here.
        // Example (PlayerPrefs):
        // var json = PlayerPrefs.GetString("UnlockedAchievements", "{}");
        // return JsonUtility.FromJson<Wrapper>(json).ToHashSet();

        return new HashSet<string>();
    }

    private void SaveUnlockedId(string id)
    {
        // TODO: Add to your in-memory set and write back to disk.
        // Example:
        // _unlockedSet.Add(id);
        // var json = JsonUtility.ToJson(new Wrapper(_unlockedSet.ToList()));
        // PlayerPrefs.SetString("UnlockedAchievements", json);
    }
}
}

