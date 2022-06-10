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
        private static Dictionary<string, bool> expandedCache = new();

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
                DmxChannelType chanType = chanDef.type;
                string label = $"{chanType} ({addr})";

                if (chanType.IsColorChannel())
                {
                    Color32 initialColor = ExtractColorFromArray(channels, addr);
                    Color32 result = EditorGUILayout.ColorField(label, initialColor);

                    ApplyColorToArray(channels, addr, result);
                }
                else if (chanType.IsColorArray())
                {   
                    int count = chanType.ColorArrayCount();

                    string expandedKey = $"{trackElt.name}/{chanType}";
                    bool expanded = false;
                    if (!expandedCache.TryGetValue(expandedKey, out expanded))
                        expandedCache.Add(expandedKey, expanded);

                    expanded = EditorGUILayout.Foldout(expanded, "Colors");

                    if (expanded)
                    {
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < count; i++)
                        {
                            Color32 initialColor = ExtractColorFromArray(channels, 3 * i);
                            Color32 result = EditorGUILayout.ColorField($"LED {i}", initialColor);

                            ApplyColorToArray(channels, 3 * i, result);
                        }
                        EditorGUI.indentLevel--;
                    }

                    expandedCache[expandedKey] = expanded;
                }
                else
                {
                    channels[addr] = (byte)EditorGUILayout.IntSlider(label, channels[addr], 0x00, 0xff);
                }
            }


            if (GUILayout.Button("Reset"))
            {
                Array.Clear(trackElt.channels, 0, trackElt.channels.Length);
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

        private static Color32 ExtractColorFromArray(int[] channels, int addr)
        {
            byte r = (byte)channels[addr];
            byte g = (byte)channels[addr + 1];
            byte b = (byte)channels[addr + 2];

            return new(r, g, b, 0xff);
        }

        private static void ApplyColorToArray(int[] channels, int addr, Color32 color)
        {
            channels[addr] = color.r;
            channels[addr + 1] = color.g;
            channels[addr + 2] = color.b;
        }
    }
}