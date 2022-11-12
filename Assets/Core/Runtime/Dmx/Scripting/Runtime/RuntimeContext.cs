using System.Collections.Generic;

namespace Plml.Dmx.Scripting.Runtime
{
    public class RuntimeContext : ILightScriptContext
    {
        public void AddToContext(FixtureVariableInfo fixtureVar) => Fixtures.Add(fixtureVar.name, new(fixtureVar, fixtureVar.fixture));
        public void AddToContext(IntegerVariableInfo intVar) => Integers.Add(intVar.name, new(intVar, intVar.defaultValue));
        public void AddToContext(FloatVariableInfo floatVar) => Floats.Add(floatVar.name, new(floatVar, floatVar.defaultValue));
        public void AddToContext(ColorVariableInfo colorVar) => Colors.Add(colorVar.name, new(colorVar, colorVar.defaultValue));

        public IDictionary<string, FixtureVariable> Fixtures { get; } = new Dictionary<string, FixtureVariable>();
        public IDictionary<string, IntVariable> Integers { get; } = new Dictionary<string, IntVariable>();
        public IDictionary<string, FloatVariable> Floats { get; } = new Dictionary<string, FloatVariable>();
        public IDictionary<string, ColorVariable> Colors { get; } = new Dictionary<string, ColorVariable>();
    }
}