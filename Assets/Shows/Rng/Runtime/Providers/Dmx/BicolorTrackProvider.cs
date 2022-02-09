using Plml.Dmx;
using System;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.Rng.Dmx
{
    public class BicolorTrackProvider : DmxTrackProvider
    {
        [EditTimeOnly]
        public DmxFixture jardinFixture;

        [EditTimeOnly]
        public DmxFixture courFixture;

        [EditTimeOnly]
        public DmxFixture[] contresFixtures;

        [RangeBounds01]
        public FloatRange saturationRange;

        [DmxRange]
        public IntRange facesDimmerRange;

        [DmxRange]
        public IntRange contresDimmerRange;

        public override DmxTrack GetElement()
        {
            this.AddChild("Contres")
                .WithComponent(out DmxTrack outTrack);

            float h1 = URandom.value;
            float h2 = (h1 + 1.0f / 3.0f) % 1.0f;
            float h3 = (h1 + 2.0f / 3.0f) % 1.0f;

            float saturation = MoreRandom.Range(saturationRange);

            int facesDimmer = MoreRandom.Range(facesDimmerRange);
            int contresDimmer = MoreRandom.Range(contresDimmerRange);
            Color32 jardinColor = Color.HSVToRGB(h1, saturation, 1.0f);
            Color32 courColor = Color.HSVToRGB(h2, saturation, 1.0f);
            Color32 contresColor = Color.HSVToRGB(h3, saturation, 1.0f);

            var jardinElt = outTrack.AddElement(jardinFixture);
            jardinElt.SetDimmer(facesDimmer);
            jardinElt.SetColor(jardinColor);

            var courElt = outTrack.AddElement(courFixture);
            courElt.SetDimmer(facesDimmer);
            courElt.SetColor(courColor);

            contresFixtures.ForEach(fixture =>
            {
                var contreElt = outTrack.AddElement(fixture);

                contreElt.TrySetDimmer(contresDimmer);
                contreElt.TrySetColor(contresColor);
            });

            return outTrack;
        }
    }
}