using System;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml
{
    public static class MoreMath
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
    }
}