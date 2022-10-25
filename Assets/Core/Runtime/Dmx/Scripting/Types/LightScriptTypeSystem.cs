using Plml.Dmx.Scripting.Compilation.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static IEnumerable<LightScriptPropertyInfo> EnumerateProperties(Type type) =>
            Enumerables.Merge(
                type
                    .GetProperties()
                    .Select(pi => (name: pi.Name, type: MapFromSystemType(pi.PropertyType))),
                type
                    .GetFields()
                    .Select(fi => (name: fi.Name, type: MapFromSystemType(fi.FieldType)))
            )
            .Where(nt => nt.type != LightScriptType.Undefined)
            .Select(nt => new LightScriptPropertyInfo(nt.name, nt.type));

        private static LightScriptType MapFromSystemType(Type sysType)
        {
            if (sysType == typeof(int) || sysType == typeof(byte))
                return LightScriptType.Integer;
            else if (sysType == typeof(float))
                return LightScriptType.Float;
            else if (sysType == typeof(Color24))
                return LightScriptType.Color;
            else if (sysType == typeof(DmxTrackElement))
                return LightScriptType.Fixture;
            else
                return LightScriptType.Undefined;
        }

        private static LightScriptTypeInfo colorType = new(
            LightScriptType.Color,
            EnumerateProperties(typeof(Color24))
                .Append(new LightScriptPropertyInfo[]
                {
                    new("red", LightScriptType.Integer, nameof(Color24.r)),
                    new("green", LightScriptType.Integer, nameof(Color24.g)),
                    new("blue", LightScriptType.Integer, nameof(Color24.b)),

                    new("h", LightScriptType.Float, nameof(Color24.hue)),
                    new("s", LightScriptType.Float, nameof(Color24.saturation)),
                    new("v", LightScriptType.Float, nameof(Color24.value)),
                })
        );

        private static LightScriptTypeInfo fixtureType = new(
            LightScriptType.Fixture,
            EnumerateProperties(typeof(DmxTrackElement))
                .Append(new LightScriptPropertyInfo[]
                {
                    new("strobe", LightScriptType.Integer, nameof(DmxTrackElement.stroboscope)),
                })
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

        public static bool IsValidOperatorType(this BinaryOperatorType @operator, LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType)
        {
            switch (lhsType)
            {
                case LightScriptType.Float:
                    switch (rhsType)
                    {
                        case LightScriptType.Integer:
                        case LightScriptType.Float:
                            resultType = LightScriptType.Float;
                            return true;

                        case LightScriptType.Color:

                            if (@operator == BinaryOperatorType.Multiplication)
                            {
                                resultType = LightScriptType.Color;
                                return true;
                            }
                            break;
                    }
                    break;

                case LightScriptType.Integer:
                    switch (rhsType)
                    {
                        case LightScriptType.Integer:
                            resultType = LightScriptType.Integer;
                            return true;
                        case LightScriptType.Float:
                            resultType = LightScriptType.Float;
                            return true;

                        case LightScriptType.Color:

                            if (@operator == BinaryOperatorType.Multiplication)
                            {
                                resultType = LightScriptType.Color;
                                return true;
                            }
                            break;
                    }
                    break;

                case LightScriptType.Color:
                    switch (rhsType)
                    {
                        case LightScriptType.Integer:
                        case LightScriptType.Float:
                            if (@operator == BinaryOperatorType.Multiplication || @operator == BinaryOperatorType.Division)
                            {
                                resultType = LightScriptType.Color;
                                return true;
                            }
                            break;
                    }

                    break;

                case LightScriptType.Fixture:
                    break;
            }

            resultType = LightScriptType.Undefined;
            return false;
        }
    }
}