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
        public int[] channels = Array.Empty<int>();

        public int Address => fixture.channelOffset;

        private void Update()
        {
            int chanCount = fixture?.model.chanCount ?? 0;
            if (channels.Length != chanCount)
                channels = new int[chanCount];
        }
    }
}