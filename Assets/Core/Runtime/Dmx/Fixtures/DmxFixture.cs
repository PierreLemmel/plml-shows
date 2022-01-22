using System;
using UnityEngine;

namespace Plml.Dmx.Fixtures
{
    public abstract class DmxFixture : MonoBehaviour
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