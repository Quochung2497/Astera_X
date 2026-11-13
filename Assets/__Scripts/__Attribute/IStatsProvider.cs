namespace Course.Attribute
{
    public interface IStatsProvider
    {
        void RegisterStat(Stats stats, float baseValue);
        void RegisterStatInstance(Stats stat, IAttribute instance);
        IAttribute Get(Stats stats);
        /// <summary> Sets MaxValue = BaseValue * (1 + percentBonus) </summary>
        void ApplyBonus(Stats stat, float percentBonus);
        /// <summary> Restores the selected stat's current and maximum values to its base value. </summary>
        void ResetToBase(Stats stat);
    }
}

