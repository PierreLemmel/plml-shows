using System;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class MemberAccessNode : SyntaxNode
    {
        public SyntaxNode Object { get; }
        public SyntaxNode Property { get; }

        public MemberAccessNode(SyntaxNode @object, SyntaxNode property)
        {
            Object = @object;
            Property = property;
        }
    }
}
