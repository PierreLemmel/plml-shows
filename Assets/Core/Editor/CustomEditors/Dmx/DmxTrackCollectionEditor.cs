using System;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Plml.Dmx.Editor
{
    [CustomEditor(typeof(DmxTrackCollection))]
    public class DmxTrackCollectionEditor : UEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DmxTrackCollection trackCollection = (DmxTrackCollection)target;


            if (GUILayout.Button("Add track"))
            {
                trackCollection.AttachNewDefaultTrack();
            }
        }
    }
}