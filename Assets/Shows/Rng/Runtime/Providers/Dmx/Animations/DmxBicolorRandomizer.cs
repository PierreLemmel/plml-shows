using Plml.Dmx;
using System;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.Rng.Dmx.Animations
{
    public class DmxBicolorRandomizer : MonoBehaviour
    {
        public DmxTrackElement[] fixturesColor1;
        public DmxTrackElement[] fixturesColor2;

        [RangeBounds01]
        public FloatRange saturationRange = new(0f, 1f);

        [RangeBounds01]
        public FloatRange valueRange = new(1f, 1f);

        public BicolorComplementaryType complementaryType = BicolorComplementaryType.ThirdOfCircle;

        private void Awake()
        {
            float h1 = URandom.value;
            float s = MoreRandom.Range(saturationRange);
            float v = MoreRandom.Range(valueRange);

            float h2 = complementaryType switch
            {
                BicolorComplementaryType.HalfCircle => (h1 + .5f) % 1f,
                BicolorComplementaryType.ThirdOfCircle => (h1 + (MoreRandom.Boolean ? 1f : 2f) / 3f) % 1f,
                _ => throw new InvalidOperationException()
            };

            Color24 color1 = Color.HSVToRGB(h1, s, v);
            Color24 color2 = Color.HSVToRGB(h2, s, v);

            foreach (var fixture in fixturesColor1)
                fixture.SetColor(color1);

            foreach (var fixture in fixturesColor2)
                fixture.SetColor(color2);
        }

        public enum BicolorComplementaryType
        {
            HalfCircle,
            ThirdOfCircle
        }
    }
}