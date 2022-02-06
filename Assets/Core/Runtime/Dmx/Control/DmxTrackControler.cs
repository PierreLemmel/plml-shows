using Plml.Dmx.SimpleFixtures;
using Plml.Dmx.OpenDmx;
using Plml.Dmx.OpenDmx.FTD2XX;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml.Dmx
{
    public class DmxTrackControler : MonoBehaviour
    {
        [EditTimeOnly]
        public GameObject fixturesObject;

        [EditTimeOnly]
        public DmxTrackCollection tracksObject;

        [Range(0.0f, 1.0f)]
        public float master = 1.0f;

        [Range(0.0f, 60.0f)]
        public float fade = 0.0f;

        [Range(1.0f, 60.0f)]
        public float refreshRate = 30.0f;

        public bool enableOpenDmx = true;

        private IOpenDmxInterface openDmx = new FTD2XXInterface();

        private byte[] channels;
        private float[] currents;
        private float[] speeds;
        private float[] targets;

        private float lastTime;

        private void Awake()
        {
            int lastChannel = fixturesObject
                .GetComponentsInChildren<DmxFixture>()
                .Max(fix => fix.channelOffset + fix.model.chanCount);

            channels = new byte[lastChannel];
            currents = new float[lastChannel];
            speeds = new float[lastChannel];
            targets = new float[lastChannel];

            lastTime = Time.time;

            if (enableOpenDmx)
                openDmx.Start();
        }

        private void OnEnable()
        {
            if (enableOpenDmx)
                openDmx.CopyData(channels);
        }

        private void Update()
        {
            Array.Clear(targets, 0, targets.Length);

            var tracks = tracksObject.GetComponentsInChildren<DmxTrack>();

            foreach (DmxTrack track in tracks.Where(t => t.isPlaying))
            {
                float master = track.master;

                foreach (DmxTrackElement elt in track.Elements)
                {
                    int[] channels = elt.channels;
                    for (int i=0, address = elt.Address; i<channels.Length; i++, address++)
                    {
                        targets[address] = Math.Max(targets[address], master * channels[i]);
                    }
                }
            }

            for (int i = 0; i < channels.Length; i++)
            {
                currents[i] = Mathf.SmoothDamp(currents[i], master * targets[i], ref speeds[i], fade);
                channels[i] = (byte)currents[i];
            }

            if (Time.time - lastTime < 1.0f / refreshRate) return;

            if (enableOpenDmx)
            {
                openDmx.CopyData(channels);
                openDmx.SendFrame();
            }

            lastTime = Time.time;
        }

        private void OnDisable()
        {
            if (enableOpenDmx)
                openDmx.ClearFrame();
        }

        private void OnDestroy()
        {
            if (enableOpenDmx)
            {
                openDmx.Stop();
                openDmx.Dispose();
            }
        }
    }
}