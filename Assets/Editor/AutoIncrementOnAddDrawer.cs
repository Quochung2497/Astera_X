using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

/// <summary>
/// A PropertyDrawer that wraps any array marked [AutoIncrementOnAdd("someIntField")]
/// and automatically assigns each newly‐added element's `someIntField` to be
/// (largest existing value + 1).
/// </summary>
[CustomPropertyDrawer(typeof(AutoIncrementOnAddAttribute))]
public class AutoIncrementOnAddDrawer : PropertyDrawer
{
    // Cache one ReorderableList per propertyPath
    private readonly Dictionary<string, ReorderableList> _lists 
        = new Dictionary<string, ReorderableList>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Debug.Log($"Invoke >> {property.propertyPath}");
        // If this field is not an array‐type, draw it normally:
        if (property.propertyType != SerializedPropertyType.Generic || !property.isArray)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        // Grab our attribute so we know which integer field to bump:
        var attr = (AutoIncrementOnAddAttribute)attribute;
        string indexFieldName = attr.indexFieldName;

        // Use property.propertyPath (e.g. "levelSpawnRules") as a unique key:
        if (!_lists.TryGetValue(property.propertyPath, out var list))
        {
            list = new ReorderableList(
                property.serializedObject,
                property,
                draggable:      true,
                displayHeader:  true,
                displayAddButton:    true,
                displayRemoveButton: true
            );

            // 1) Draw the array’s header
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, label);
            };

            // 2) Draw each element in the array (with full default inspector style):
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = property.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            // 3) Calculate each element’s height (handles nested arrays, foldouts, etc.)
            list.elementHeightCallback = (int index) =>
            {
                var element = property.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 4;
            };

            // 4) Hook into “+” so we can auto‐increment our integer field
            list.onAddCallback = (ReorderableList l) =>
            {
                // a) Make sure we have the latest data in SerializedProperty:
                property.serializedObject.Update();

                // b) Increase the array size by exactly one:
                int newIndex = property.arraySize;
                property.arraySize++;

                // c) Commit that new size to the serialized object:
                property.serializedObject.ApplyModifiedProperties();
                
                // d) Re‐grab the array (it may have re‐serialized):
                property.serializedObject.Update();
                SerializedProperty newElement = property.GetArrayElementAtIndex(newIndex);

                // e) Find the maximum existing indexFieldName among all previous elements:
                int maxVal = 0;
                for (int i = 0; i < newIndex; i++)
                {
                    var existingElem = property.GetArrayElementAtIndex(i);
                    var idxProp = existingElem.FindPropertyRelative(indexFieldName);
                    if (idxProp != null && idxProp.propertyType == SerializedPropertyType.Integer)
                    {
                        maxVal = Mathf.Max(maxVal, idxProp.intValue);
                    }
                }

                // f) Now set the new element’s `indexFieldName = maxVal + 1`
                var newIdxProp = newElement.FindPropertyRelative(indexFieldName);
                if (newIdxProp != null && newIdxProp.propertyType == SerializedPropertyType.Integer)
                {
                    newIdxProp.intValue = maxVal + 1;
                }

                // g) Finally, apply the modified properties to actually write back to disk:
                property.serializedObject.ApplyModifiedProperties();
            };

            _lists[property.propertyPath] = list;
        }

        // 5) Draw the ReorderableList in place of the default Unity array UI:
        list.DoList(position);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // If not an array, Unity can calculate a normal height:
        if (property.propertyType != SerializedPropertyType.Generic || !property.isArray)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        // If we already built a ReorderableList for this property, ask it how tall it is:
        if (_lists.TryGetValue(property.propertyPath, out var list))
        {
            return list.GetHeight();
        }

        // Otherwise, reserve two lines so Unity can create the ReorderableList later:
        return EditorGUIUtility.singleLineHeight * 2;
    }
}