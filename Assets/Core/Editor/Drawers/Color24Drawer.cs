using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    [CustomPropertyDrawer(typeof(Color24), true)]
    public class Color24Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty rProperty = property.FindPropertyRelative("r");
            SerializedProperty gProperty = property.FindPropertyRelative("g");
            SerializedProperty bProperty = property.FindPropertyRelative("b");

            Color32 input = new(
                (byte)rProperty.intValue,
                (byte)gProperty.intValue,
                (byte)bProperty.intValue,
                0xff
            );

            Color32 result = EditorGUI.ColorField(position, label, input);

            rProperty.intValue = result.r;
            gProperty.intValue = result.g;
            bProperty.intValue = result.b;
        }
    }
}
