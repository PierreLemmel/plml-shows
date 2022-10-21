using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Dmx.Scripting.Types
{
    internal class LightScriptTypeInfo
    {
        public LightScriptType Type { get; }
        public bool CanHaveProperties => propertyLookup?.Any() ?? false;

        private Dictionary<string, LightScriptPropertyInfo> propertyLookup;

        public LightScriptTypeInfo(LightScriptType type, IEnumerable<LightScriptPropertyInfo> properties)
        {
            Type = type;
            propertyLookup = properties.ToDictionary(
                pi => pi.Name,
                pi => pi
            );
        }

        public LightScriptTypeInfo(LightScriptType type, params LightScriptPropertyInfo[] properties) :
            this(type, properties.AsEnumerable())
        { }

        public bool TryGetProperty(string property, out LightScriptPropertyInfo propertyType) => propertyLookup
            .TryGetValue(property, out propertyType);
    }
}