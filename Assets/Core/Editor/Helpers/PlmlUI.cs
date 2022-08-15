using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

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
    }
}
