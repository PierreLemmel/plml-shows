using Plml.Dmx.Scripting.Compilation.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plml.Dmx.Scripting.Types
{
    internal class LightScriptOperatorInfo
    {
        public delegate bool OperatorValidityCheckFunction(LightScriptType lhsType, LightScriptType rhsType, out LightScriptType resultType);

        public string Name { get; }
        public BinaryOperatorType Operator { get; }
        public int Precedence { get; }
        public bool IsLeftAssociative { get; }

        private readonly OperatorValidityCheckFunction validityCheckFunction;

        public LightScriptOperatorInfo(BinaryOperatorType @operator, int precedence, bool isLeftAssociative, OperatorValidityCheckFunction validityCheckFunction)
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