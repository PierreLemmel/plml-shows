﻿using UnityEngine;
using Plml.Dmx.Fixtures;

namespace Plml.EnChiens.Emulators
{
    public class FlatParEmulator : LightEmulator<FlatParLed_CW_WW_Amber>
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