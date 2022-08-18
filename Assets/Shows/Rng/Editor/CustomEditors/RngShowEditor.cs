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

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Start Show")) show.StartShow();
                if (GUILayout.Button("Stop Show")) show.StopShow();
            }
            else
            {
                if (GUILayout.Button("Regenerate Scenes"))
                {
                    Debug.Log("");
                    Debug.Log("Generating show...");
                    show.RegenerateScenes();
                    Debug.Log("Show generated");

                    foreach (var scene in show.scenes)
                    {
                        Debug.Log(scene.name);
                    }
                    Debug.Log("");
                }

                if (GUILayout.Button("Save show"))
                {
                    show.SerializeShow();
                }
            }

            base.OnInspectorGUI();
        }
    }
}