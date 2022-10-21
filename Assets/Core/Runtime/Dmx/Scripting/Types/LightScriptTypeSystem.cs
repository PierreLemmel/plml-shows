using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Dmx.Scripting.Types
{
    internal static class LightScriptTypeSystem
    {
        public static bool CanHaveProperties(this LightScriptType type) => type switch
        {
            LightScriptType.Integer or LightScriptType.Float => false,
            LightScriptType.Color or LightScriptType.Fixture => true,
            _ => throw new LightScriptTypeException($"Unsupported type: '{type}'")
        };

        private static LightScriptType MapFromSystemType(Type sysType)
        {
            if (sysType == typeof(int) || sysType == typeof(byte))
                return LightScriptType.Integer;
            else if (sysType == typeof(float))
                return LightScriptType.Float;
            else
                throw new LightScriptTypeException($"Unsupported System Type: {sysType.FullName}");
        }

        private static LightScriptTypeInfo colorType = new(
            LightScriptType.Color,
            typeof(Color24)
                .GetProperties()
                .Select(pi => new LightScriptPropertyInfo(
                    pi.Name, 
                    MapFromSystemType(pi.PropertyType)
                ))
        );

        private static LightScriptTypeInfo fixtureType = new(
            LightScriptType.Fixture,
            typeof(DmxTrackElement)
                .GetProperties()
                .Select(pi => new LightScriptPropertyInfo(
                    pi.Name,
                    MapFromSystemType(pi.PropertyType)
                ))
        );

        public static bool HasProperty(this LightScriptType type, string property, out LightScriptType propertyType)
        {
            LightScriptPropertyInfo pi = null;

            bool result = type switch
            {
                LightScriptType.Color => colorType.TryGetProperty(property, out pi),
                LightScriptType.Fixture => fixtureType.TryGetProperty(property, out pi),
                _ => false
            };

            propertyType = pi?.Type ?? LightScriptType.Undefined;

            return result;
        }
    }
}