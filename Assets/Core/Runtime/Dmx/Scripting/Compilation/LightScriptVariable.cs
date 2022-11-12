using Plml.Dmx.Scripting.Types;

namespace Plml.Dmx.Scripting.Compilation
{
    public class LightScriptVariable
    {
        public LightScriptType Type { get; }
        public string Name { get; }
        public bool IsReadOnly { get; }

        public LightScriptVariable(LightScriptType type, string name, bool isReadOnly = false)
        {
            Type = type;
            Name = name;
            IsReadOnly = isReadOnly;
        }
    }
}