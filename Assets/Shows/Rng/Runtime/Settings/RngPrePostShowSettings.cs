using Plml.Rng.Audio;
using Plml.Rng.Dmx;
using System;
using UnityEngine;

namespace Plml.Rng
{
    [Serializable]
    public class RngPrePostShowSettings
    {
        [Range(0f, 20f)]
        public float timeGap = 10.0f;

        [Range(0f, 5f)]
        public float fade = 0f;

        public DmxTrackProvider dmxProvider;
    }
}