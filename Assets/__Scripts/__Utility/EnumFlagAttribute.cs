using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using TMPro.EditorUtilities;
using UnityEditor;
#endif

/// <summary>
/// Attribute used to mark an enum field for displaying as a flag mask in the Unity Inspector.
/// </summary>
public class EnumFlagAttribute : PropertyAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumFlagAttribute"/> class.
    /// </summary>
    public EnumFlagAttribute() { }
}

#if UNITY_EDITOR
/// <summary>
/// Custom property drawer for the <see cref="EnumFlagAttribute"/>.
/// Provides a flag mask field in the Unity Inspector for enum properties.
/// </summary>
[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer
{
    /// <summary>
    /// Renders the property in the Unity Inspector as a flag mask field.
    /// </summary>
    /// <param name="position">The rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The serialized property being drawn.</param>
    /// <param name="label">The label of the property.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Create a list of enum names to display, excluding "none" and "all".
        List<string> propsToShow = new List<string>(property.enumNames);
        propsToShow.Remove("none");
        propsToShow.Remove("all");
        
        // Render the mask field in the Unity Inspector.
        property.intValue = EditorGUI.MaskField(
            position, 
            label, 
            property.intValue, 
            propsToShow.ToArray()
        );
    }
}
#endif