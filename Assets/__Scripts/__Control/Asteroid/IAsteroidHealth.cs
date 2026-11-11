namespace Course.Control.Asteroid
{
    public interface IAsteroidHealth
    {
        void ResetHealth(int hp);
        void SetPointValue(int points);
        (bool Accepted, bool Died, int? AwardedPoints) HitByBullet(float damage);
        (bool Accepted, bool Died, int? AwardedPoints) HitByPlayer();
    }
}
