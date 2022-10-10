namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal abstract class BinaryOperatorNode : SyntaxNode<BinaryOperatorNode>
    {
        public BinaryOperatorType Operator { get; }

        public SyntaxNode LeftHandSide { get; }
        public SyntaxNode RightHandSide { get; }

        protected BinaryOperatorNode(BinaryOperatorType @operator, SyntaxNode lhs, SyntaxNode rhs)
        {
            Operator = @operator;
            LeftHandSide = lhs;
            RightHandSide = rhs;
        }

        protected override bool Equals(BinaryOperatorNode other) => Operator == other.Operator
                                                                && LeftHandSide == other.LeftHandSide
                                                                && RightHandSide == other.RightHandSide;
    }

    internal class AssignmentNode : BinaryOperatorNode
    {
        public AssignmentNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Assignment, lhs, rhs) { }
    }

    internal class AdditionNode : BinaryOperatorNode
    {
        public AdditionNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Addition, lhs, rhs) { }
    }

    internal class SubstractionNode : BinaryOperatorNode
    {
        public SubstractionNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Substraction, lhs, rhs) { }
    }

    internal class MultiplicationNode : BinaryOperatorNode
    {
        public MultiplicationNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Multiplication, lhs, rhs) { }
    }

    internal class DivisionNode : BinaryOperatorNode
    {
        public DivisionNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Division, lhs, rhs) { }
    }

    internal enum BinaryOperatorType
    {
        Assignment,
        Addition,
        Substraction,
        Multiplication,
        Division
    }
}