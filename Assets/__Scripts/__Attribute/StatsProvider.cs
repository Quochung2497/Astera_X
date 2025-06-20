using System.Collections.Generic;
using Course.Attribute;
using UnityEngine;

[DisallowMultipleComponent]
public class StatsProvider : MonoBehaviour, IStatsProvider
{
    readonly Dictionary<Stats, IAttribute> _attrs = new();

    /// <summary>
    /// Call once from each Installer, passing in your config SO data.
    /// </summary>
    public void RegisterStat(Stats stats, float baseValue)
    {
        if (_attrs.ContainsKey(stats))
        {
            Debug.LogWarning($"StatsProvider: stat {stats} already registered; skipping registration.");
            return;
        }
        _attrs[stats] = new Attribute(baseValue);
    }
    
    public void RegisterStatInstance(Stats stat, IAttribute instance)
    {
        _attrs[stat] = instance; 
    }

    /// <summary>
    /// Everyone else asks for the same instance.
    /// </summary>
    public IAttribute Get(Stats stats)
    {
        if (!_attrs.TryGetValue(stats, out var a))
            throw new KeyNotFoundException($"Stat {stats} not registered");
        return a;
    }
    
    public void SetMaxValue(Stats stats, float percentBonus)
    {
        var attr = Get(stats);
        attr.SetMaxValue(attr.BaseValue * (1f + percentBonus));
    }
    
    public void ResetAllToBase()
    {
        foreach (var kv in _attrs)
        {
            var attr = kv.Value;
            attr.SetMaxValue(attr.BaseValue);
        }
    }
}
