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
                    foreach (var fixture in FindObjectsOfType<DmxFixture>())
                    {
                        GameObject trackEltObj = new(fixture.name);
                        trackEltObj.transform.SetParent(track.transform);
                        var trackElt = trackEltObj.AddComponent<DmxTrackElement>();
                        trackElt.fixture = fixture;
                    }
                }
            }

            base.OnInspectorGUI();
        }
    }
}
