using System;

namespace Plml.Dmx.Scripting.Runtime
{
    [Serializable]
    public class ColorVariableInfo : VariableInfo<Color24>
    {
        public ColorVariableInfo() : base() { }
        public ColorVariableInfo(string name, Color24 defaultValue = new(), bool isReadonly = false) :
            base(name, defaultValue, isReadonly)
        { }
    }
}