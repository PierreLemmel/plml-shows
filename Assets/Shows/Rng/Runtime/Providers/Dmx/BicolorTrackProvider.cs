using Plml.Dmx;
using System;
using UnityEngine;

namespace Plml.Rng.Dmx
{
    public class ColorTrackProvider : DmxTrackProvider
    {
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