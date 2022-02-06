using System;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    public abstract class CustomRangeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var range = (CustomRangeAttribute)attribute;

            float min = range.min;
            float max = range.max;

            float indentation = 14.0f * EditorGUI.indentLevel;

            float labelWidth = EditorGUIUtility.labelWidth;
            float currentViewWidth = EditorGUIUtility.currentViewWidth;

            float textWidth = 50.0f;
            float sliderWidth = currentViewWidth - labelWidth - 88.0f;
            float sliderExtra = 10.0f;
            float textExtra = EditorGUI.indentLevel > 0 ? -sliderExtra : 5.0f; //Dirty hack, I hate myself

            float a = ToSliderValue((property.floatValue - min) / (max - min));

            Rect labelRect = new(position.x + indentation, position.y, labelWidth - indentation, position.height);
            GUI.Label(labelRect, label);
            
            Rect sliderRect = new(position.x + labelWidth, position.y, sliderWidth + sliderExtra, position.height);
            a = GUI.HorizontalSlider(sliderRect, a, 0.0f, 1.0f);

            float value = min + FromSliderValue(a) * (max - min);

            Rect textRect = new(sliderRect.xMax + textExtra, position.y, textWidth, position.height);

            EditorStyles.numberField.fixedWidth = textWidth;
            value = EditorGUI.FloatField(textRect, value);
            EditorStyles.numberField.fixedWidth = 0.0f;

            property.floatValue = Mathf.Clamp(value, min, max);
        }

        protected abstract float ToSliderValue(float value);
        protected abstract float FromSliderValue(float value);
    }
}