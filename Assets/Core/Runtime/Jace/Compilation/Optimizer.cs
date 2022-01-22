using Plml.Jace.Operations;
using Plml.Jace.Execution;
using Plml.Jace.Compilation;

namespace Plml.Jace.Compilation
{
    public class Optimizer
    {
        private readonly IExecutor executor;

        public Optimizer(IExecutor executor)
        {
            this.executor = executor;
        }

        public Operation Optimize(Operation operation, IFunctionRegistry functionRegistry)
        {
            if (!operation.DependsOnVariables && !(operation is FloatingPointConstant))
            {
                float result = executor.Execute(operation, functionRegistry);
                return new FloatingPointConstant(result);
            }
            else
            {
                switch (operation)
                {
                    case Addition addition:
                        addition.Argument1 = Optimize(addition.Argument1, functionRegistry);
                        addition.Argument2 = Optimize(addition.Argument2, functionRegistry);
                        break;
                    case Substraction substraction:
                        substraction.Argument1 = Optimize(substraction.Argument1, functionRegistry);
                        substraction.Argument2 = Optimize(substraction.Argument2, functionRegistry);
                        break;
                    case Multiplication multiplication:
                        multiplication.Argument1 = Optimize(multiplication.Argument1, functionRegistry);
                        multiplication.Argument2 = Optimize(multiplication.Argument2, functionRegistry);
                        break;
                    case Division division:
                        division.Dividend = Optimize(division.Dividend, functionRegistry);
                        division.Divisor = Optimize(division.Divisor, functionRegistry);
                        break;
                    case Exponentiation exponentiation:
                        exponentiation.Base = Optimize(exponentiation.Base, functionRegistry);
                        exponentiation.Exponent = Optimize(exponentiation.Exponent, functionRegistry);
                        break;
                }

                return operation;
            }
        }
    }
}