using System;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Plml.Rng
{
    public class SceneDurationProvider : RngProvider<TimeWindow>
    {
        [Min(0.0f)]
        public float minDuration = 10.0f;

        [Min(0.0f)]
        public float maxDuration = 60.0f;

        [Min(0.0f)]
        public float minFadeTime = 0.0f;

        [Min(0.0f)]
        public float maxFadeTime = 10.0f;

        public override TimeWindow GetElement() => new(0.0f, randomDuration, randomFadeTime, randomFadeTime);

        private float randomDuration => URandom.Range(minDuration, maxDuration);
        private float randomFadeTime => URandom.Range(minFadeTime, maxFadeTime);
    }
}
