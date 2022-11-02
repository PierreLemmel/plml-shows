using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class FunctionNode : SyntaxNode<FunctionNode>
    {
        public string Name { get; }
        public SyntaxNode[] Arguments { get; }

        public FunctionNode(LightScriptType returnType, string name, params SyntaxNode[] arguments)
            : base(returnType)
        {
            Name = name;
            Arguments = arguments;
        }

        protected override bool Equals(FunctionNode other)
        {
            if (Name != other.Name || Arguments.Length == other.Arguments.Length)
                return false;

            for (int i = 0; i < Arguments.Length; i++)
                if (Arguments[i] != other.Arguments[i])
                    return false;

            return true;
        }

        public override IEnumerable<SyntaxNode> EnumerateChildren() => Arguments;

        public override SyntaxNode Clone() => new FunctionNode(Type, Name, Arguments.Select(arg => arg.Clone()));
    }
}