using System;

namespace Plml.Dmx.Scripting
{
    [Serializable]
    public class LightScriptFloatData
    {
        public string name;
        public float defaultValue;

        public LightScriptFloatData() { }
        public LightScriptFloatData(string name, float defaultValue = 0)
        {
            this.name = name;
            this.defaultValue = defaultValue;
        }
    }
}