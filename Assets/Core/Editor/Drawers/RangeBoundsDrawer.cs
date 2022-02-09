using System;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    [CustomPropertyDrawer(typeof(RangeBoundsAttribute), true)]
    public class RangeBoundsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RangeBoundsAttribute bounds = (RangeBoundsAttribute)attribute;

            if (bounds.rangeType == RangeBoundsAttribute.RangeType.Float)
            {
                float min = property.FindPropertyRelative("min").floatValue;
                float max = property.FindPropertyRelative("max").floatValue;

                string sliderLabel = $"{label} ({min:0.00} | {max:0.00})";
                EditorGUI.MinMaxSlider(position, sliderLabel, ref min, ref max, bounds.lowerBound, bounds.upperBound);

                property.FindPropertyRelative("min").floatValue = min;
                property.FindPropertyRelative("max").floatValue = max;
            }
            else
            {
                float min = property.FindPropertyRelative("min").intValue;
                float max = property.FindPropertyRelative("max").intValue;

                string sliderLabel = $"{label} ({min} | {max})";
                EditorGUI.MinMaxSlider(position, sliderLabel, ref min, ref max, bounds.lowerBound, bounds.upperBound);

                property.FindPropertyRelative("min").intValue = Mathf.RoundToInt(min);
                property.FindPropertyRelative("max").intValue = Mathf.RoundToInt(max);
            }
        }
    }
}
