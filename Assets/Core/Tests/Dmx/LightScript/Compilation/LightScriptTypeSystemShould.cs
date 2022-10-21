using NUnit.Framework;
using Plml.Dmx.Scripting;
using Plml.Dmx.Scripting.Compilation;
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
            new object[] { LightScriptType.Color, "v", true, LightScriptType.Integer },

            new object[] { LightScriptType.Color, "hue", true, LightScriptType.Float },
            new object[] { LightScriptType.Color, "saturation", true, LightScriptType.Float },
            new object[] { LightScriptType.Color, "value", true, LightScriptType.Float },


            new object[] { LightScriptType.Color, "foo", false, LightScriptType.Undefined },

            new object[] { LightScriptType.Fixture, "dimmer", true, LightScriptType.Integer },
            new object[] { LightScriptType.Fixture, "color", true, LightScriptType.Integer },
            new object[] { LightScriptType.Fixture, "strobe", true, LightScriptType.Integer },
            new object[] { LightScriptType.Fixture, "value", true, LightScriptType.Integer },

            new object[] { LightScriptType.Integer, "value", false, LightScriptType.Undefined },
            new object[] { LightScriptType.Float, "dimmer", false, LightScriptType.Undefined },
            new object[] { LightScriptType.Color, "dimmer", false, LightScriptType.Undefined },

            new object[] { LightScriptType.Fixture, "Dimmer", false, LightScriptType.Undefined },
            new object[] { LightScriptType.Fixture, "foo", false, LightScriptType.Undefined },
            new object[] { LightScriptType.Fixture, "channels", false, LightScriptType.Undefined },
        };
    }
}