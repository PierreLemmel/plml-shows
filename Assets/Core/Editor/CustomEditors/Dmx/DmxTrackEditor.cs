using Plml.Dmx;
using System.Collections.Generic;
using System.Linq;
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

            DmxTrackElement[] elements = track.GetComponentsInChildren<DmxTrackElement>();
            DmxChannelType[] channelTypes = elements.SelectMany(elt => elt.fixture.model.channels.Select(cd => cd.type))
                .Distinct()
                .OrderBy(ct => (int)ct)
                .ToArray();

            foreach(var channelType in channelTypes)
            {
                IReadOnlyCollection<DmxTrackElement> eltsWithChan = elements
                    .Where(elt => elt.HasChannel(channelType))
                    .ToList();

                string label = channelType.ToString();
                if (channelType == DmxChannelType.Color)
                {
                    Color maxColor = Colors.Max(eltsWithChan.Select(elt => elt.GetColor()));
                    Color newColor = EditorGUILayout.ColorField(label, maxColor);

                    if (newColor != maxColor)
                    {
                        foreach (var elt in eltsWithChan)
                        {
                            elt.SetColor(newColor);
                        }
                    }
                }
                else
                {
                    int maxValue = eltsWithChan.Max(elt => elt.GetChannel(channelType));

                    int newValue = EditorGUILayout.IntSlider(label, maxValue, 0x00, 0xff);

                    if (newValue != maxValue)
                    {
                        foreach (var elt in eltsWithChan)
                        {
                            elt.SetChannel(channelType, (byte)newValue);
                        }
                    }
                }
            }

            base.OnInspectorGUI();
        }
    }
}