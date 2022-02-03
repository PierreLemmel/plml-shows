using System;
using UnityEngine;

namespace Plml.Dmx.SimpleFixtures
{
    public class TestFixture : SimpleDmxFixture
    {
        public int nbOfChannels = 16;
        public byte[] testChannels;

        protected override int GetNumberOfChannels() => nbOfChannels;
        protected override void UpdateChannels(byte[] channels) => Array.Copy(testChannels, channels, testChannels.Length);
    }
}