using Plml.Dmx.SimpleFixtures;
using UnityEngine;

namespace Plml.EnChiens.Emulators
{
    public class ServoEmulator : LightEmulator
    {
        protected override Color32 GetColor() => SumColors(
            ColorMix(fixture.cold, coldColor),
            ColorMix(fixture.warm, warmColor)
        );

        protected override int GetIntensity() => 255;
        protected override int GetStrobe() => fixture.stroboscope;
    }
}