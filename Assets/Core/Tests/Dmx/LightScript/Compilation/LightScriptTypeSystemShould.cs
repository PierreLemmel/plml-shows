using NUnit.Framework;
using Plml.Dmx.Scripting;
using Plml.Dmx.Scripting.Compilation;
using Plml.Dmx.Scripting.Compilation.Nodes;
using Plml.Dmx.Scripting.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plml.Tests.Dmx.Scripting.Compilation
{
    internal class LightScriptTypeSystemShould
    {
        [Test]
        [TestCaseSource(nameof(CanHavePropertiesTestCaseSource))]
        public void Can_Have_Properties_Returns_Expected(LightScriptType type, bool expected) => Assert
            .That(type.CanHaveProperties(), Is.EqualTo(expected));

        public static IEnumerable<object[]> CanHavePropertiesTestCaseSource => new object[][]
        {
            new object[] { LightScriptType.Integer, false },
            new object[] { LightScriptType.Float, false },
            new object[] { LightScriptType.Color, true },
            new object[] { LightScriptType.Fixture, true },
        };

        [Test]
        [TestCaseSource(nameof(HasPropertyTestCaseSource))]
        public void HasProperty_Returns_Expected(LightScriptType type, string property, bool expected, LightScriptType expectedType)
        {
            bool result = type.HasProperty(property, out LightScriptType resultType);
            
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(resultType, Is.EqualTo(expectedType));
        }

        public static IEnumerable<object[]> HasPropertyTestCaseSource => new object[][]
        {
            new object[] { LightScriptType.Color, "r", true, LightScriptType.Integer },
            new object[] { LightScriptType.Color, "g", true, LightScriptType.Integer },
            new object[] { LightScriptType.Color, "b", true, LightScriptType.Integer },

            new object[] { LightScriptType.Color, "red", true, LightScriptType.Integer },
            new object[] { LightScriptType.Color, "green", true, LightScriptType.Integer },
            new object[] { LightScriptType.Color, "blue", true, LightScriptType.Integer },


            new object[] { LightScriptType.Color, "h", true, LightScriptType.Float },
            new object[] { LightScriptType.Color, "s", true, LightScriptType.Float },
            new object[] { LightScriptType.Color, "v", true, LightScriptType.Float },

            new object[] { LightScriptType.Color, "hue", true, LightScriptType.Float },
            new object[] { LightScriptType.Color, "saturation", true, LightScriptType.Float },
            new object[] { LightScriptType.Color, "value", true, LightScriptType.Float },


            new object[] { LightScriptType.Color, "foo", false, LightScriptType.Undefined },

            new object[] { LightScriptType.Fixture, "dimmer", true, LightScriptType.Integer },
            new object[] { LightScriptType.Fixture, "color", true, LightScriptType.Color },
            new object[] { LightScriptType.Fixture, "strobe", true, LightScriptType.Integer },
            new object[] { LightScriptType.Fixture, "stroboscope", true, LightScriptType.Integer },
            new object[] { LightScriptType.Fixture, "value", true, LightScriptType.Integer },

            new object[] { LightScriptType.Integer, "value", false, LightScriptType.Undefined },
            new object[] { LightScriptType.Float, "dimmer", false, LightScriptType.Undefined },
            new object[] { LightScriptType.Color, "dimmer", false, LightScriptType.Undefined },

            new object[] { LightScriptType.Fixture, "Dimmer", false, LightScriptType.Undefined },
            new object[] { LightScriptType.Fixture, "foo", false, LightScriptType.Undefined },
            new object[] { LightScriptType.Fixture, "channels", false, LightScriptType.Undefined },
        };

        [Test]
        [TestCaseSource(nameof(IsValidOperatorType_TestCaseSource))]
        public void IsValidOperatorType_Returns_Expected(BinaryOperatorType binaryOperator, LightScriptType lhsType, LightScriptType rhsType, bool expectedResult, LightScriptType expectedResultType)
        {
            bool result = binaryOperator.IsValidOperatorType(lhsType, rhsType, out LightScriptType resultType);

            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(resultType, Is.EqualTo(expectedResultType));
        }

        public static IEnumerable<object[]> IsValidOperatorType_TestCaseSource => new object[][]
        {
            // Integer + Integer
            new object[]
            {
                BinaryOperatorType.Assignment,
                LightScriptType.Integer,
                LightScriptType.Integer,
                true,
                LightScriptType.Integer
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Integer,
                LightScriptType.Integer,
                true,
                LightScriptType.Integer
            },
            new object[]
            {
                BinaryOperatorType.Substraction,
                LightScriptType.Integer,
                LightScriptType.Integer,
                true,
                LightScriptType.Integer
            },
            new object[]
            {
                BinaryOperatorType.Multiplication,
                LightScriptType.Integer,
                LightScriptType.Integer,
                true,
                LightScriptType.Integer
            },
            new object[]
            {
                BinaryOperatorType.Division,
                LightScriptType.Integer,
                LightScriptType.Integer,
                true,
                LightScriptType.Integer
            },
            new object[]
            {
                BinaryOperatorType.Modulo,
                LightScriptType.Integer,
                LightScriptType.Integer,
                true,
                LightScriptType.Integer
            },
            new object[]
            {
                BinaryOperatorType.Exponentiation,
                LightScriptType.Integer,
                LightScriptType.Integer,
                true,
                LightScriptType.Integer
            },

            // Float + Float
            new object[]
            {
                BinaryOperatorType.Assignment,
                LightScriptType.Float,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Float,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Substraction,
                LightScriptType.Float,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Multiplication,
                LightScriptType.Float,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Division,
                LightScriptType.Float,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Modulo,
                LightScriptType.Float,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Exponentiation,
                LightScriptType.Float,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },

            // Float + Integer
            new object[]
            {
                BinaryOperatorType.Assignment,
                LightScriptType.Float,
                LightScriptType.Integer,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Assignment,
                LightScriptType.Integer,
                LightScriptType.Float,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Float,
                LightScriptType.Integer,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Integer,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Substraction,
                LightScriptType.Float,
                LightScriptType.Integer,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Substraction,
                LightScriptType.Integer,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Multiplication,
                LightScriptType.Float,
                LightScriptType.Integer,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Multiplication,
                LightScriptType.Integer,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Division,
                LightScriptType.Float,
                LightScriptType.Integer,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Division,
                LightScriptType.Integer,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Modulo,
                LightScriptType.Float,
                LightScriptType.Integer,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Modulo,
                LightScriptType.Integer,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Exponentiation,
                LightScriptType.Float,
                LightScriptType.Integer,
                true,
                LightScriptType.Float
            },
            new object[]
            {
                BinaryOperatorType.Exponentiation,
                LightScriptType.Integer,
                LightScriptType.Float,
                true,
                LightScriptType.Float
            },

            // DmxFixture + X
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Fixture,
                LightScriptType.Integer,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Fixture,
                LightScriptType.Float,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Fixture,
                LightScriptType.Color,
                false,
                LightScriptType.Undefined
            },

            // Color + Color
            new object[]
            {
                BinaryOperatorType.Assignment,
                LightScriptType.Color,
                LightScriptType.Color,
                true,
                LightScriptType.Color
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Color,
                LightScriptType.Color,
                true,
                LightScriptType.Color
            },
            new object[]
            {
                BinaryOperatorType.Substraction,
                LightScriptType.Color,
                LightScriptType.Color,
                true,
                LightScriptType.Color
            },
            new object[]
            {
                BinaryOperatorType.Multiplication,
                LightScriptType.Color,
                LightScriptType.Color,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Division,
                LightScriptType.Color,
                LightScriptType.Color,
                false,
                LightScriptType.Undefined
            },

            // Color + Integer/Float
            // Multiplication
            new object[]
            {
                BinaryOperatorType.Multiplication,
                LightScriptType.Color,
                LightScriptType.Integer,
                true,
                LightScriptType.Color
            },
            new object[]
            {
                BinaryOperatorType.Multiplication,
                LightScriptType.Integer,
                LightScriptType.Color,
                true,
                LightScriptType.Color
            },
            new object[]
            {
                BinaryOperatorType.Multiplication,
                LightScriptType.Color,
                LightScriptType.Float,
                true,
                LightScriptType.Color
            },
            new object[]
            {
                BinaryOperatorType.Multiplication,
                LightScriptType.Float,
                LightScriptType.Color,
                true,
                LightScriptType.Color
            },

            // Division
            new object[]
            {
                BinaryOperatorType.Division,
                LightScriptType.Color,
                LightScriptType.Integer,
                true,
                LightScriptType.Color
            },
            new object[]
            {
                BinaryOperatorType.Division,
                LightScriptType.Integer,
                LightScriptType.Color,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Division,
                LightScriptType.Color,
                LightScriptType.Float,
                true,
                LightScriptType.Color
            },
            new object[]
            {
                BinaryOperatorType.Division,
                LightScriptType.Float,
                LightScriptType.Color,
                false,
                LightScriptType.Undefined
            },

            // Addition
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Color,
                LightScriptType.Integer,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Integer,
                LightScriptType.Color,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Color,
                LightScriptType.Float,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Addition,
                LightScriptType.Float,
                LightScriptType.Color,
                false,
                LightScriptType.Undefined
            },

            // Substraction
            new object[]
            {
                BinaryOperatorType.Substraction,
                LightScriptType.Color,
                LightScriptType.Integer,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Substraction,
                LightScriptType.Integer,
                LightScriptType.Color,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Substraction,
                LightScriptType.Color,
                LightScriptType.Float,
                false,
                LightScriptType.Undefined
            },
            new object[]
            {
                BinaryOperatorType.Substraction,
                LightScriptType.Float,
                LightScriptType.Color,
                false,
                LightScriptType.Undefined
            },
        };
    }
}