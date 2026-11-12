namespace Course.Core
{
    internal interface IGameStateReader
    {
        GameState CurrentState { get; }
    }
}
