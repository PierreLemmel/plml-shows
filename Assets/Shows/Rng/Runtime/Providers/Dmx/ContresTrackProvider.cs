using Plml.Dmx;
using System;
using UnityEngine;

namespace Plml.Rng.Dmx
{
    public class ContresTrackProvider : DmxTrackProvider
    {
        [EditTimeOnly]
        public DmxFixture facesFixtures;

        [EditTimeOnly]
        public DmxFixture contresFixtures;

        [RangeBounds(0.0f, 1.0f)]
        public FloatRange facesDimmer;
        public Color32 faceFromColor;

        public override DmxTrack GetElement()
        {
            GameObject original = GetComponentInChildren<DmxTrack>().gameObject;

            GameObject clone = Instantiate(original);
            return clone.GetComponent<DmxTrack>();
        }

        private void Awake()
        {
            GameObject tracksObject = GetComponentInChildren<DmxTrack>()?.gameObject ?? new GameObject("Track")
                .WithComponent<DmxTrack>()
                .AttachTo(this);
        }
    }
}