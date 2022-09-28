using Plml.Dmx;
using System;
using UnityEngine;

namespace Plml.Rng.Dmx
{
    public class StaticTrackProvider : DmxTrackProvider
    {
        public string trackName = "";

        public override DmxTrack GetNextElement()
        {
            GameObject original = GetComponentInChildren<DmxTrack>().gameObject;

            GameObject clone = Instantiate(original);

            clone.name = !string.IsNullOrEmpty(trackName) ? trackName : original.name;
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