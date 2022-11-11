using System;

namespace Plml.Dmx.Scripting
{
    public class LightScriptData
    {
        public LightScriptFixtureData[] fixtures = Array.Empty<LightScriptFixtureData>();
        public LightScriptIntegerData[] integers = Array.Empty<LightScriptIntegerData>();
        public LightScriptFloatData[] floats = Array.Empty<LightScriptFloatData>();
        public LightScriptColorData[] colors = Array.Empty<LightScriptColorData>();

        public string text;
    }
}