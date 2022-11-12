using Plml.Dmx.Scripting.Runtime;
using System.Collections.Generic;

namespace Plml.Dmx.Scripting
{
    public interface ILightScriptContext
    {
        public void AddToContext(FixtureVariableInfo fixture);
        public void AddToContext(IntegerVariableInfo intValue);
        public void AddToContext(FloatVariableInfo floatValue);
        public void AddToContext(ColorVariableInfo color);

        public IDictionary<string, FixtureVariable> Fixtures { get; }
        public IDictionary<string, IntVariable> Integers { get; }
        public IDictionary<string, FloatVariable> Floats { get; }
        public IDictionary<string, ColorVariable> Colors { get; }
    }
}