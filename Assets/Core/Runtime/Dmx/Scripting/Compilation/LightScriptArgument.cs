using Plml.Dmx.Scripting.Types;

namespace Plml.Dmx.Scripting.Compilation
{
    public class LightScriptArgument
    {
        public LightScriptType Type { get; }
        public bool IsParams { get; }

        public LightScriptArgument(LightScriptType type, bool isParams = false)
        {
            Type = type;
            IsParams = isParams;
        }
    }
}