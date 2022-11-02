using Plml.Dmx.Scripting.Types;
using System.Collections.Generic;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal abstract class BinaryOperatorNode : SyntaxNode<BinaryOperatorNode>
    {
        public BinaryOperatorType Operator { get; }

        public SyntaxNode LeftHandSide { get; }
        public SyntaxNode RightHandSide { get; }

        protected BinaryOperatorNode(BinaryOperatorType @operator, LightScriptType resultType, SyntaxNode lhs, SyntaxNode rhs) :
            base(resultType)
        {
            Operator = @operator;
            LeftHandSide = lhs;
            RightHandSide = rhs;
        }

        protected BinaryOperatorNode(BinaryOperatorType @operator, SyntaxNode lhs, SyntaxNode rhs)
        {
            Type = @operator.GetOperatorResultType(lhs.Type, rhs.Type);
            Operator = @operator;
            LeftHandSide = lhs;
            RightHandSide= rhs;
        }

        protected override bool Equals(BinaryOperatorNode other) => Operator == other.Operator
                                                                && LeftHandSide == other.LeftHandSide
                                                                && RightHandSide == other.RightHandSide;

        public override IEnumerable<SyntaxNode> EnumerateChildren() => Enumerables.Create(
            LeftHandSide,
            RightHandSide
        );
    }

    internal class AssignmentNode : BinaryOperatorNode
    {
        public AssignmentNode(LightScriptType resultType, SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Assignment, resultType, lhs, rhs) { }
        public AssignmentNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Assignment, lhs, rhs) { }

        public override SyntaxNode Clone() => new AssignmentNode(Type, LeftHandSide.Clone(), RightHandSide.Clone());
    }

    internal class AdditionNode : BinaryOperatorNode
    {
        public AdditionNode(LightScriptType resultType, SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Addition, resultType, lhs, rhs) { }
        public AdditionNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Addition, lhs, rhs) { }

        public override SyntaxNode Clone() => new AdditionNode(Type, LeftHandSide.Clone(), RightHandSide.Clone());
    }

    internal class SubstractionNode : BinaryOperatorNode
    {
        public SubstractionNode(LightScriptType resultType, SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Substraction, resultType, lhs, rhs) { }
        public SubstractionNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Substraction, lhs, rhs) { }

        public override SyntaxNode Clone() => new SubstractionNode(Type, LeftHandSide.Clone(), RightHandSide.Clone());
    }

    internal class MultiplicationNode : BinaryOperatorNode
    {
        public MultiplicationNode(LightScriptType resultType, SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Multiplication, resultType, lhs, rhs) { }
        public MultiplicationNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Multiplication, lhs, rhs) { }

        public override SyntaxNode Clone() => new MultiplicationNode(Type, LeftHandSide.Clone(), RightHandSide.Clone());
    }

    internal class DivisionNode : BinaryOperatorNode
    {
        public DivisionNode(LightScriptType resultType, SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Division, resultType, lhs, rhs) { }
        public DivisionNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Division, lhs, rhs) { }

        public override SyntaxNode Clone() => new DivisionNode(Type, LeftHandSide.Clone(), RightHandSide.Clone());
    }

    internal class ModuloNode : BinaryOperatorNode
    {
        public ModuloNode(LightScriptType resultType, SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Modulo, resultType, lhs, rhs) { }
        public ModuloNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Modulo, lhs, rhs) { }

        public override SyntaxNode Clone() => new ModuloNode(Type, LeftHandSide.Clone(), RightHandSide.Clone());
    }

    internal class ExponentiationNode : BinaryOperatorNode
    {
        public ExponentiationNode(LightScriptType resultType, SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Exponentiation, resultType, lhs, rhs) { }
        public ExponentiationNode(SyntaxNode lhs, SyntaxNode rhs) : base(BinaryOperatorType.Exponentiation, lhs, rhs) { }

        public override SyntaxNode Clone() => new ExponentiationNode(Type, LeftHandSide.Clone(), RightHandSide.Clone());
    }

    internal enum BinaryOperatorType
    {
        Assignment,
        Addition,
        Substraction,
        Multiplication,
        Division,
        Modulo,
        Exponentiation
    }
}