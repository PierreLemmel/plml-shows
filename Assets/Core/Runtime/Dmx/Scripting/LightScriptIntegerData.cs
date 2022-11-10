using System;

namespace Plml.Dmx.Scripting
{
    [Serializable]
    public class LightScriptIntegerData
    {
        public string name;
        public int defaultValue;

        public LightScriptIntegerData() { }
        public LightScriptIntegerData(string name, int defaultValue = 0)
        { 
            this.name = name;
            this.defaultValue = defaultValue;
        }
    }
}