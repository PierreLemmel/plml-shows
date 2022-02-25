using UnityEngine;

namespace Plml.Dmx.Emulators
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