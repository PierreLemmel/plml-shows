using Plml.Dmx.Scripting.Types;

namespace Plml.Dmx.Scripting.Compilation
{
    public class LightScriptConstant
    {
        public LightScriptType Type { get; }
        public string Name { get; }

        public object Value { get; }

        public int IntValue => (int)Value;
        public float FloatValue => (float)Value;
        public Color24 ColorValue => (Color24)Value;

        private LightScriptConstant(LightScriptType type, string name, object value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public LightScriptConstant(string name, int value) : this(LightScriptType.Integer, name, value) { }
        public LightScriptConstant(string name, float value) : this(LightScriptType.Float, name, value) { }
        public LightScriptConstant(string name, Color24 value) : this(LightScriptType.Color, name, value) { }
    }
}