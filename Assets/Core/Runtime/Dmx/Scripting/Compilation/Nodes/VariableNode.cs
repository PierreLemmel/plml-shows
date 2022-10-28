using System.Collections.Generic;
using System.Linq;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class VariableNode : SyntaxNode<VariableNode>
    {
        public string Name { get; }

        public VariableNode(LightScriptType type, string name) : base(type) => Name = name;

        protected override bool Equals(VariableNode other) => Type == other.Type
                                                            && Name == other.Name;

        public override IEnumerable<SyntaxNode> EnumerateChildren() => Enumerable.Empty<SyntaxNode>();

        public override SyntaxNode Clone() => new VariableNode(Type, Name);
    }
}