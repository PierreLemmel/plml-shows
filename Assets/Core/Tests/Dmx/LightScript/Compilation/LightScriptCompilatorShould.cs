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
        private LightScriptCompilationOptions defaultCompilationOptions;

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

            ILightScriptContext context = LightScriptCompilation.BuildContext(data);

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
        public void Return_Expected_Result(LightScriptToken[] input, LightScriptData data, AbstractSyntaxTree expected)
        {
            ILightScriptContext context = LightScriptCompilation.BuildContext(data);
            AbstractSyntaxTree result = LightScriptCompilation.BuildAst(input, context);

            Debug.Log("expected:\n" + expected.Stringify() + "\n\nresult:\n" + result.Stringify());

            Assert.That(expected, Is.EqualTo(result));
        }

        public static IEnumerable<object[]> BuildASTTestCaseSource => new object[][]
        {
            new object[]
            {
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
                    text = "parLed.dimmer = 255",
                    fixtures = GetFixtures("parLed1", "parLed2")
                },
                new AbstractSyntaxTree(
                    new AssignmentNode(
                        lhs: new MemberAccessNode(
                            new VariableNode(LightScriptType.Fixture, "parLed"),
                            "dimmer"
                        ),
                        rhs: new ConstantNode(255)
                    )
                )
            }
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

            Assert.IsTrue(result.isOk);
            Assert.IsFalse(result.hasError);
        }
    }
}
