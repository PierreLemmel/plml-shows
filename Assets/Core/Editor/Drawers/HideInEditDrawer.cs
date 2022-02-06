using System;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    [CustomPropertyDrawer(typeof(HideInPlayModeAttribute))]
    public class HideInEditDrawer : PropertyDrawer
    {
        private bool IsEditing => !Application.isPlaying;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => IsEditing ? 0.0f : EditorGUI.GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsEditing)
                EditorGUI.PropertyField(position, property, label, true);
        }
    }
}