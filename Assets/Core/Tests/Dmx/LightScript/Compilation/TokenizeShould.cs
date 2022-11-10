using NUnit.Framework;
using Plml.Dmx.Scripting;
using Plml.Dmx.Scripting.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TokenType = Plml.Dmx.Scripting.Compilation.LightScriptTokenType;

namespace Plml.Tests.Dmx.Scripting.Compilation
{
    internal class TokenizeShould
    {
        private static string Stringify(IEnumerable<LightScriptToken> tokens) => string.Join(", ", tokens.Select(t => t.ToString()));

        [Test]
        [TestCaseSource(nameof(TokenizeTestCaseSource))]
        public void Return_Expected_Result(string input, LightScriptToken[] expected)
        {
            var result = LightScriptCompilation.Tokenize(input);

            Debug.Log("expected:\n" + Stringify(expected) + "\nresult:\n" + Stringify(result));
            CollectionAssert.AreEquivalent(expected, result);
        }

        [Test]
        [TestCaseSource(nameof(ValidateTokensTestCaseSource))]
        public void Validate_Valid_Input(string input)
        {
            var tokens = LightScriptCompilation.Tokenize(input);
            Assert.DoesNotThrow(() => LightScriptCompilation.ValidateTokens(tokens));
        }

        [Test]
        [TestCaseSource(nameof(TokenizeErrorsTestCaseSource))]
        public void Invalidate_When_Invalid_Tokens(string input, CompilationErrorType expectedErrorType)
        {
            var tokens = LightScriptCompilation.Tokenize(input);

            var exception = Assert.Throws<TokenizationException>(() => LightScriptCompilation.ValidateTokens(tokens));
            Assert.AreEqual(expectedErrorType, exception.ErrorType);
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
                    new(TokenType.Assignment),
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
                    new(TokenType.Assignment),
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
                    new(TokenType.Assignment),
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
                    new(TokenType.Assignment),
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
                    new(TokenType.Assignment),
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
                    new(TokenType.Assignment),
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
                    new(TokenType.Assignment),
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
                    new(TokenType.Assignment),
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
                    new(TokenType.Assignment),
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

            // nested functions
            new object[]
            {
                "parLed1.dimmer = abs(sin(0.3 * t)) * 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Identifier, "abs"),
                    new(TokenType.LeftBracket),
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

            // Multiline
            new object[]
            {
                @"parLed1.dimmer = 255
                parLed2.dimmer = 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                    new(TokenType.StatementEnding),
                    new(TokenType.Identifier, "parLed2"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                }
            },

            // Multiline (semicolons)
            new object[]
            {
                @"parLed1.dimmer = 255;
                parLed2.dimmer = 255;",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                    new(TokenType.StatementEnding),
                    new(TokenType.Identifier, "parLed2"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                    new(TokenType.StatementEnding),
                }
            },

            // Multiline (empty lines)
            new object[]
            {
                @"
                parLed1.dimmer = 255;

                parLed2.dimmer = 255;
                ",
                new LightScriptToken[]
                {
                    new(TokenType.StatementEnding),
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                    new(TokenType.StatementEnding),
                    new(TokenType.Identifier, "parLed2"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                    new(TokenType.StatementEnding),
                }
            },

            // Multiple assignments 1
            new object[]
            {
                "parLed1.dimmer = parLed2.dimmer = 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Identifier, "parLed2"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                }
            },

            // Multiple assignments 2
            new object[]
            {
                "parLed1.color.red = parLed1.color.green = parLed1.color.blue = 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "color"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "red"),
                    new(TokenType.Assignment),

                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "color"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "green"),
                    new(TokenType.Assignment),

                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "color"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "blue"),
                    new(TokenType.Assignment),

                    new(TokenType.Number, "255"),
                }
            },

            // Modulo
            new object[]
            {
                "parLed1.dimmer = 10000 % 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "10000"),
                    new(TokenType.Operator, "%"),
                    new(TokenType.Number, "255"),
                }
            },
            
            // Exponentiation
            new object[]
            {
                "parLed1.dimmer = 10 ^ 2",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "10"),
                    new(TokenType.Operator, "^"),
                    new(TokenType.Number, "2"),
                }
            },

            // Multiple arguments
            new object[]
            {
                "parLed1.dimmer = min(100, dimmer1, dimmer2, 255 * rng())",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Identifier, "min"),
                    new(TokenType.LeftBracket),
                    new(TokenType.Number, "100"),
                    new(TokenType.ArgumentSeparator),
                    new(TokenType.Identifier, "dimmer1"),
                    new(TokenType.ArgumentSeparator),
                    new(TokenType.Identifier, "dimmer2"),
                    new(TokenType.ArgumentSeparator),
                    new(TokenType.Number, "255"),
                    new(TokenType.Operator, "*"),
                    new(TokenType.Identifier, "rng"),
                    new(TokenType.LeftBracket),
                    new(TokenType.RightBracket),
                    new(TokenType.RightBracket),
                }
            },

            // Negatives number
            new object[]
            {
                "x = -200",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "x"),
                    new(TokenType.Assignment),
                    new(TokenType.Operator, "-"),
                    new(TokenType.Number, "200"),
                }
            },

            // Substraction
            new object[]
            {
                "x = 200 - 100",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "x"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "200"),
                    new(TokenType.Operator, "-"),
                    new(TokenType.Number, "100"),
                }
            },
        };

        public static IEnumerable<object> ValidateTokensTestCaseSource => TokenizeTestCaseSource.Select(objArr => objArr[0]);

        public static IEnumerable<object[]> TokenizeErrorsTestCaseSource => new object[][]
        {
            new object[]
            {
                "parLed.dimmer = 0.7.7 * 255",
                CompilationErrorType.InvalidNumberFormat
            },
            new object[]
            {
                "parLed.dimmer = 2S5",
                CompilationErrorType.InvalidNumberFormat
            },
            new object[]
            {
                "parLed.dimmer = 0xfg",
                CompilationErrorType.InvalidNumberFormat
            },
            new object[]
            {
                "parLed.dimmer = 0x0xff",
                CompilationErrorType.InvalidNumberFormat
            },
            new object[]
            {
                "parLed.dimmer = (50 + 60))",
                CompilationErrorType.InvalidBrackets
            },
            new object[]
            {
                "parLed.dimmer = (50 + 60",
                CompilationErrorType.InvalidBrackets
            },
            new object[]
            {
                "parLed.dimmer = 50 + 60)",
                CompilationErrorType.InvalidBrackets
            },
            new object[]
            {
                "parLed.dimmer = )50 + 60(",
                CompilationErrorType.InvalidBrackets
            },
            new object[]
            {
                @"parLed1.dimmer = (50 + 60
                parLed2.dimmer = 50 + 60)",
                CompilationErrorType.InvalidBrackets
            },
            new object[]
            {
                "parLed1.dimmer = min(100, dimmer1, dimmer2, 255 * rng(),)",
                CompilationErrorType.InvalidArgCount
            },
            new object[]
            {
                "parLed1.dimmer = min(,100, dimmer1, dimmer2, 255 * rng())",
                CompilationErrorType.InvalidArgCount
            }
        };
    }
}
