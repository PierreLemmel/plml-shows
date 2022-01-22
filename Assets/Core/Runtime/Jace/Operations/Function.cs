using System.Collections.Generic;
using System.Linq;

namespace Plml.Jace.Operations
{
    public class Function : Operation
    {
        public Function(string functionName, IList<Operation> arguments)
            : base(arguments.FirstOrDefault(o => o.DependsOnVariables) != null)
        {
            this.FunctionName = functionName;
            this.Arguments = arguments;
        }

        public string FunctionName { get; private set; }

        public IList<Operation> Arguments { get; private set; }
    }
}
