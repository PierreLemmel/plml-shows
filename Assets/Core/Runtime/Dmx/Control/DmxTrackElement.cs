using System;
using UnityEngine;

namespace Plml.Dmx
{
    [ExecuteAlways]
    public class DmxTrackElement : MonoBehaviour
    {
        [EditTimeOnly]
        public DmxFixture fixture;

        [ReadOnly]
        public int[] channels;

        public int Address => fixture.channelOffset;

        private void Awake()
        {
            channels = new int[fixture.channelOffset];
        }

        private void Update()
        {
            int chanCount = fixture.model.chanCount;
            if (channels.Length != chanCount)
                channels = new int[chanCount];
        }
    }
}