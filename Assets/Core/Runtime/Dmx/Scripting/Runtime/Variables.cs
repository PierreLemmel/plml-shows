namespace Plml.Dmx.Scripting.Runtime
{
    public class Variable<TInfo, TValue> where TInfo : VariableInfo
    {
        public TInfo Info { get; }
        public TValue Value { get; set; }

        public Variable(TInfo info, TValue value)
        {
            Info = info;
            Value = value;
        }
    }

    public class FixtureVariable : Variable<FixtureVariableInfo, DmxTrackElement>
    {
        public FixtureVariable(FixtureVariableInfo info, DmxTrackElement value) : base(info, value)
        {
        }
    }

    public class IntVariable : Variable<IntegerVariableInfo, int>
    {
        public IntVariable(IntegerVariableInfo info, int value) : base(info, value)
        {
        }
    }

    public class FloatVariable : Variable<FloatVariableInfo, float>
    {
        public FloatVariable(FloatVariableInfo info, float value) : base(info, value)
        {
        }
    }

    public class ColorVariable : Variable<ColorVariableInfo, Color24>
    {
        public ColorVariable(ColorVariableInfo info, Color24 value) : base(info, value)
        {
        }
    }
}