using System;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class BinaryOperatorNode : SyntaxNode
    {
        public SyntaxNode LeftHandSide { get; }
        public SyntaxNode RightHandSide { get; }

        public BinaryOperatorNode(SyntaxNode lhs, SyntaxNode rhs)
        {
            LeftHandSide = lhs;
            RightHandSide = rhs;
        }
    }
}