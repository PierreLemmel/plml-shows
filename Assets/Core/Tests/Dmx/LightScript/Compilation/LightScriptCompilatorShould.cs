using NUnit.Framework;
using Plml.Dmx;
using Plml.Dmx.Scripting;
using Plml.Dmx.Scripting.Compilation;
using Plml.Dmx.Scripting.Compilation.Nodes;
using Plml.Dmx.Scripting.Types;
using System.Collections.Generic;
using UnityEngine;

using TokenType = Plml.Dmx.Scripting.Compilation.LightScriptTokenType;

namespace Plml.Tests.Dmx.Scripting.Compilation
{
    internal class LightScriptCompilatorShould
    {
        private static readonly LightScriptCompilationOptions defaultCompilationOptions = new()
        {
            log = true
        };

        private static (GameObject fixtures, DmxFixture parLed1, DmxFixture parLed2) CreateSimpleLightingPlan()
        {
            GameObject fixtures = new("Fixtures");

            FixtureDefinition parLedFixtureDefinition = ScriptableObject.CreateInstance<FixtureDefinition>();
            parLedFixtureDefinition.name = "Par Led Test";
            parLedFixtureDefinition.manufacturer = "test";
            parLedFixtureDefinition.mode = "6CH";
            parLedFixtureDefinition.chanCount = 6;
            parLedFixtureDefinition.channels = new DmxChannelDefinition[]
            {
                new() { channel = 0, type = DmxChannelType.Color },
                new() { channel = 3, type = DmxChannelType.White },
                new() { channel = 4, type = DmxChannelType.Dimmer },
                new() { channel = 5, type = DmxChannelType.Stroboscope },
            };

            fixtures.AddChild("Par Led 1")
                .WithComponent(pr =>
                {
                    pr.model = parLedFixtureDefinition;
                    pr.channelOffset = 1;
                }, out DmxFixture parLed1);

            fixtures.AddChild("Par Led 2")
                .WithComponent(pr =>
                {
                    pr.model = parLedFixtureDefinition;
                    pr.channelOffset = 10;
                }, out DmxFixture parLed2);

            return (fixtures, parLed1, parLed2);
        }

        private static (DmxTrack track, DmxTrackElement parLed1TrackElement, DmxTrackElement parLed2TrackElement) CreateSimpleLightingPlanTrackElements()
        {
            (_, DmxFixture parLed1Fixture, DmxFixture parLed2Fixture) = CreateSimpleLightingPlan();

            new GameObject("Track").WithComponent<DmxTrack>(out var track);

            var parLed1TrackElement = track.AddElement(parLed1Fixture);
            var parLed2TrackElement = track.AddElement(parLed2Fixture);

            return (track, parLed1TrackElement, parLed2TrackElement);
        }

        private static LightScriptFixtureData[] GetFixtures(string name1, string name2 = null)
        {
            (_, var parLed1, var parLed2) = CreateSimpleLightingPlanTrackElements();

            return new LightScriptFixtureData[]
            {
                new(name1, parLed1),
                new(name2, parLed2)
            };
        }

        [Test]
        public void Build_Context_As_Expected()
        {
            (_, DmxTrackElement parLed1, DmxTrackElement parLed2) = CreateSimpleLightingPlanTrackElements();

            LightScriptData data = new()
            {
                text = "parLed1.dimmer = 120",
                fixtures = new LightScriptFixtureData[]
                {
                    new("parLed1", parLed1),
                    new("parLed2", parLed2),
                }
            };

            ILightScriptCompilationContext context = LightScriptCompilation.BuildContext(data);

            bool hasTime = context.TryGetVariable("t", out var timeVariable);
            Assert.That(hasTime, Is.True);
            Assert.That(timeVariable.Name, Is.EqualTo("t"));
            Assert.That(timeVariable.Type, Is.EqualTo(LightScriptType.Float));

            bool hasDeltaTime = context.TryGetVariable("dt", out var deltaTimeVariable);
            Assert.That(hasDeltaTime, Is.True);
            Assert.That(deltaTimeVariable.Name, Is.EqualTo("dt"));
            Assert.That(deltaTimeVariable.Type, Is.EqualTo(LightScriptType.Float));

            bool hasFrame = context.TryGetVariable("frame", out var frameVariable);
            Assert.That(hasFrame, Is.True);
            Assert.That(frameVariable.Name, Is.EqualTo("frame"));
            Assert.That(frameVariable.Type, Is.EqualTo(LightScriptType.Integer));

            bool hasParLed1 = context.TryGetVariable("parLed1", out var parLed1Variable);
            Assert.That(hasParLed1, Is.True);
            Assert.That(parLed1Variable.Name, Is.EqualTo("parLed1"));
            Assert.That(parLed1Variable.Type, Is.EqualTo(LightScriptType.Fixture));

            bool hasParLed2 = context.TryGetVariable("parLed2", out var parLed2Variable);
            Assert.That(hasParLed2, Is.True);
            Assert.That(parLed2Variable.Name, Is.EqualTo("parLed2"));
            Assert.That(parLed2Variable.Type, Is.EqualTo(LightScriptType.Fixture));
        }

