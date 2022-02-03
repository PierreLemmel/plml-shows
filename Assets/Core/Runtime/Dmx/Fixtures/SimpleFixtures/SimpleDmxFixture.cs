using System;
using UnityEngine;

namespace Plml.Dmx.SimpleFixtures
{
    //[Obsolete("SimpleFixtures are obsolete, use Fixture definitions instead")]
    public abstract class SimpleDmxFixture : MonoBehaviour
    {
        [EditTimeOnly]
        [Range(1, 512)]
        public int channelOffset = 1;
        
        private byte[] channels;

        protected abstract int GetNumberOfChannels();
        protected abstract void UpdateChannels(byte[] channels);

        private void SetupChannels() => channels = new byte[GetNumberOfChannels()];

        public byte[] Channels
        {
            get
            {
                if (channels is null)
                    SetupChannels();

                UpdateChannels(channels);
                return channels;
            }
        }
    }
}