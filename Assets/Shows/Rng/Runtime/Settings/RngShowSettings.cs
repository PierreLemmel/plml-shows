using System;
using UnityEngine;

namespace Plml.Rng
{
    [Serializable]
    public class RngShowSettings
    {
        [Range(5, 50)]
        public int minScenes = 10;

        [Range(10, 1000)]
        public int maxScenes = 30;

        [CubicRange(0.2f, 4.0f)]
        public float durationSpread = 1.0f;

        [Range(600.0f, 7200.0f)]
        public float showDuration = 3600.0f;
    }
}