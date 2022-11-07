using Plml.Dmx.Scripting.Compilation.Nodes;
using System;
using System.Linq;

namespace Plml.Dmx.Scripting.Compilation
{
    internal class AbstractSyntaxTree
    {
        public SyntaxNode[] Statements { get; }

        public AbstractSyntaxTree(params SyntaxNode[] statements) => Statements = statements.ToArray();

        public override bool Equals(object obj)
        {
            if (obj is AbstractSyntaxTree ast)
            {
                if (Statements.Length != ast.Statements.Length)
                    return false;

                return Statements
                    .Zip(ast.Statements, (lhs, rhs) => (lhs, rhs))
                    .All(tuple => tuple.lhs == tuple.rhs);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;
                foreach (SyntaxNode node in Statements)
                    result = (397 * result) ^ node.GetHashCode();
                return result;
            }
        }
    }

    internal static class SyntaxTreeExtensions
    {
        public static string Stringify(this AbstractSyntaxTree tree) => string.Join(
            "\n",
            tree.Statements.Select(statement => statement.Stringify())
        );

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
                        $"{unary.Operator}",
                        StringifyNode(unary.Operand, indent + 1)
                    }),
                    BinaryOperatorNode binary => string.Join("\n", new string[]
                    {
                        $"{binary.Operator}",
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