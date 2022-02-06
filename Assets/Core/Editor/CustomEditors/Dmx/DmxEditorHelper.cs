using System;
using System.Collections.Generic;
using UnityEngine;

using UObject = UnityEngine.Object;

namespace Plml.Dmx.Editor
{
    internal static class DmxEditorHelper
    {
        public static void AttachDefaultTrackElements(this Component component)
        {
            foreach (var fixture in UObject.FindObjectsOfType<DmxFixture>())
            {
                GameObject trackEltObj = new(fixture.name);
                trackEltObj.transform.SetParent(component.transform);
                var trackElt = trackEltObj.AddComponent<DmxTrackElement>();
                trackElt.fixture = fixture;
            }
        }

        public static void AttachNewDefaultTrack(this Component component)
        {
            GameObject trackObj = new("New dmx track");
            trackObj.transform.SetParent(component.transform);
            var track = trackObj.AddComponent<DmxTrack>();
            track.AttachDefaultTrackElements();
        }
    }
}