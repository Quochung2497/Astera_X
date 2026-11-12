namespace Course.Level
{
    internal interface ILevelProgression
    {
        int CurrentLevel { get; }
        SpawnRule? CurrentSpawnRule { get; }
        bool HasPendingClear { get; }

        void Initialize(int startLevel);
        void AdvanceLevel();
        void PrepareCurrentLevel();
        void OnAsteroidRemoved(int remainingAsteroids);
        bool ShouldClearAfterSpawn(int remainingAsteroids);
        bool TryResolveClear(GameState gameState, int remainingAsteroids);
    }
}
