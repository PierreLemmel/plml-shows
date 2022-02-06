using Plml.Dmx;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Plml.Dmx.Editor
{
    [CustomEditor(typeof(DmxTrack))]
    public class DmxTrackEditor : UEditor
    {
        public override void OnInspectorGUI()
        {
            DmxTrack track = (DmxTrack)target;

            if (!Application.isPlaying && track.transform.childCount == 0)
            {
                if (GUILayout.Button("Initialize"))
                {
                    track.AttachDefaultTrackElements();
                }
            }

            base.OnInspectorGUI();
        }
    }
}
