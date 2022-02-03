using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml.Dmx.SimpleFixtures
{
    public class ParLedServo_SharkCombi : SimpleDmxFixture
    {
        [Range(0, 0xff)]
        public int dimmer;

        [Range(0, 0xff)]
        public int pan;

        [Range(0, 0xff)]
        public int tilt;

        [Range(0, 0xff)]
        public int cold;

        [Range(0, 0xff)]
        public int warm;

        public Color32 color = Color.black;

        [Range(0, 0xff)]
        public int strobe;

        protected override int GetNumberOfChannels() => 20;

        protected override void UpdateChannels(byte[] channels)
        {
            channels[0] = (byte)pan;
            channels[2] = (byte)tilt;
            channels[5] = (byte)strobe;
            channels[6] = color.r;
            channels[7] = color.g;
            channels[8] = color.b;
            channels[9] = (byte)cold;
            channels[10] = (byte)warm;
        }
    }
}