using Plml.Rng.Dmx;
using System;
using UnityEngine;

namespace Plml.Rng
{
    [Serializable]
    public class RngIntroOutroSettings
    {
        public SceneDurationProvider durationProvider;

        public DmxTrackProvider dmxProvider;

        [Range(0f, 30f)]
        public float lightOffset;

        public AudioClip music;

        [Range(0.0f, 1.0f)]
        public float volume;
    }
}