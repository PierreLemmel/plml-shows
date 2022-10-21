using Plml.Dmx.Scripting.Types;
using System;

namespace Plml.Dmx.Scripting
{
    public class LightScriptVariable
    {
        public LightScriptType Type { get; }
        public string Name { get; }

        public LightScriptVariable(LightScriptType type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}