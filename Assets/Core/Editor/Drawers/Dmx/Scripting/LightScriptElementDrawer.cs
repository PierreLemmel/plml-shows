using UnityEditor;
using UnityEngine;

namespace Plml.Dmx.Scripting.Editor
{
    [CustomPropertyDrawer(typeof(LightScriptElement), true)]
    public class LightScriptElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.LabelField("Test");
            base.OnGUI(position, property, label);
        }
    }
}
