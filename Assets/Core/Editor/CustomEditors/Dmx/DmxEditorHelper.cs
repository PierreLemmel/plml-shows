using UnityEngine;

using UObject = UnityEngine.Object;

namespace Plml.Dmx.Editor
{
    internal static class DmxEditorHelper
    {
        public static void AttachDefaultTrackElements(this DmxTrack track) => track.AddElements(UObject.FindObjectsOfType<DmxFixture>().Reverse());

        public static void AttachNewDefaultTrack(this Component component)
        {
            GameObject trackObj = new("New dmx track");
            trackObj.transform.SetParent(component.transform);
            var track = trackObj.AddComponent<DmxTrack>();
            track.AttachDefaultTrackElements();
        }
    }
}