using System.Collections.Generic;

namespace Course.Attribute
{
    internal sealed class StatsProvider : IStatsProvider
    {
        private readonly Dictionary<Stats, IAttribute> _attrs = new();

        public void RegisterStat(Stats stat, float baseValue)
        {
            if (_attrs.ContainsKey(stat)) return;

            _attrs.Add(stat, new Attribute(baseValue));
        }

        public void RegisterStatInstance(Stats stat, IAttribute instance)
        {
            _attrs[stat] = instance;
        }

        public IAttribute Get(Stats stat)
        {
            if (!_attrs.TryGetValue(stat, out var attribute))
                throw new KeyNotFoundException($"Stat {stat} not registered");

            return attribute;
        }

        public void ApplyBonus(Stats stat, float percentBonus)
        {
            if (percentBonus == 0f)
            {
                ResetToBase(stat);
                return;
            }

            var attribute = Get(stat);
            attribute.SetMaxValue(attribute.BaseValue * (1f + percentBonus));
        }

        public void ResetToBase(Stats stat)
        {
            var attribute = Get(stat);
            attribute.SetMaxValue(attribute.BaseValue);
        }
    }
}
