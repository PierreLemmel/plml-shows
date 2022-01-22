using System;
using UnityEngine;

namespace Plml.Dmx.Fixtures
{
    public class TestFixture : DmxFixture
    {
        public int nbOfChannels = 16;
        public byte[] testChannels;

        protected override int GetNumberOfChannels() => nbOfChannels;
        protected override void UpdateChannels(byte[] channels) => Array.Copy(testChannels, channels, testChannels.Length);
    }
}