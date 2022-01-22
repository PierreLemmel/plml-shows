using System;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    [CustomPropertyDrawer(typeof(HideInPlayModeAttribute))]
    public class HideInPlayDrawer : PropertyDrawer
    {
        private bool IsPlaying => Application.isPlaying;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => IsPlaying ? 0.0f : EditorGUI.GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsPlaying)
                EditorGUI.PropertyField(position, property, label, true);
        }
    }
}