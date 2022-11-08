using Plml.Dmx.Scripting.Compilation.Nodes;

namespace Plml.Dmx.Scripting.Types
{
    internal class LightScriptBinaryOperatorInfo
    {
        public delegate bool OperatorValidityCheckFunction(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType);

        public string Name { get; }
        public BinaryOperatorType Operator { get; }
        public int Precedence { get; }
        public bool IsLeftAssociative { get; }

        private readonly OperatorValidityCheckFunction validityCheckFunction;

        public LightScriptBinaryOperatorInfo(BinaryOperatorType @operator, int precedence, bool isLeftAssociative, OperatorValidityCheckFunction validityCheckFunction)
        {
            Name = @operator.ToString();
            Operator = @operator;
            Precedence = precedence;
            IsLeftAssociative = isLeftAssociative;

            this.validityCheckFunction = validityCheckFunction;
        }

        public bool IsValidOperatorType(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType) => validityCheckFunction(lhsType, rhsType, out resultType);
    }
}