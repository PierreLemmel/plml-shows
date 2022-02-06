using System;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    [CustomPropertyDrawer(typeof(PlayTimeOnlyAttribute))]
    public class PlayTimeOnlyDrawer : OverlayPropertyDrawer<PlayTimeOnlyAttribute>
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI
            .GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = Application.isPlaying;
            DisplayProperty(position, property, label);
            GUI.enabled = false;
        }

        private void DisplayProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            if (InnerDrawer != null)
                InnerDrawer.OnGUI(position, property, label);
            else
                EditorGUI.PropertyField(position, property, label, true);
        }
    }
}