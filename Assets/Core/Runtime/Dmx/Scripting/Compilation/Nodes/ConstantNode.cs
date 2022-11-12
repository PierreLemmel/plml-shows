using Plml.Dmx.Scripting.Types;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Dmx.Scripting.Compilation.Nodes
{
    internal class ConstantNode : SyntaxNode<ConstantNode>
    {
        public object Value { get; }

        public int IntValue => (int)Value;
        public float FloatValue => (float)Value;
        public Color24 Color24Value => (Color24)Value;

        private ConstantNode(LightScriptType type, object value) : base(type) => Value = value;

        public ConstantNode(int value) : this(LightScriptType.Integer, value) { }
        public ConstantNode(float value) : this(LightScriptType.Float, value) { }
        public ConstantNode(Color24 value) : this(LightScriptType.Color, value) { }

        protected override bool Equals(ConstantNode other) => Type == other.Type
                                                    && Equals(Value, other.Value);

        public override IEnumerable<SyntaxNode> EnumerateChildren() => Enumerable.Empty<SyntaxNode>();

        public override SyntaxNode Clone() => new ConstantNode(Type, Value);
    }
}