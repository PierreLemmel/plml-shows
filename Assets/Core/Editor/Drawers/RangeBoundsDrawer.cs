using System;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    [CustomPropertyDrawer(typeof(RangeBoundsAttribute))]
    public class RangeBoundsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RangeBoundsAttribute bounds = (RangeBoundsAttribute)attribute;

            Debug.Log(property.propertyType);

            //EditorGUI.MinMaxSlider(position, )
            base.OnGUI(position, property, label);
        }
    }
}
