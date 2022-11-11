using System;

namespace Plml.Dmx.Scripting
{
    [Serializable]
    public class LightScriptColorData
    {
        public string name;
        public Color24 defaultValue;

        public LightScriptColorData() { }
        public LightScriptColorData(string name, Color24 defaultValue = new())
        {
            this.name = name;
            this.defaultValue = defaultValue;
        }
    }
}