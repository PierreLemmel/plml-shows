using Plml.Dmx.Scripting.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal abstract class ConversionNode : SyntaxNode<ConversionNode>
    {
        public SyntaxNode Target { get; }

        public LightScriptType ToType { get; }

        public ConversionNode(SyntaxNode target, LightScriptType toType) : base(toType)
        {
            Target = target;
            ToType = toType;
        }

        protected override bool Equals(ConversionNode other) => Target == other.Target
            && ToType == other.ToType;

        public override IEnumerable<SyntaxNode> EnumerateChildren() => Enumerables.Create(Target);
    }

    internal class ImplicitConversionNode : ConversionNode
    {
        public ImplicitConversionNode(SyntaxNode target, LightScriptType toType) : base(target, toType)
        {
            if (!LightScriptTypeSystem.HasImplicitConversion(Target.Type, toType))
                throw new LightScriptException($"There is no implicit conversion between '{Target.Type}' and '{toType}'");
        }

        public override SyntaxNode Clone() => new ImplicitConversionNode(Target, ToType);
    }

    internal class ExplicitConversionNode : ConversionNode
    {
        public ExplicitConversionNode(SyntaxNode target, LightScriptType toType) : base(target, toType)
        {
            if (!LightScriptTypeSystem.HasExplicitConversion(Target.Type, toType))
                throw new LightScriptException($"There is no explicit conversion between '{Target.Type}' and '{toType}'");
        }

        public override SyntaxNode Clone() => new ExplicitConversionNode(Target, ToType);
    }
}