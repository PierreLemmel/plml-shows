using System;
using System.Collections.Generic;
using Plml.Jace.Compilation;
using Plml.Jace.Operations;

namespace Plml.Jace.Execution
{
    public interface IExecutor
    {
        float Execute(Operation operation, IFunctionRegistry functionRegistry);
        float Execute(Operation operation, IFunctionRegistry functionRegistry, IDictionary<string, float> variables);

        Func<IDictionary<string, float>, float> BuildFormula(Operation operation, IFunctionRegistry functionRegistry);
    }
}
