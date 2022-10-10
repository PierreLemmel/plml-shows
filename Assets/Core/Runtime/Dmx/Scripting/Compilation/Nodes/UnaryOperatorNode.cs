using System;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class UnaryOperatorNode : SyntaxNode
    {
        public SyntaxNode Operand { get; }

        public UnaryOperatorNode(SyntaxNode operand)
        {
            Operand = operand;
        }
    }
}