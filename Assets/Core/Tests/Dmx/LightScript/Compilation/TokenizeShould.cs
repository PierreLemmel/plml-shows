using NUnit.Framework;
using Plml.Dmx.Scripting;
using Plml.Dmx.Scripting.Compilation;
using System;
using System.Collections.Generic;

using TokenType = Plml.Dmx.Scripting.Compilation.LightScriptTokenType;

namespace Plml.Tests.Dmx.Scripting.Compilation
{
    internal class TokenizeShould
    {
        [Test]
        [TestCaseSource(nameof(TokenizeTestCaseSource))]
        public void Return_Expected_Result(string input, LightScriptToken[] expected)
        {
            IReadOnlyCollection<LightScriptToken> result = LightScriptCompilation.Tokenize(input);

            CollectionAssert.AreEquivalent(expected, result);
        }

        public static IEnumerable<object[]> TokenizeTestCaseSource => new object[][]
        {
            // Simple case
            new object[]
            {
                "parLed.dimmer = 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignation),
                    new(TokenType.Number, "255")
                }
            },
            
            // Semicolon
            new object[]
            {
                "parLed.dimmer = 255;",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignation),
                    new(TokenType.Number, "255"),
                    new(TokenType.StatementEnding)
                }
            },
            
            // Floating point
            new object[]
            {
                "parLed.dimmer = 0.7 * 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignation),
                    new(TokenType.Number, "0.7"),
                    new(TokenType.Operator, "*"),
                    new(TokenType.Number, "255")
                }
            },
            
            // Hexadecimal (no letter)
            new object[]
            {
                "parLed.dimmer = 0x00",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignation),
                    new(TokenType.Number, "0x00"),
                }
            },
            
            // Hexadecimal (letter)
            new object[]
            {
                "parLed.dimmer = 0xff",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignation),
                    new(TokenType.Number, "0xff"),
                }
            },

            // Hexadecimal (capital letter)
            new object[]
            {
                "parLed.dimmer = 0xFF",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignation),
                    new(TokenType.Number, "0xFF"),
                }
            },
            
            // Floating point and hesadecimals
            new object[]
            {
                "parLed.dimmer = 0.7 * 0xff",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignation),
                    new(TokenType.Number, "0.7"),
                    new(TokenType.Operator, "*"),
                    new(TokenType.Number, "0xff")
                }
            },

            // Number in identifier
            new object[]
            {
                "parLed1.dimmer = 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignation),
                    new(TokenType.Number, "255")
                }
            },

            // functions
            new object[]
            {
                "parLed1.dimmer = (1 + sin(0.3 * t)) * 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignation),
                    new(TokenType.LeftBracket),
                    new(TokenType.Number, "1"),
                    new(TokenType.Operator, "+"),
                    new(TokenType.Identifier, "sin"),
                    new(TokenType.LeftBracket),
                    new(TokenType.Number, "0.3"),
                    new(TokenType.Operator, "*"),
                    new(TokenType.Identifier, "t"),
                    new(TokenType.RightBracket),
                    new(TokenType.RightBracket),
                    new(TokenType.Operator, "*"),
                    new(TokenType.Number, "255"),
                }
            },
        };
    }
}
