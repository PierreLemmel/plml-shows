using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UEditor = UnityEditor.Editor;

namespace Plml.Rng.Editor
{
    [CustomEditor(typeof(RngShow))]
    public class RngShowEditor : UEditor
    {
        public override void OnInspectorGUI()
        {
            RngShow show = (RngShow)target;

            if (GUILayout.Button("Start Show")) show.StartShow();
            if (GUILayout.Button("Stop Show")) show.StopShow();
            if (GUILayout.Button("Regenerate Scenes")) show.RegenerateScenes();

            base.OnInspectorGUI();
        }
    }
}