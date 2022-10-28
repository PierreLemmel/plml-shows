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

    internal enum BinaryOperatorType
    {
        Assignment,
        Addition,
        Substraction,
        Multiplication,
        Division
    }

    internal static class BinaryOperators
    {
        public static int GetPrecedence(this BinaryOperatorType @operator) => @operator switch
        {
            BinaryOperatorType.Assignment => 0,

            BinaryOperatorType.Addition => 10,
            BinaryOperatorType.Substraction => 10,

            BinaryOperatorType.Multiplication => 20,
            BinaryOperatorType.Division => 20,

            _ => throw new LightScriptException($"Unsupported operator '{@operator}'")
        };
    }
}