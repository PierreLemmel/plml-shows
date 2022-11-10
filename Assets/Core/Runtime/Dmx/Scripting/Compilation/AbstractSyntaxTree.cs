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
    }
}