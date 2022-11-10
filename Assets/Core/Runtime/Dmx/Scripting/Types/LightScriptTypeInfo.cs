using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Dmx.Scripting.Types
{
    internal class LightScriptTypeInfo
    {
        public LightScriptType Type { get; }
        public Type SystemType { get; }
        public bool CanHaveProperties => propertyLookup?.Any() ?? false;

        private Dictionary<string, LightScriptPropertyInfo> propertyLookup;

        public LightScriptTypeInfo(LightScriptType type, Type systemType, IEnumerable<LightScriptPropertyInfo> properties)
        {
            Type = type;
            SystemType = systemType;
            propertyLookup = properties.ToDictionary(
                pi => pi.Name,
                pi => pi
            );
            properties.ForEach(pi => pi.OwnerType = this);
        }

        public LightScriptTypeInfo(LightScriptType type, Type systemType, params LightScriptPropertyInfo[] properties) :
            this(type, systemType, properties.AsEnumerable())
        { }

        public bool TryGetProperty(string property, out LightScriptPropertyInfo propertyType) => propertyLookup
            .TryGetValue(property, out propertyType);
    }
}