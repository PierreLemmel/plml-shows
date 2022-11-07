using System.Collections.Generic;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class FunctionNode : SyntaxNode<FunctionNode>
    {
        public string Name => Function.Name;
        public LightScriptFunction Function { get; }
        public SyntaxNode[] Arguments { get; }

        public FunctionNode(LightScriptFunction function, params SyntaxNode[] arguments)
            : base(function.ReturnType)
        {
            Function = function;
            Arguments = arguments;
        }

        protected override bool Equals(FunctionNode other)
        {
            if (Name != other.Name || Arguments.Length != other.Arguments.Length)
                return false;

            for (int i = 0; i < Arguments.Length; i++)
                if (Arguments[i] != other.Arguments[i])
                    return false;

            return true;
        }

        public override IEnumerable<SyntaxNode> EnumerateChildren() => Arguments;

        public override SyntaxNode Clone() => new FunctionNode(Function, Arguments.Select(arg => arg.Clone()));
    }
}