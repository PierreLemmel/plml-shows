using Plml.Rng.Audio;
using Plml.Rng.Dmx;
using System;
using UnityEngine;

namespace Plml.Rng
{
    [Serializable]
    public class RngPrePostShowSettings
    {
        [Range(1f, 20f)]
        public float timeGap = 10.0f;

        public DmxTrackProvider dmxProvider;

        public AudioClipProvider musics;
    }
}