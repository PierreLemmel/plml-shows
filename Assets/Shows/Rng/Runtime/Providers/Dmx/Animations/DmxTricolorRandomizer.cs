using Plml.Dmx;
using System;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.Rng.Dmx.Animations
{
    public class DmxTricolorRandomizer : MonoBehaviour
    {
        public DmxTrackElement[] fixturesColor1;
        public DmxTrackElement[] fixturesColor2;
        public DmxTrackElement[] fixturesColor3;

        [RangeBounds01]
        public FloatRange saturationRange = new(0f, 1f);

        [RangeBounds01]
        public FloatRange valueRange12 = new(1f, 1f);

        [RangeBounds01]
        public FloatRange valueRange3 = new(1f, 1f);


        private void Awake()
        {
            float h1 = URandom.value;

            bool hrDirection = MoreRandom.Boolean;
            float h2 = (h1 + (hrDirection ? 1f : 2f) / 3f) % 1f;
            float h3 = (h1 + (hrDirection ? 2f : 1f) / 3f) % 1f;

            float s = MoreRandom.Range(saturationRange);
            float v12 = MoreRandom.Range(valueRange12);
            float v3 = MoreRandom.Range(valueRange3);

            
            Color24 color1 = Color.HSVToRGB(h1, s, v12);
            Color24 color2 = Color.HSVToRGB(h2, s, v12);
            Color24 color3 = Color.HSVToRGB(h3, s, v3);


            foreach (var fixture in fixturesColor1) fixture.SetColor(color1);
            foreach (var fixture in fixturesColor2) fixture.SetColor(color2);
            foreach (var fixture in fixturesColor3) fixture.SetColor(color3);
        }
    }
}