using System;

namespace Plml.Dmx.Scripting.Types
{
    internal class LightScriptPropertyInfo
    {
        public string Name { get; }
        public LightScriptType Type { get; }

        public LightScriptPropertyInfo(string name, LightScriptType type)
        {
            Name = name;
            Type = type;
        }
    }
}