using System;

namespace Plml.Dmx.Scripting.Types
{
    internal class LightScriptPropertyInfo
    {
        public string Name { get; }
        public LightScriptType Type { get; }
        public string UnderlyingProperty { get; }
        public LightScriptTypeInfo OwnerType { get; internal set; }

        public LightScriptPropertyInfo(string name, LightScriptType type, string property)
        {
            Name = name;
            Type = type;
            UnderlyingProperty = property;
        }

        public LightScriptPropertyInfo(string name, LightScriptType type)
            : this(name, type, name) { }
    }
}