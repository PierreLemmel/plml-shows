using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Plml
{
    public static class PlmlUI
    {
        public static void Indented(Action action)
        {
            EditorGUI.indentLevel++;
            action();
            EditorGUI.indentLevel--;
        }

        public static void Disabled(Action action, bool disabled = true)
        {
            EditorGUI.BeginDisabledGroup(disabled);
            action();
            EditorGUI.EndDisabledGroup();
        }

        public static void Horizontal(Action action, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            action();
            EditorGUILayout.EndHorizontal();
        }

        public static void Vertical(Action action, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            action();
            EditorGUILayout.EndVertical();
        }
    }
}
