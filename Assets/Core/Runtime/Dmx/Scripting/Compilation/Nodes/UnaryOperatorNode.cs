namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal abstract class UnaryOperatorNode : SyntaxNode<UnaryOperatorNode>
    {
        public UnaryOperatorType Operator { get; }

        public SyntaxNode Operand { get; }

        public UnaryOperatorNode(UnaryOperatorType @operator, SyntaxNode operand)
        {
            Operator = @operator;
            Operand = operand;
        }

        protected override bool Equals(UnaryOperatorNode other) => Operand == other.Operand;
    }

    internal class UnaryPlusNode : UnaryOperatorNode
    {
        public UnaryPlusNode(SyntaxNode operand) : base(UnaryOperatorType.Plus, operand) { }
    }

    internal class UnaryMinusNode : UnaryOperatorNode
    {
        public UnaryMinusNode(SyntaxNode operand) : base(UnaryOperatorType.Minus, operand) { }
    }

    internal enum UnaryOperatorType
    {
        Plus,
        Minus,
    }
}