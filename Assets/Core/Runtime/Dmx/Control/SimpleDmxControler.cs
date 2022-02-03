using Plml.Dmx.SimpleFixtures;
using Plml.Dmx.OpenDmx;
using Plml.Dmx.OpenDmx.FTD2XX;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml.Dmx
{
    //[Obsolete("SimpleFixtures are obsolete, use Fixture definitions instead")]
    public class SimpleDmxControler : MonoBehaviour
    {
        public GameObject fixturesObject;

        [Range(0.0f, 1.0f)]
        public float master = 1.0f;

        [Range(0.0f, 60.0f)]
        public float fade = 0.0f;

        [Range(1.0f, 60.0f)]
        public float refreshRate = 30.0f;

        public bool enableOpenDmx = true;

        private SimpleDmxFixture[] fixtures;

        private IOpenDmxInterface openDmx = new FTD2XXInterface();

        private byte[] channels;
        private float[] currents;
        private float[] speeds;
        private byte[] targets;

        private float lastTime;

        private void Awake()
        {
            fixtures = fixturesObject.GetComponentsInChildren<SimpleDmxFixture>();

            int lastChannel = fixtures.Max(fix => fix.channelOffset + fix.Channels.Length);

            channels = new byte[lastChannel];
            currents = new float[lastChannel];
            speeds = new float[lastChannel];
            targets = new byte[lastChannel];

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
            if (Time.time - lastTime < 1.0f / refreshRate) return;

            foreach (SimpleDmxFixture fixture in fixtures)
                Array.Copy(fixture.Channels, 0, targets, fixture.channelOffset, fixture.Channels.Length);

            for (int i = 0; i < channels.Length; i++)
            {
                currents[i] = Mathf.SmoothDamp(currents[i], master * targets[i], ref speeds[i], fade);
                channels[i] = (byte)currents[i];
            }

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