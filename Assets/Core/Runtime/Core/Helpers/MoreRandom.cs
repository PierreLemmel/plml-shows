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

        public static Color Color(Color from, Color to) => UColor.Lerp(from, to, URandom.value);
    }
}