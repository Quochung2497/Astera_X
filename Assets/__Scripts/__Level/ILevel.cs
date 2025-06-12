using System;
using UnityEngine;

namespace Course.Level
{
    public interface ILevel
    {
        event Action OnPreLevel;      
        event Action OnLevelStarted;
        event Action OnLevelCleared;
        int CurrentLevel { get; }
        void Initialize(int level);
        void LevelCleared();
        void StartNextLevel();
    }
}
