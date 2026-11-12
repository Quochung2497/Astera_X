namespace Course.Core
{
    internal sealed class GameStateModel : IGameStateReader
    {
        public GameState CurrentState { get; private set; } = GameState.none;

        public bool ShouldFreezeSimulation =>
            CurrentState is GameState.mainMenu or GameState.paused or GameState.shop;

        public GameState? GetPauseTarget() => GetToggleTarget(GameState.paused);

        public GameState? GetShopTarget() => GetToggleTarget(GameState.shop);

        public void SetState(GameState state) => CurrentState = state;

        private GameState? GetToggleTarget(GameState overlayState)
        {
            if (CurrentState == GameState.level)
                return overlayState;

            return CurrentState == overlayState ? GameState.level : null;
        }
    }
}
