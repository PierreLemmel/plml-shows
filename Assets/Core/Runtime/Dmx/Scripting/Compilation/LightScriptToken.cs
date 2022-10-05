namespace Plml.Dmx.Scripting.Compilation
{
    internal struct LightScriptToken
    {
        public LightScriptTokenType type;
        public string content;

        public LightScriptToken(LightScriptTokenType type, string content)
        {
            this.type = type;
            this.content = content;
        }

        public LightScriptToken(LightScriptTokenType type) : this(type, "") { }

        public override string ToString() => $"{type}: {content}";
    }
}