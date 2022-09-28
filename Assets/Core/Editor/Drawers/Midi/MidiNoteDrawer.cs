using Plml.Midi;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plml.Editor.Midi
{
    [CustomPropertyDrawer(typeof(MidiNote))]
    public class MidiNoteDrawer : PropertyDrawer
    {
        private static string[] labels = Enumerables.Sequence(128)
            .Select(i => ((MidiNote)i).Label())
            .ToArray();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int intValue = property.intValue;
            property.intValue = EditorGUI.Popup(position, intValue, labels);
        }
    }
}
