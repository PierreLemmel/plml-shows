using Plml.Dmx;
using UnityEngine;

namespace Plml.Rng.Dmx.Animations
{
    public class DmxColorRandomizer : MonoBehaviour
    {
        public DmxTrackElement[] fixtures;

        [RangeBounds01]
        public FloatRange saturationRange = new(0f, 1f);

        [RangeBounds01]
        public FloatRange valueRange = new(1f, 1f);

        private void Awake()
        {
            float h = Random.value;
            float s = MoreRandom.Range(saturationRange);
            float v = MoreRandom.Range(valueRange);

            Color24 color = Color.HSVToRGB(h, s, v);

            foreach (var fixture in fixtures)
                fixture.SetColor(color);
        }
    }
}