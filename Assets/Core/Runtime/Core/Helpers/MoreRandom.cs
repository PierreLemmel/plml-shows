using System;
using UnityEngine;

using URandom = UnityEngine.Random;
using UColor = UnityEngine.Color;

namespace Plml
{
    public static class MoreRandom
    {
        public static float NormalDistribution(float mean, float stdDev)
        {
            //@see: Box-Muller algorithm
            float u1 = URandom.value;
            float u2 = URandom.value;

            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

            float result = mean + stdDev * randStdNormal;

            return result;
        }

        public static UColor Color(UColor from, UColor to) => UColor.Lerp(from, to, URandom.value);

        public static float Range(FloatRange range) => URandom.Range(range.min, range.max);
        public static int Range(IntRange range) => URandom.Range(range.min, range.max);

        public static Vector2 Vector2(float maxAmplitude) => URandom.insideUnitCircle * maxAmplitude;

        public static bool Boolean => URandom.value >= 0.5;
    }
}