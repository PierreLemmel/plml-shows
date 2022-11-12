using System;

namespace Plml.Dmx.Scripting.Runtime
{
    [Serializable]
    public class IntegerVariableInfo : VariableInfo<int>
    {
        public bool hasBounds = true;
        public int minValue = 0;
        public int maxValue = 255;

        public IntegerVariableInfo() : base() { }
        public IntegerVariableInfo(string name, int defaultValue = 0, bool isReadonly = false,
            bool hasBounds = true, int minValue = 0, int maxValue = 255
        ) :
            base(name, defaultValue, isReadonly)
        {
            this.hasBounds = hasBounds;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}