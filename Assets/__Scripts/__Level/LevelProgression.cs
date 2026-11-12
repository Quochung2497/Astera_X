namespace Course.Level
{
    internal sealed class LevelProgression : ILevelProgression
    {
        private readonly ILevelSpawnRules _spawnRules;

        public int CurrentLevel { get; private set; }
        public SpawnRule? CurrentSpawnRule { get; private set; }
        public bool HasPendingClear { get; private set; }

        public LevelProgression(ILevelSpawnRules spawnRules)
        {
            _spawnRules = spawnRules;
        }

        public void Initialize(int startLevel)
        {
            CurrentLevel = startLevel;
            CurrentSpawnRule = null;
        }

        public void AdvanceLevel() => CurrentLevel++;

        public void PrepareCurrentLevel() => CurrentSpawnRule = _spawnRules.PrepareLevel(CurrentLevel);

        public void OnAsteroidRemoved(int remainingAsteroids)
        {
            if (remainingAsteroids == 0)
                HasPendingClear = true;
        }

        public bool ShouldClearAfterSpawn(int remainingAsteroids) => remainingAsteroids == 0;

        public bool TryResolveClear(GameState gameState, int remainingAsteroids)
        {
            if (!HasPendingClear || gameState is GameState.paused or GameState.shop)
                return false;

            HasPendingClear = false;
            return gameState == GameState.level && remainingAsteroids == 0;
        }
    }
}
