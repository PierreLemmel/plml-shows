﻿using Plml.Dmx.Fixtures;
using UnityEngine;

namespace Plml.EnChiens.Emulators
{
    public class ParLedRGBWEmulator : LightEmulator<ParLedRGBW>
    {
        protected override Color32 GetColor() => fixture.color;
        protected override int GetIntensity() => fixture.dimmer;
        protected override int GetStrobe() => fixture.stroboscope;
    }
}