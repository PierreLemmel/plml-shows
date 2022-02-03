using System;
using UnityEngine;

namespace Plml.Dmx.SimpleFixtures
{
    public class FlatParLed_CW_WW_Amber : SimpleDmxFixture
    {
        [Range(0x00, 0xff)]
        public int dimmer;

        [Range(0x00, 0xff)]
        public int cold;

        [Range(0x00, 0xff)]
        public int warm;

        [Range(0x00, 0xff)]
        public int amber;

        protected override int GetNumberOfChannels() => 4;

        protected override void UpdateChannels(byte[] channels)
        {
            channels[0] = (byte)cold;
            channels[1] = (byte)warm;
            channels[2] = (byte)amber;
            channels[3] = (byte)dimmer;
        }
    }
}
