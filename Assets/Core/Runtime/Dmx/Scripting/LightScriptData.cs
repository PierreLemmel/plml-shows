using System;

namespace Plml.Dmx.Scripting
{
    [Serializable]
    public class LightScriptData
    {
        public LightScriptFixtureData[] fixtures = Array.Empty<LightScriptFixtureData>();
        public LightScriptIntegerData[] integers = Array.Empty<LightScriptIntegerData>();

        public string text;
    }
}