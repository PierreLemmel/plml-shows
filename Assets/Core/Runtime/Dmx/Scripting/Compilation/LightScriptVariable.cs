namespace Plml.Dmx.Scripting.Compilation
{
    public class LightScriptVariable
    {
        public LightScriptType Type { get; }
        public string Name { get; }

        public LightScriptVariable(LightScriptType type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}