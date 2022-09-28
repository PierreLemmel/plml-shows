using Plml.Dmx;
using System;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Plml.Rng.Dmx.Improvibar
{
    public class ContresTrackProvider : DmxTrackProvider
    {
        [EditTimeOnly]
        public DmxFixture[] facesFixtures;

        [EditTimeOnly]
        public DmxFixture[] contresFixtures;

        [DmxRange]
        public IntRange facesDimmerRange;

        public Color32 faceFromColor;
        public Color32 faceToColor;

        [DmxRange]
        public IntRange contresDimmerRange;

        [RangeBounds01]
        public FloatRange contresSaturationRange;

        public override DmxTrack GetNextElement()
        {
            this.AddChild("Contres")
                .WithComponent(out DmxTrack outTrack);

            int facesDimmer = MoreRandom.Range(facesDimmerRange);
            Color32 facesColor = MoreRandom.Color(faceFromColor, faceToColor);

            int contresDimmer = MoreRandom.Range(contresDimmerRange);
            float contresSaturation = MoreRandom.Range(contresSaturationRange);
            Color32 contresColor = Color.HSVToRGB(URandom.value, contresSaturation, 1.0f);

            facesFixtures.ForEach(fixture =>
            {
                var faceElt = outTrack.AddElement(fixture);

                faceElt.TrySetDimmer(facesDimmer);
                faceElt.TrySetColor(facesColor);
            });

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