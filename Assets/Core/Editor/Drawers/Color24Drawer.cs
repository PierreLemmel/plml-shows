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
            SerializedObject foo = property.serializedObject;
            Color32 color = EditorGUILayout.ColorField(label, property.colorValue);
            property.FindPropertyRelative("r").intValue = color.r;
            property.FindPropertyRelative("g").intValue = color.g;
            property.FindPropertyRelative("b").intValue = color.b;
        }
    }
}
