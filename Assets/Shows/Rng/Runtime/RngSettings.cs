using System;
using UnityEngine;

namespace Plml.Rng
{
    [Serializable]
    public class RngSettings
    {
        [Range(5, 15)]
        public int minScenes = 10;

        [Range(10, 40)]
        public int maxScenes = 30;

        [CubicRange(0.2f, 4.0f)]
        public float durationSpread = 1.0f;
    }
}