namespace Course.Attribute
{
    public interface IAsteroid
    {
        void InitAsteroid();

        void SetSize(int amount);
        
        void InitializeCluster(int clusterSize);
    }
}