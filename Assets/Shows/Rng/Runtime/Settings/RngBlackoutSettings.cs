using Plml.Rng.Dmx;
using System;
using UnityEngine;

namespace Plml.Rng
{
    [Serializable]
    public class RngBlackoutSettings
    {
        [Range(1f, 10f)]
        public float duration = 5f;

        [Range(0f, 2.5f)]
        public float fade = 0.8f;

        public DmxTrackProvider dmxProvider;
    }
}