        [Test]
        [TestCaseSource(nameof(BuildASTTestCaseSource))]
        public void Build_AST_As_Expected(string formula, LightScriptToken[] input, LightScriptData data, AbstractSyntaxTree expected)
        {
            ILightScriptCompilationContext context = LightScriptCompilation.BuildContext(data);
            AbstractSyntaxTree result = LightScriptCompilation.BuildAst(input, context);

            Debug.Log("expected:\n" + expected.Stringify() + "\n\nresult:\n" + result.Stringify());

            Assert.That(expected, Is.EqualTo(result));
        }

        public static IEnumerable<object[]> BuildASTTestCaseSource => new object[][]
        {
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
                },
                new LightScriptData()
                {
                    text = "parLed1.dimmer = 255",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            LightScriptType.Integer,
                            "dimmer"
                        ),
                        rhs: new ConstantNode(255)
                    )
                )
            },
            new object[]
            {
                "parLed1.dimmer = 255 - 100",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                    new(TokenType.Operator, "-"),
                    new(TokenType.Number, "100"),
                },
                new LightScriptData()
                {
                    text = "parLed1.dimmer = 255 - 100",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        LightScriptType.Integer,
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            LightScriptType.Integer,
                            "dimmer"
                        ),
                        rhs: new SubstractionNode(
                            new ConstantNode(255),
                            new ConstantNode(100)
                        )
                    )
                )
            },
            new object[]
            {
                "parLed1.color.red = 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "color"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "red"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                },
                new LightScriptData()
                {
                    text = "parLed1.color.red = 255",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode( 
                            new MemberAccessNode(
                                new VariableNode(LightScriptType.Fixture, "parLed1"),
                                "color"
                            ),
                            "red"
                        ),
                        rhs: new ConstantNode(255)
                    )
                )
            },
            new object[]
            {
                "dimmer1 = dimmer2 = 255",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "dimmer1"),
                    new(TokenType.Assignment),
                    new(TokenType.Identifier, "dimmer2"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                },
                new LightScriptData()
                {
                    text = "dimmer1 = dimmer2 = 255",
                    integers = new LightScriptIntegerData[]
                    {
                        new("dimmer1"),
                        new("dimmer2"),
                    },
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new VariableNode(LightScriptType.Integer, "dimmer1"),
                        rhs: new AssignmentNode(
                            lhs: new VariableNode(LightScriptType.Integer, "dimmer2"),
                            rhs: new ConstantNode(255)
                        )
                    )
                )
            },
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
                },
                new LightScriptData()
                {
                    text = "parLed1.dimmer = parLed2.dimmer = 255",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            "dimmer"
                        ),
                        rhs: new AssignmentNode(
                            lhs: new MemberAccessNode(
                                new VariableNode(LightScriptType.Fixture, "parLed2"),
                                "dimmer"
                            ),
                            rhs: new ConstantNode(255)
                        )
                    )
                )
            },
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
                },
                new LightScriptData()
                {
                    text = "parLed1.color.red = parLed1.color.green = parLed1.color.blue = 255",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new MemberAccessNode(
                                new VariableNode(LightScriptType.Fixture, "parLed1"),
                                "color"
                            ),
                            "red"
                        ),
                        rhs: new AssignmentNode(
                            lhs: new MemberAccessNode(
                                new MemberAccessNode(
                                    new VariableNode(LightScriptType.Fixture, "parLed1"),
                                    "color"
                                ),
                                "green"
                            ),
                            rhs: new AssignmentNode(
                                lhs: new MemberAccessNode(
                                    new MemberAccessNode(
                                        new VariableNode(LightScriptType.Fixture, "parLed1"),
                                        "color"
                                    ),
                                    "blue"
                                ),
                                rhs: new ConstantNode(255)
                            )
                        )
                    )
                )
            },
            new object[]
            {
                "parLed1.dimmer = 255 - 100 * 2 + 30",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.Number, "255"),
                    new(TokenType.Operator, "-"),
                    new(TokenType.Number, "100"),
                    new(TokenType.Operator, "*"),
                    new(TokenType.Number, "2"),
                    new(TokenType.Operator, "+"),
                    new(TokenType.Number, "30"),
                },
                new LightScriptData()
                {
                    text = "parLed1.dimmer = 255 - 100 * 2 + 30",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        LightScriptType.Integer,
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            LightScriptType.Integer,
                            "dimmer"
                        ),
                        rhs: new AdditionNode(
                            lhs: new SubstractionNode(
                                new ConstantNode(255),
                                new MultiplicationNode(
                                    new ConstantNode(100),
                                    new ConstantNode(2)
                                )
                            ),
                            rhs: new ConstantNode(30)
                        )
                    )
                )
            },
            new object[]
            {
                "parLed1.dimmer = (255 - 155) * 2 + 30",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),
                    new(TokenType.LeftBracket),
                    new(TokenType.Number, "255"),
                    new(TokenType.Operator, "-"),
                    new(TokenType.Number, "155"),
                    new(TokenType.RightBracket),
                    new(TokenType.Operator, "*"),
                    new(TokenType.Number, "2"),
                    new(TokenType.Operator, "+"),
                    new(TokenType.Number, "30"),
                },
                new LightScriptData()
                {
                    text = "parLed1.dimmer = (255 - 155) * 2 + 30",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        LightScriptType.Integer,
                        new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            LightScriptType.Integer,
                            "dimmer"
                        ),
                        new AdditionNode(
                            new MultiplicationNode(
                                new SubstractionNode(
                                    new ConstantNode(255),
                                    new ConstantNode(155)
                                ),
                                new ConstantNode(2)
                            ),
                            new ConstantNode(30)
                        )
                    )
                )
            },
            new object[]
            {
                "parLed1.dimmer = (50 + (2 + 1) * (5 + 5)) * 2 + 30",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),

                    new(TokenType.LeftBracket),
                    new(TokenType.Number, "50"),
                    new(TokenType.Operator, "+"),
                    
                    new(TokenType.LeftBracket),
                    new(TokenType.Number, "2"),
                    new(TokenType.Operator, "+"),
                    new(TokenType.Number, "1"),
                    new(TokenType.RightBracket),

                    new(TokenType.Operator, "*"),

                    new(TokenType.LeftBracket),
                    new(TokenType.Number, "5"),
                    new(TokenType.Operator, "+"),
                    new(TokenType.Number, "5"),
                    new(TokenType.RightBracket),

                    new(TokenType.RightBracket),

                    new(TokenType.Operator, "*"),
                    new(TokenType.Number, "2"),

                    new(TokenType.Operator, "+"),
                    new(TokenType.Number, "30"),
                },
                new LightScriptData()
                {
                    text = "parLed1.dimmer = (50 + (2 + 1) * (5 + 5)) * 2 + 30",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        LightScriptType.Integer,
                        new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            LightScriptType.Integer,
                            "dimmer"
                        ),
                        new AdditionNode(
                            new MultiplicationNode(
                                new AdditionNode(
                                    new ConstantNode(50),
                                    new MultiplicationNode(
                                        new AdditionNode(
                                            new ConstantNode(2),
                                            new ConstantNode(1)
                                        ),
                                        new AdditionNode(
                                            new ConstantNode(5),
                                            new ConstantNode(5)
                                        )
                                    )
                                ),
                                new ConstantNode(2)
                            ),
                            new ConstantNode(30)
                        )
                    )
                )
            },

            // Modulo
            new object[]
            {
                "parLed1.dimmer = 2 * 300 % 255 + 10",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),

                    new(TokenType.Number, "2"),
                    new(TokenType.Operator, "*"),
                    new(TokenType.Number, "300"),
                    new(TokenType.Operator, "%"),
                    new(TokenType.Number, "255"),
                    new(TokenType.Operator, "+"),
                    new(TokenType.Number, "10"),
                },
                new LightScriptData()
                {
                    text = "parLed1.dimmer = 2 * 300 % 255 + 10",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        LightScriptType.Integer,
                        new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            LightScriptType.Integer,
                            "dimmer"
                        ),
                        new AdditionNode(
                            new ModuloNode(
                                new MultiplicationNode(
                                    new ConstantNode(2),
                                    new ConstantNode(300)
                                ),
                                new ConstantNode(255)
                            ),  
                            new ConstantNode(10)
                        )
                    )
                )
            },

            // Exponentation
            new object[]
            {
                "parLed1.dimmer = 2 * 10 ^ 2 + 55",
                new LightScriptToken[]
                {
                    new(TokenType.Identifier, "parLed1"),
                    new(TokenType.DotNotation),
                    new(TokenType.Identifier, "dimmer"),
                    new(TokenType.Assignment),

                    new(TokenType.Number, "2"),
                    new(TokenType.Operator, "*"),
                    new(TokenType.Number, "10"),
                    new(TokenType.Operator, "^"),
                    new(TokenType.Number, "2"),
                    new(TokenType.Operator, "+"),
                    new(TokenType.Number, "55"),
                },
                new LightScriptData()
                {
                    text = "parLed1.dimmer = 2 * 10 ^ 2 + 55",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        LightScriptType.Integer,
                        new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            LightScriptType.Integer,
                            "dimmer"
                        ),
                        new AdditionNode(
                            new MultiplicationNode(
                                new ConstantNode(2),
                                new ExponentiationNode(
                                    new ConstantNode(10),
                                    new ConstantNode(2)
                                )
                            ),
                            new ConstantNode(55)
                        )
                    )
                )
            },
        };

        [Test]
        [TestCaseSource(nameof(OptimizeASTTestCaseSource))]
        public void Optimize_AST_As_Expected(AbstractSyntaxTree input, AbstractSyntaxTree expected)
        {
            AbstractSyntaxTree result = LightScriptCompilation.OptimizeAst(input);

            Debug.Log("expected:\n" + expected.Stringify() + "\n\nresult:\n" + result.Stringify());
            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> OptimizeASTTestCaseSource => new object[][]
        {
            new object[]
            {
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            "dimmer"
                        ),
                        rhs: new SubstractionNode(
                            new ConstantNode(255),
                            new ConstantNode(100)
                        )
                    )
                ),
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            "dimmer"
                        ),
                        rhs: new ConstantNode(155)
                    )
                )
            },
            new object[]
            {
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            "dimmer"
                        ),
                        rhs: new AdditionNode(
                            new MultiplicationNode(
                                new ConstantNode(2),
                                new ConstantNode(100)
                            ),
                            new ConstantNode(55)
                        )
                    )
                ),
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            "dimmer"
                        ),
                        rhs: new ConstantNode(255)
                    )
                )
            },
            new object[]
            {
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            "dimmer"
                        ),
                        rhs: new ExponentiationNode(
                            new ConstantNode(10),
                            new ConstantNode(2)
                        )
                    )
                ),
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            "dimmer"
                        ),
                        rhs: new ConstantNode(100)
                    )
                )
            },
            new object[]
            {
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            "dimmer"
                        ),
                        rhs: new ModuloNode(
                            new ConstantNode(500),
                            new ConstantNode(200)
                        )
                    )
                ),
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed1"),
                            "dimmer"
                        ),
                        rhs: new ConstantNode(100)
                    )
                )
            },
        };

        [Test]
        public void Compile_Without_Errors_For__Set_Dimmer_To_A_Constant_Value()
        {
            (_, DmxTrackElement parLed, _) = CreateSimpleLightingPlanTrackElements();

            LightScriptData data = new()
            {
                text = "parLed.dimmer = 120",
                fixtures = new LightScriptFixtureData[]
                {
                    new("parLed", parLed)
                }
            };
            LightScriptCompilationOptions options = defaultCompilationOptions;

            LightScriptCompilationResult result = LightScriptCompilation.Compile(data, options);

            Debug.Log(result.message);

            Assert.IsTrue(result.isOk);
            Assert.IsFalse(result.hasError);
        }

        [Test]
        public void Works_As_Expected__Set_Dimmer_To_A_Constant_Value()
        {
            (_, DmxTrackElement parLed, _) = CreateSimpleLightingPlanTrackElements();

            LightScriptData data = new()
            {
                text = "parLed.dimmer = 120",
                fixtures = new LightScriptFixtureData[]
                {
                    new("parLed", parLed)
                }
            };
            LightScriptCompilationOptions options = defaultCompilationOptions;

            LightScriptCompilationResult result = LightScriptCompilation.Compile(data, options);

            Debug.Log(result.message);

            ILightScriptContext context = new LightScriptContext();
            context.AddToContext("parLed", parLed);


            result.action(context);
            Assert.That(parLed.dimmer, Is.EqualTo(120));
        }
    }
}
