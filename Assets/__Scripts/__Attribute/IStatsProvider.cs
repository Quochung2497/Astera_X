using UnityEngine;

namespace Course.Attribute
{
    public interface IStatsProvider
    {
        void RegisterStat(Stats stats, float baseValue);
        void RegisterStatInstance(Stats stat, IAttribute instance);
        IAttribute Get(Stats stats);
        /// <summary> Sets MaxValue = BaseValue * (1 + percentBonus) </summary>
        void SetMaxValue(Stats stats, float percentBonus);

        void ResetAllToBase();
    }
}

