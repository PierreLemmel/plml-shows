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
    }

    internal abstract class SyntaxNode<TNode> : SyntaxNode where TNode : SyntaxNode<TNode>
    {
        public override bool Equals(object other) => (other is TNode otherNode) && otherNode.Type == Type && Equals(otherNode);
        public override int GetHashCode() => base.GetHashCode();

        protected SyntaxNode() { }
        protected SyntaxNode(LightScriptType type) : base(type) { }

        protected abstract bool Equals(TNode other);
    }
}