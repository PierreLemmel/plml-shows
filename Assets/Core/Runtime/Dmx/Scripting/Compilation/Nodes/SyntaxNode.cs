using UnityEngine;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal abstract class SyntaxNode
    {
        public static bool operator ==(SyntaxNode lhs, SyntaxNode rhs) => (lhs is null && rhs is null)
                                                                        || (lhs?.Equals(rhs) ?? false);

        public static bool operator !=(SyntaxNode lhs, SyntaxNode rhs) => !(lhs == rhs);

        public override bool Equals(object other) => base.Equals(other);

        public override int GetHashCode() => base.GetHashCode();
    }

    internal abstract class SyntaxNode<TNode> : SyntaxNode where TNode : SyntaxNode<TNode>
    {
        public override bool Equals(object other) => (other is TNode otherNode) && Equals(otherNode);
        public override int GetHashCode() => base.GetHashCode();

        protected abstract bool Equals(TNode other);
    }
}