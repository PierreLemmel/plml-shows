using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;


namespace Plml.Dmx.Scripting.Editor
{
    [CustomEditor(typeof(LightScript))]
    public class LightScriptEditor : UEditor
    {
        public override void OnInspectorGUI()
        {
            LightScript lightScript = (LightScript)target;

            base.OnInspectorGUI();

            PlmlUI.Disabled(() =>
            {
                if (GUILayout.Button("Compile"))
                    lightScript.Compile();
            }, !Application.isPlaying);
        }
    }
}