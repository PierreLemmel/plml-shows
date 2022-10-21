using Plml.Dmx.Scripting.Types;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class VariableNode : SyntaxNode<VariableNode>
    {
        public LightScriptType Type { get; }
        public string Name { get; }

        public VariableNode(LightScriptType type, string name)
        {
            Type = type;
            Name = name;
        }

        protected override bool Equals(VariableNode other) => Type == other.Type
                                                            && Name == other.Name;
    }

}