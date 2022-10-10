using System;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class ConstantNode : SyntaxNode
    {
        public object Value { get; }

        public ConstantNode(object value) => Value = value;
    }
}