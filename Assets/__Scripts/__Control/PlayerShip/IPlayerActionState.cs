namespace Course.Control.Player
{
    public interface IPlayerActionState
    {
        bool CanMove { get; }
        bool CanAim { get; }
        bool CanFire { get; }
    }
}
