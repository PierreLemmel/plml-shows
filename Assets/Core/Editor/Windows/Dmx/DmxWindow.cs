using System;
using UnityEngine;
using UnityEditor;

namespace Plml.Dmx.Editor
{
    public class DmxWindow : EditorWindow
    {
        private bool sending = false;

        private void OnGUI()
        {
            DmxTrackControler dmxControler = FindObjectOfType<DmxTrackControler>();

            EditorGUILayout.LabelField("Dmx Control");
            PlmlUI.Horizontal(() =>
            {
                PlmlUI.Disabled(() =>
                {
                    if (GUILayout.Button("Send Current Dmx State"))
                    {
                        dmxControler.SetupFromEditor();
                        sending = true;
                    }
                }, dmxControler == null || sending);

                PlmlUI.Disabled(() =>
                {
                    if (GUILayout.Button("Stop"))
                    {
                        dmxControler.StopSendingFrameFromEditor();
                        sending = false;
                    }
                }, !sending);
            });

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Dmx Info :");
            
            byte[] channels = dmxControler?.GetChannelsFromEditor();
            if (channels != null)
            {
                EditorGUILayout.LabelField($"Channel count: {channels.Length}");

                EditorGUILayout.Separator();


                PlmlUI.Horizontal(() =>
                {
                    GUILayoutOption sizeOption = GUILayout.Width(35f);
                    for (int i = 0; i < channels.Length; i++)
                    {
                        PlmlUI.Vertical(() =>
                        {
                            EditorGUILayout.LabelField(i < 10 ? "  " + i : (i < 100 ? " " + i : i.ToString()), sizeOption);
                            EditorGUILayout.LabelField(channels[i].ToString("000"), sizeOption);
                        }, sizeOption);
                    }
                }, GUILayout.ExpandWidth(false));
            }
            else
                EditorGUILayout.LabelField("Nothing to display");
        }

        private int frame;
        private void Update()
        {
            if (sending && frame++ % 10 == 0)
            {
                DmxTrackControler dmxControler = FindObjectOfType<DmxTrackControler>();

                if (Application.isPlaying)
                {
                    dmxControler.StopSendingFrameFromEditor();
                    sending = false;
                }
                
                dmxControler.SendCurrentFrameFromEditor();
                Repaint();
            }
        }
    }
}