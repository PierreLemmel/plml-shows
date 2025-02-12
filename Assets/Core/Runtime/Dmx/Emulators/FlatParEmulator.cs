﻿using UnityEngine;

namespace Plml.Dmx.Emulators
{
    public class FlatParEmulator : LightEmulator
    {
        protected override Color32 GetColor() => SumColors(
            ColorMix(fixture.cold, coldColor),
            ColorMix(fixture.warm, warmColor),
            ColorMix(fixture.amber, amberColor)
        );

        protected override int GetIntensity() => fixture.dimmer;
        protected override int GetStrobe() => 0;
    }
}