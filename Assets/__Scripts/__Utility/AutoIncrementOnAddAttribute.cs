using System;
using UnityEngine;

/// <summary>
/// Put this on any array‐field in a ScriptableObject; 
/// the inspector will automatically increment the 'indexFieldName' inside each element.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class AutoIncrementOnAddAttribute : PropertyAttribute
{
    // The name of the int field inside each array‐element that should be auto‐incremented.
    public string indexFieldName;

    public AutoIncrementOnAddAttribute(string indexFieldName)
    {
        this.indexFieldName = indexFieldName;
    }
}