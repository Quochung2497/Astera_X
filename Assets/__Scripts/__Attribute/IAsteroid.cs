namespace Course.Attribute
{
    public interface IAsteroid
    {
        int size { get; }
        void InitAsteroid();

        void SetSize(int amount);
    }
}