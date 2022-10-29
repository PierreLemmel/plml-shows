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

        public static LightScriptType GetPropertyType(this LightScriptType type, string property) => type.HasProperty(property, out var propertyType) ?
            propertyType :
            throw new LightScriptTypeException($"Missing property '{property}' on type {type}");

        private static IDictionary<BinaryOperatorType, LightScriptOperatorInfo> operators = new Dictionary<BinaryOperatorType, LightScriptOperatorInfo>()
        {
            [BinaryOperatorType.Assignment] = new(BinaryOperatorType.Assignment, 0, false, IsValidAssignment),
            
            [BinaryOperatorType.Addition] = new(BinaryOperatorType.Addition, 10, true, IsValidAddition),
            [BinaryOperatorType.Substraction] = new(BinaryOperatorType.Substraction, 10, true, IsValidSubstraction),
            
            [BinaryOperatorType.Multiplication] = new(BinaryOperatorType.Multiplication, 20, true, IsValidMultiplication),
            [BinaryOperatorType.Division] = new(BinaryOperatorType.Division, 20, true, IsValidDivision),
        };

        private static bool IsValidAssignment(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType)
        {
            if (lhsType == rhsType)
            {
                resultType = lhsType;
                return true;
            }
            else if (lhsType == LightScriptType.Float && rhsType == LightScriptType.Integer)
            {
                resultType = LightScriptType.Float;
                return true;
            }

            resultType = LightScriptType.Undefined;
            return false;
        }

        private static bool IsValidAdditionSubstraction(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType)
        {
            switch (lhsType)
            {
                case LightScriptType.Float:
                    if (rhsType == LightScriptType.Integer || rhsType == LightScriptType.Float)
                    {
                        resultType = LightScriptType.Float;
                        return true;
                    }
                    break;

                case LightScriptType.Integer:
                    if (rhsType == LightScriptType.Integer)
                    {
                        resultType = LightScriptType.Integer;
                        return true;
                    }
                    else if (rhsType == LightScriptType.Float)
                    {
                        resultType = LightScriptType.Float;
                        return true;
                    }
                    break;

                case LightScriptType.Color:
                    if (rhsType == LightScriptType.Color)
                    {
                        resultType = LightScriptType.Color;
                        return true;
                    }
                    break;
            }

            resultType = LightScriptType.Undefined;
            return false;
        }

        private static bool IsValidAddition(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType) => IsValidAdditionSubstraction(lhsType, rhsType, out resultType);
        private static bool IsValidSubstraction(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType) => IsValidAdditionSubstraction(lhsType, rhsType, out resultType);

        private static bool IsValidMultiplication(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType)
        {
            switch (lhsType)
            {
                case LightScriptType.Float:
                    switch (rhsType)
                    {
                        case LightScriptType.Float:
                        case LightScriptType.Integer:
                            resultType = LightScriptType.Float;
                            return true;

                        case LightScriptType.Color:
                            resultType = LightScriptType.Color;
                            return true;
                    }

                    break;

                case LightScriptType.Integer:

                    switch (rhsType)
                    {
                        case LightScriptType.Float:
                            resultType = LightScriptType.Float;
                            return true;

                        case LightScriptType.Integer:
                            resultType = LightScriptType.Integer;
                            return true;

                        case LightScriptType.Color:
                            resultType = LightScriptType.Color;
                            return true;
                    }

                    break;
                    
                case LightScriptType.Color:

                    switch (rhsType)
                    {
                        case LightScriptType.Float:
                        case LightScriptType.Integer:
                            resultType = LightScriptType.Color;
                            return true;
                    }

                    break;
            }

            resultType = LightScriptType.Undefined;
            return false;
        }

        private static bool IsValidDivision(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType)
        {
            switch (lhsType)
            {
                case LightScriptType.Float:
                    switch (rhsType)
                    {
                        case LightScriptType.Float:
                        case LightScriptType.Integer:
                            resultType = LightScriptType.Float;
                            return true;
                    }

                    break;

                case LightScriptType.Integer:

                    switch (rhsType)
                    {
                        case LightScriptType.Float:
                            resultType = LightScriptType.Float;
                            return true;

                        case LightScriptType.Integer:
                            resultType = LightScriptType.Integer;
                            return true;
                    }

                    break;

                case LightScriptType.Color:

                    switch (rhsType)
                    {
                        case LightScriptType.Float:
                        case LightScriptType.Integer:
                            resultType = LightScriptType.Color;
                            return true;
                    }

                    break;
            }

            resultType = LightScriptType.Undefined;
            return false;
        }

        public static LightScriptOperatorInfo GetOperatorInfo(this BinaryOperatorType operatorType) => operators[operatorType];

        public static bool IsValidOperatorType(this BinaryOperatorType @operator, LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType) => GetOperatorInfo(@operator).IsValidOperatorType(lhsType, rhsType, out resultType);

        public static LightScriptType GetOperatorResultType(this BinaryOperatorType @operator, LightScriptType lhsType, LightScriptType rhsType) => @operator.IsValidOperatorType(lhsType, rhsType, out var result) ?
            result :
            throw new LightScriptException($"Result of operator '{@operator}' on types '{lhsType}' and '{rhsType}' cannot be inferred from usage");
    }
}