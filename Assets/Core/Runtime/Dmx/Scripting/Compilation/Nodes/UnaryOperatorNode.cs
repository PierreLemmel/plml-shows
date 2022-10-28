using System.Collections.Generic;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal abstract class UnaryOperatorNode : SyntaxNode<UnaryOperatorNode>
    {
        public UnaryOperatorType Operator { get; }

        public SyntaxNode Operand { get; }

        public UnaryOperatorNode(UnaryOperatorType @operator, SyntaxNode operand) : base(operand.Type)
        {
            Operator = @operator;
            Operand = operand;
        }

        protected override bool Equals(UnaryOperatorNode other) => Operand == other.Operand;

        public override IEnumerable<SyntaxNode> EnumerateChildren() => Enumerables.Create(Operand);
    }

    internal class UnaryPlusNode : UnaryOperatorNode
    {
        public UnaryPlusNode(SyntaxNode operand) : base(UnaryOperatorType.Plus, operand) { }

        public override SyntaxNode Clone() => new UnaryPlusNode(Operand.Clone());
    }

    internal class UnaryMinusNode : UnaryOperatorNode
    {
        public UnaryMinusNode(SyntaxNode operand) : base(UnaryOperatorType.Minus, operand) { }

        public override SyntaxNode Clone() => new UnaryMinusNode(Operand.Clone());
    }

    internal enum UnaryOperatorType
    {
        Plus,
        Minus,
    }
}