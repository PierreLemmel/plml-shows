﻿using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    EditorGUI.LabelField(position, label, new GUIContent(property.boolValue.ToString()));
                    break;
                case SerializedPropertyType.Enum:
                    EditorGUI.LabelField(position, label, new GUIContent(property.enumDisplayNames[property.enumValueIndex]));
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.LabelField(position, label, new GUIContent(property.floatValue.ToString()));
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.LabelField(position, label, new GUIContent(property.intValue.ToString()));
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.LabelField(position, label, new GUIContent(property.stringValue));
                    break;
                default:
                    Debug.LogError($"Unsupported property type: {property.propertyType}");
                    break;
            }
        }
    }
}