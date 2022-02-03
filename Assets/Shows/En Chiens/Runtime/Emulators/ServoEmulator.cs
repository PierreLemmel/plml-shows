using Plml.Dmx.SimpleFixtures;
using UnityEngine;

namespace Plml.EnChiens.Emulators
{
    public class ServoEmulator : LightEmulator<ParLedServo_SharkCombi>
    {
        protected override Color32 GetColor() => SumColors(
            ColorMix(fixture.cold, coldColor),
            ColorMix(fixture.warm, warmColor)
        );

        protected override int GetIntensity() => fixture.dimmer;
        protected override int GetStrobe() => fixture.strobe;
    }
}