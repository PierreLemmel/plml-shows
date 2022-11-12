using System;

namespace Plml.Dmx.Scripting.Runtime
{
    [Serializable]
    public class FixtureVariableInfo : VariableInfo
    {
        public DmxTrackElement fixture;

        public FixtureVariableInfo()
        {
            isReadOnly = true;
        }

        public FixtureVariableInfo(string name, DmxTrackElement fixture)
        {
            this.name = name;
            this.fixture = fixture;
        }
    }
}