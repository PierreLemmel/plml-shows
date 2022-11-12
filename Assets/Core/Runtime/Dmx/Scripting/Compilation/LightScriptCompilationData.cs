using Plml.Dmx.Scripting.Runtime;
using System;

namespace Plml.Dmx.Scripting.Compilation
{
    public class LightScriptCompilationData
    {
        public FixtureVariableInfo[] fixtures = Array.Empty<FixtureVariableInfo>();
        public IntegerVariableInfo[] integers = Array.Empty<IntegerVariableInfo>();
        public FloatVariableInfo[] floats = Array.Empty<FloatVariableInfo>();
        public ColorVariableInfo[] colors = Array.Empty<ColorVariableInfo>();

        public string text;
    }
}