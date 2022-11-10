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

        public static LightScriptType MapFromSystemType(Type sysType)
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

        public static Type MapFromToSystemType(LightScriptType lsType) => lsType switch
        {
            LightScriptType.Integer => typeof(int),
            LightScriptType.Float => typeof(float),
            LightScriptType.Color => typeof(Color24),
            LightScriptType.Fixture => typeof(DmxTrackElement),
            _ => throw new LightScriptTypeException($"Unsupported LightScriptType '{lsType}'")
        };

        private static LightScriptTypeInfo colorType = new(
            LightScriptType.Color,
            typeof(Color24),
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
            typeof(Color24),
            EnumerateProperties(typeof(DmxTrackElement))
                .Append(new LightScriptPropertyInfo[]
                {
                    new("strobe", LightScriptType.Integer, nameof(DmxTrackElement.stroboscope)),
                })
        );

        public static bool TryGetPropertyInfo(this LightScriptType type, string property, out LightScriptPropertyInfo propertyInfo)
        {
            propertyInfo = null;

            bool result = type switch
            {
                LightScriptType.Color => colorType.TryGetProperty(property, out propertyInfo),
                LightScriptType.Fixture => fixtureType.TryGetProperty(property, out propertyInfo),
                _ => false
            };

            return result;
        }

        public static LightScriptPropertyInfo GetPropertyInfo(this LightScriptType type, string property) => type.TryGetPropertyInfo(property, out LightScriptPropertyInfo propertyInfo) ?
            propertyInfo :
            throw new LightScriptTypeException($"Missing property '{property}' on type {type}");

        public static bool HasProperty(this LightScriptType type, string property, out LightScriptType propertyType)
        {
            if(type.TryGetPropertyInfo(property, out LightScriptPropertyInfo pi))
            {
                propertyType = pi.Type;
                return true;
            }
            else
            {
                propertyType = LightScriptType.Undefined;
                return false;
            }
        }

        public static LightScriptType GetPropertyType(this LightScriptType type, string property) => type.HasProperty(property, out var propertyType) ?
            propertyType :
            throw new LightScriptTypeException($"Missing property '{property}' on type {type}");

        private static IDictionary<BinaryOperatorType, LightScriptBinaryOperatorInfo> operators = new Dictionary<BinaryOperatorType, LightScriptBinaryOperatorInfo>()
        {
            [BinaryOperatorType.Assignment] = new(BinaryOperatorType.Assignment, 0, false, IsValidAssignment),
            
            [BinaryOperatorType.Addition] = new(BinaryOperatorType.Addition, 10, true, IsValidAddition),
            [BinaryOperatorType.Substraction] = new(BinaryOperatorType.Substraction, 10, true, IsValidSubstraction),
            
            [BinaryOperatorType.Multiplication] = new(BinaryOperatorType.Multiplication, 20, true, IsValidMultiplication),
            [BinaryOperatorType.Division] = new(BinaryOperatorType.Division, 20, true, IsValidDivision),

            [BinaryOperatorType.Modulo] = new(BinaryOperatorType.Modulo, 20, true, IsValidModulo),

            [BinaryOperatorType.Exponentiation] = new(BinaryOperatorType.Exponentiation, 30, true, IsValidExponentiation),
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

        private static bool IsValidModuloOrExponentation(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType)
        {
            if (lhsType == LightScriptType.Float)
            {
                if (rhsType == LightScriptType.Float || rhsType == LightScriptType.Integer)
                {
                    resultType = LightScriptType.Float;
                    return true;
                }
            }
            else if (lhsType == LightScriptType.Integer)
            {
                if (rhsType == LightScriptType.Float)
                {
                    resultType = LightScriptType.Float;
                    return true;
                }
                else if (rhsType == LightScriptType.Integer)
                {
                    resultType = LightScriptType.Integer;
                    return true;
                }
            }

            resultType = LightScriptType.Undefined;
            return false;
        }

        private static bool IsValidModulo(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType)
            => IsValidModuloOrExponentation(lhsType, rhsType, out resultType);

        private static bool IsValidExponentiation(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType)
            => IsValidModuloOrExponentation(lhsType, rhsType, out resultType);

        public static LightScriptBinaryOperatorInfo GetOperatorInfo(this BinaryOperatorType operatorType) => operators[operatorType];

        public static bool IsValidOperatorType(this BinaryOperatorType @operator, LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType) => GetOperatorInfo(@operator).IsValidOperatorType(lhsType, rhsType, out resultType);

        public static bool IsValidOperatorType(this UnaryOperatorType @operator, LightScriptType targetType) => targetType.IsOneOf(LightScriptType.Float, LightScriptType.Integer);

        public static LightScriptType GetOperatorResultType(this BinaryOperatorType @operator, LightScriptType lhsType, LightScriptType rhsType) => @operator.IsValidOperatorType(lhsType, rhsType, out var result) ?
            result :
            throw new LightScriptException($"Result of operator '{@operator}' on types '{lhsType}' and '{rhsType}' cannot be inferred from usage");

        public static bool HasImplicitConversion(LightScriptType from, LightScriptType to)
        {
            switch (from)
            {
                case LightScriptType.Integer:
                    switch (to)
                    {
                        case LightScriptType.Float:
                            return true;
                    }
                    break;
            }

            return false;
        }

        public static bool HasExplicitConversion(LightScriptType from, LightScriptType to)
        {
            switch (from)
            {
                case LightScriptType.Float:
                    switch (to)
                    {
                        case LightScriptType.Integer:
                            return true;
                    }
                    break;
            }

            return false;
        }


        public static bool IsAssignableTo(this LightScriptType inputType, LightScriptType targetType) => inputType == targetType
            || HasImplicitConversion(inputType, targetType);

        public static bool CanBeConvertedTo(this LightScriptType inputType, LightScriptType targetType) => inputType == targetType
            || HasImplicitConversion(inputType, targetType)
            || HasExplicitConversion(inputType, targetType);
    }
}