using NUnit.Framework;
using Plml.Dmx;
using Plml.Dmx.Scripting;
using System.Collections.Generic;
using UnityEngine;

using UObject = UnityEngine.Object;

namespace Plml.Tests.Dmx.Scripting.Compilation
{
    public class LightScriptCompilatorShould
    {
        private LightScriptCompilationOptions defaultCompilationOptions;

        private (GameObject fixtures, DmxFixture parLed1, DmxFixture parLed2) CreateSimpleLightingPlan()
        {
            GameObject fixtures = new("Fixtures");

            FixtureDefinition parLedFixtureDefinition = new()
            {
                name = "Par Led Test",
                manufacturer = "test",
                mode = "6CH",
                chanCount = 6,
                channels = new DmxChannelDefinition[]
                {
                    new() { channel = 0, type = DmxChannelType.Color },
                    new() { channel = 3, type = DmxChannelType.White },
                    new() { channel = 4, type = DmxChannelType.Dimmer },
                    new() { channel = 5, type = DmxChannelType.Stroboscope },
                }
            };

            fixtures.AddChild("Par Led 1")
                .WithComponent(pr =>
                {
                    pr.model = parLedFixtureDefinition;
                    pr.channelOffset = 1;
                },out DmxFixture parLed1);

            fixtures.AddChild("Par Led 2")
                .WithComponent(pr =>
                {
                    pr.model = parLedFixtureDefinition;
                    pr.channelOffset = 10;
                }, out DmxFixture parLed2);

            return (fixtures, parLed1, parLed2);
        }

        private (DmxTrack track, DmxTrackElement parLed1TrackElement, DmxTrackElement parLed2TrackElement) CreateSimpleLightingPlanTrackElements()
        {
            (_, DmxFixture parLed1Fixture, DmxFixture parLed2Fixture) = CreateSimpleLightingPlan();

            new GameObject("Track").WithComponent<DmxTrack>(out var track);

            var parLed1TrackElement = track.AddElement(parLed1Fixture);
            var parLed2TrackElement = track.AddElement(parLed2Fixture);

            return (track, parLed1TrackElement, parLed2TrackElement);
        }

        [Test]
        public void Compile_Without_Errors_For__Set_Dimmer_To_A_Constant_Value()
        {
            (_, DmxTrackElement parLed, _) = CreateSimpleLightingPlanTrackElements();

            LightScriptData data = new()
            {
                text = "parLed.dimmer = 120",
                fixtures = new[]
                {
                    parLed
                }
            };
            LightScriptCompilationOptions options = defaultCompilationOptions;

            LightScriptCompilationResult result = LightScriptCompilation.Compile(data, options);

            Assert.IsTrue(result.isOk);
            Assert.IsFalse(result.hasError);
        }
    }
}
