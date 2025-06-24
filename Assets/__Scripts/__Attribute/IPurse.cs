namespace Course.Services
{
    public interface IPurse
    {
        bool CanAfford(int amount);
        bool Spend(int amount);
    }
}