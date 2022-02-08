using System;
using UnityEngine;

namespace Plml.Rng.Audio
{
    [Serializable]
    public class RngAudioData
    {
        [EditTimeOnly]
        public TimeWindow musicWindow;

        [EditTimeOnly]
        public float audioVolume;

        [EditTimeOnly]
        public AudioClip audioClip;
    }
}