using System;

namespace Plml.Dmx.Scripting.Runtime
{
    [Serializable]
    public class FloatVariableInfo : VariableInfo<float>
    {
        public bool hasBounds;
        public float minValue;
        public float maxValue;

        public FloatVariableInfo() : base() { }
        public FloatVariableInfo(string name, float defaultValue = 0, bool isReadonly = false,
            bool hasBounds = false, float minValue = 0f, float maxValue = 100f
        ) :
            base(name, defaultValue, isReadonly)
        {
            this.hasBounds = hasBounds;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}