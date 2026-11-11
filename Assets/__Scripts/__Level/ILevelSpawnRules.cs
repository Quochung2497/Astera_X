namespace Course.Level
{
    public interface ILevelSpawnRules
    {
        SpawnRule? PrepareLevel(int level);
    }
}
