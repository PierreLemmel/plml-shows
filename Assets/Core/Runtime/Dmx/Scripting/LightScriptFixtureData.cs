using System;

namespace Plml.Dmx.Scripting
{
    [Serializable]
    public class LightScriptFixtureData
    {
        public string name;
        public DmxTrackElement fixture;

        public LightScriptFixtureData() { }

        public LightScriptFixtureData(string name, DmxTrackElement fixture)
        {
            this.name = name;
            this.fixture = fixture;
        }
    }
}