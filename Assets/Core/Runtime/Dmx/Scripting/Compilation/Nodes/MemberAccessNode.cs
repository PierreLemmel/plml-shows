using System;
using System.Collections.Generic;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class MemberAccessNode : SyntaxNode<MemberAccessNode>
    {
        public SyntaxNode Target { get; }
        public string Property { get; }

        public MemberAccessNode(SyntaxNode target, string property)
        {
            Target = target;
            Property = property;
        }

        protected override bool Equals(MemberAccessNode other) => Target == other.Target && Property == other.Property;

        public override IEnumerable<SyntaxNode> EnumerateChildren() => Enumerables.Create(Target);
    }
}
