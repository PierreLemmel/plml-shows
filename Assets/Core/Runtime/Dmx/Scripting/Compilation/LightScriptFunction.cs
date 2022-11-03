namespace Plml.Dmx.Scripting.Compilation
{
    public class LightScriptFunction
    {
        public LightScriptType ReturnType { get; }
        public string Name { get; }
        public LightScriptType[] Arguments { get; }

        public LightScriptFunction(LightScriptType type, string name, params LightScriptType[] arguments)
        {
            ReturnType = type;
            Name = name;
            Arguments = arguments.ShallowCopy();
        }
    }
}