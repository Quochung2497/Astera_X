using Course.Attribute;
using Course.Core;

namespace Course.Control.Player
{
    internal sealed class PlayerActionState : IPlayerActionState
    {
        private enum JumpPhase
        {
            Ready,
            Hidden,
            VisibleImmune
        }

        private readonly IHealthBehaviour _health;
        private readonly IGameStateReader _gameState;
        private JumpPhase _phase = JumpPhase.Ready;

        public PlayerActionState(IHealthBehaviour health, IGameStateReader gameState)
        {
            _health = health;
            _gameState = gameState;
        }

        public bool CanMove => !_health.IsDead && _phase != JumpPhase.Hidden;
        public bool CanAim => !_health.IsDead;
        public bool CanFire => CanMove && _gameState.CurrentState == GameState.level;
        public bool CanTakeDamage => !_health.IsDead && _phase == JumpPhase.Ready;

        public bool TryBeginJump()
        {
            if (!CanTakeDamage || _gameState.CurrentState != GameState.level)
                return false;

            _phase = JumpPhase.Hidden;
            return true;
        }

        public void Reappear() => _phase = JumpPhase.VisibleImmune;

        public void EndImmunity() => _phase = JumpPhase.Ready;
    }
}
