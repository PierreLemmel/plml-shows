using Plml.Dmx.Scripting.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal abstract class SyntaxNode
    {
        public LightScriptType Type { get; protected set; } = LightScriptType.Undefined;

        protected SyntaxNode() { }
        protected SyntaxNode(LightScriptType type) => Type = type;

        public static bool operator ==(SyntaxNode lhs, SyntaxNode rhs) => (lhs is null && rhs is null)
                                                                        || (lhs?.Equals(rhs) ?? false);

        public static bool operator !=(SyntaxNode lhs, SyntaxNode rhs) => !(lhs == rhs);

        public override bool Equals(object other) => base.Equals(other);

        public override int GetHashCode() => base.GetHashCode();

        public abstract IEnumerable<SyntaxNode> EnumerateChildren();

        public abstract SyntaxNode Clone();

        public override string ToString() => this.Stringify();
    }

    internal abstract class SyntaxNode<TNode> : SyntaxNode where TNode : SyntaxNode<TNode>
    {
        public override bool Equals(object other) => (other is TNode otherNode) && otherNode.Type == Type && Equals(otherNode);
        public override int GetHashCode() => base.GetHashCode();

        protected SyntaxNode() { }
        protected SyntaxNode(LightScriptType type) : base(type) { }

        protected abstract bool Equals(TNode other);
    }

    internal static class SyntaxNodes
    {
        public static string Stringify(this SyntaxNode node)
        {
            return StringifyNode(node, 0);

            string StringifyNode(SyntaxNode node, int indent)
            {
                string str = node switch
                {
                    VariableNode variable => $"{variable.Type} {variable.Name}",
                    ConstantNode constant => constant.Value.ToString(),
                    MemberAccessNode ma => string.Join("\n", new string[]
                    {
                        $"Member access",
                        StringifyNode(ma.Target, indent + 1),
                        IndentString(indent + 1) + ma.Property,
                        IndentString(indent) + ")"
                    }),
                    UnaryOperatorNode unary => string.Join("\n", new string[]
                    {
                        $"{unary.Operator} ({unary.Type})",
                        StringifyNode(unary.Operand, indent + 1)
                    }),
                    BinaryOperatorNode binary => string.Join("\n", new string[]
                    {
                        $"{binary.Operator} ({binary.Type})",
                        StringifyNode(binary.LeftHandSide, indent + 1),
                        StringifyNode(binary.RightHandSide, indent + 1),
                    }),
                    FunctionNode function => string.Join("\n", Enumerables.Merge(
                        Enumerables.Create(function.Name),
                        function.Arguments.Select(arg => StringifyNode(arg, indent + 1))
                    )),
                    ImplicitConversionNode @implicit => string.Join("\n", new string[]
                    {
                        $"Implicit: {@implicit.Target.Type} -> {@implicit.ToType}",
                        StringifyNode(@implicit.Target, indent + 1),
                    }),
                    ExplicitConversionNode @explicit => string.Join("\n", new string[]
                    {
                        $"Explicit: {@explicit.Target.Type} -> {@explicit.ToType}",
                        StringifyNode(@explicit.Target, indent + 1)
                    }),
                    _ => throw new InvalidOperationException($"Unsupported node type: {node.GetType().Name}")
                };

                return IndentString(indent) + str;
            }

            string IndentString(int indent) => "    ".Repeat(indent);
        }
    }
}