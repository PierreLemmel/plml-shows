using System;
using System.Collections.Generic;
using System.Linq;
using Plml.Jace.Compilation;
using Plml.Jace.Maths;
using Plml.Jace.Operations;
using Plml.Jace.Parsing;

namespace Plml.Jace.Execution
{
    public class Interpreter : IExecutor
    {
        public Func<IDictionary<string, float>, float> BuildFormula(Operation operation, 
            IFunctionRegistry functionRegistry)
        {
            return variables => Execute(operation, functionRegistry, variables);
        }

        public float Execute(Operation operation, IFunctionRegistry functionRegistry)
        {
            return Execute(operation, functionRegistry, new Dictionary<string, float>());
        }

        public float Execute(Operation operation, IFunctionRegistry functionRegistry, 
            IDictionary<string, float> variables)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            switch(operation)
            {
                case FloatingPointConstant constant:
                    return constant.Value;
                case Variable variable:
                    bool variableFound = variables.TryGetValue(variable.Name, out float value);

                    if (variableFound)
                        return value;
                    else
                        throw new VariableNotDefinedException($"The variable \"{variable.Name}\" used is not defined.");
                case Multiplication multiplication:
                    return Execute(multiplication.Argument1, functionRegistry, variables) * Execute(multiplication.Argument2, functionRegistry, variables);

                case Addition addition:
                    return Execute(addition.Argument1, functionRegistry, variables) + Execute(addition.Argument2, functionRegistry, variables);

                case Substraction addition:
                    return Execute(addition.Argument1, functionRegistry, variables) - Execute(addition.Argument2, functionRegistry, variables);

                case Division division:
                    return Execute(division.Dividend, functionRegistry, variables) / Execute(division.Divisor, functionRegistry, variables);

                case Modulo division:
                    return Execute(division.Dividend, functionRegistry, variables) % Execute(division.Divisor, functionRegistry, variables);

                case Exponentiation exponentiation:
                    return Mathf.Pow(Execute(exponentiation.Base, functionRegistry, variables), Execute(exponentiation.Exponent, functionRegistry, variables));

                case UnaryMinus unaryMinus:
                    return -Execute(unaryMinus.Argument, functionRegistry, variables);

                case And and:
                    var andLhs = Execute(and.Argument1, functionRegistry, variables) != 0;
                    var andRhs = Execute(and.Argument2, functionRegistry, variables) != 0;

                    return (andLhs && andRhs) ? 1.0f : 0.0f;

                case Or or:
                    var orLhs = Execute(or.Argument1, functionRegistry, variables) != 0;
                    var orRhs = Execute(or.Argument2, functionRegistry, variables) != 0;

                    return (orLhs || orRhs) ? 1.0f : 0.0f;

                case LessThan lessThan:
                    return (Execute(lessThan.Argument1, functionRegistry, variables) < Execute(lessThan.Argument2, functionRegistry, variables)) ? 1.0f : 0.0f;

                case LessOrEqualThan lessOrEqualThan:
                    return (Execute(lessOrEqualThan.Argument1, functionRegistry, variables) <= Execute(lessOrEqualThan.Argument2, functionRegistry, variables)) ? 1.0f : 0.0f;

                case GreaterThan greaterThan:
                    return (Execute(greaterThan.Argument1, functionRegistry, variables) > Execute(greaterThan.Argument2, functionRegistry, variables)) ? 1.0f : 0.0f;

                case GreaterOrEqualThan greaterOrEqualThan:
                    return (Execute(greaterOrEqualThan.Argument1, functionRegistry, variables) >= Execute(greaterOrEqualThan.Argument2, functionRegistry, variables)) ? 1.0f : 0.0f;

                case Equal equal:
                    return (Execute(equal.Argument1, functionRegistry, variables) == Execute(equal.Argument2, functionRegistry, variables)) ? 1.0f : 0.0f;

                case NotEqual notEqual:
                    return (Execute(notEqual.Argument1, functionRegistry, variables) != Execute(notEqual.Argument2, functionRegistry, variables)) ? 1.0f : 0.0f;

                case Function function:

                    FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);

                    float[] arguments = new float[functionInfo.NumberOfParameters];
                    for (int i = 0; i < arguments.Length; i++)
                        arguments[i] = Execute(function.Arguments[i], functionRegistry, variables);

                    return Invoke(functionInfo.Function, arguments);

                default:
                    throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".", nameof(operation));
            }
        }

        private float Invoke(Delegate function, float[] args)
        {
            switch (function)
            {
                case Func<float> f:
                    return f.Invoke();
                case Func<float, float> f:
                    return f.Invoke(args[0]);
                case Func<float, float, float> f:
                    return f.Invoke(args[0], args[1]);
                case Func<float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2]);
                case Func<float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3]);
                case Func<float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4]);
                case Func<float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5]);
                case Func<float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                case Func<float, float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                case Func<float, float, float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                case Func<float, float, float, float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
                case Func<float, float, float, float, float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);
                case Func<float, float, float, float, float, float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);
                case Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]);
                case Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]);
                case Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14]);
                case Func<float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float, float> f:
                    return f.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15]);
                default:
                    throw new ArgumentException($"Unexpected delegate type: '{function.GetType().Name}'");
            }
        }
    }
}
