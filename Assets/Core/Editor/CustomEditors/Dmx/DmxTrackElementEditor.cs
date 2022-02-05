using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Plml.Dmx.Editor
{
    [CustomEditor(typeof(DmxTrackElement))]
    public class DmxTrackElementEditor : UEditor
    {
        public override void OnInspectorGUI()
        {
            DmxTrackElement trackElt = (DmxTrackElement)target;

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.ObjectField("Fixture", trackElt.fixture, typeof(DmxTrackElement), true);
            EditorGUI.EndDisabledGroup();

            int[] channels = trackElt.channels;
            foreach (DmxChannelDefinition chanDef in trackElt.fixture.model.channels)
            {
                int addr = chanDef.channel;
                string label = $"{chanDef.type} ({addr})";

                if (chanDef.type == DmxChannelType.Color)
                {
                    byte r = (byte)channels[addr];
                    byte g = (byte)channels[addr + 1];
                    byte b = (byte)channels[addr + 2];

                    Color32 result = EditorGUILayout.ColorField(label, new Color32(r, g, b, 0xff));

                    channels[addr] = result.r;
                    channels[addr + 1] = result.g;
                    channels[addr + 2] = result.b;
                }
                else
                {
                    channels[addr] = (byte)EditorGUILayout.IntSlider(label, channels[addr], 0x00, 0xff);
                }
            }

            EditorGUI.BeginDisabledGroup(true);

            int chan = 0;
            EditorGUILayout.LabelField("Channels: ");
            EditorGUI.indentLevel++;
            foreach (var val in trackElt.channels)
            {
                EditorGUILayout.IntSlider($"Chan {chan++}:" , val, 0, 0xff);
            }
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
        }
    }
}