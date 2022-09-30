using Plml.Dmx;
using UnityEngine;

namespace Plml.Rng.Dmx.Animations
{
    public class DmxChanRandomizer : MonoBehaviour
    {
        public DmxTrackElement[] fixtures;
        public DmxChannelType channel;

        [DmxRange]
        public IntRange range = new(0x00, 0xff);

        private void Awake()
        {
            int value = MoreRandom.Range(range);

            foreach (var fixture in fixtures)
                fixture.SetChannel(channel, value);
        }
    }
}