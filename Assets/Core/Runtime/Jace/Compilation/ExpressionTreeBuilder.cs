using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Plml.Jace.Maths;
using Plml.Jace.Operations;
using Plml.Jace.Parsing;

namespace Plml.Jace.Compilation
{
    internal class ExpressionTreeBuilder
    {
        public Expression<TFunc> BuildExpressionTree<TFunc>(
            Operation operation,
            IFunctionRegistry functionRegistry,
            IConstantRegistry constantRegistry,
            IList<string> paramNames)
            where TFunc : Delegate
        {
            ParameterExpression[] parameters = paramNames
                .Select(param => Expression.Parameter(typeof(float), param))
                .ToArray();

            IDictionary<string, ParameterExpression> parametersDic = parameters.ToDictionary(parameter => parameter.Name);

            return Expression.Lambda<TFunc>(
                GenerateMethodBody(
                    operation,
                    functionRegistry,
                    constantRegistry,
                    parametersDic),
                parameters
            );
        }

        private Expression GenerateMethodBody(
            Operation operation,
            IFunctionRegistry functionRegistry,
            IConstantRegistry constantRegistry,
            IDictionary<string, ParameterExpression> parametersDic)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (operation is FloatingPointConstant constant)
            {
                return Expression.Constant(constant.Value, typeof(float));
            }
            else if (operation is Variable variable)
            {
                if (parametersDic.TryGetValue(variable.Name, out ParameterExpression parameter))
                    return parameter;
                else if (constantRegistry.IsConstantName(variable.Name))
                {
                    ConstantInfo ci = constantRegistry.GetConstantInfo(variable.Name);
                    return Expression.Constant(ci.Value);
                }
                else
                    throw new VariableNotDefinedException($"Undefined symbol: \"{variable.Name}\"");
            }
            else if (operation is Multiplication multiplication)
            {
                Expression argument1 = GenerateMethodBody(multiplication.Argument1, functionRegistry, constantRegistry, parametersDic);
                Expression argument2 = GenerateMethodBody(multiplication.Argument2, functionRegistry, constantRegistry, parametersDic);

                return Expression.Multiply(argument1, argument2);
            }
            else if (operation is Addition addition)
            {
                Expression argument1 = GenerateMethodBody(addition.Argument1, functionRegistry, constantRegistry, parametersDic);
                Expression argument2 = GenerateMethodBody(addition.Argument2, functionRegistry, constantRegistry, parametersDic);

                return Expression.Add(argument1, argument2);
            }
            else if (operation is Substraction substraction)
            {
                Expression argument1 = GenerateMethodBody(substraction.Argument1, functionRegistry, constantRegistry, parametersDic);
                Expression argument2 = GenerateMethodBody(substraction.Argument2, functionRegistry, constantRegistry, parametersDic);

                return Expression.Subtract(argument1, argument2);
            }
            else if (operation is Division division)
            {
                Expression dividend = GenerateMethodBody(division.Dividend, functionRegistry, constantRegistry, parametersDic);
                Expression divisor = GenerateMethodBody(division.Divisor, functionRegistry, constantRegistry, parametersDic);

                return Expression.Divide(dividend, divisor);
            }
            else if (operation is Modulo modulo)
            {
                Expression dividend = GenerateMethodBody(modulo.Dividend, functionRegistry, constantRegistry, parametersDic);
                Expression divisor = GenerateMethodBody(modulo.Divisor, functionRegistry, constantRegistry, parametersDic);

                return Expression.Modulo(dividend, divisor);
            }
            else if (operation is Exponentiation exponentiation)
            {
                Expression @base = GenerateMethodBody(exponentiation.Base, functionRegistry, constantRegistry, parametersDic);
                Expression exponent = GenerateMethodBody(exponentiation.Exponent, functionRegistry, constantRegistry, parametersDic);

                return Expression.Call(null, typeof(Mathf).GetRuntimeMethod("Pow", new Type[] { typeof(float), typeof(float) }), @base, exponent);
            }
            else if (operation is UnaryMinus unaryMinus)
            {
                Expression argument = GenerateMethodBody(unaryMinus.Argument, functionRegistry, constantRegistry, parametersDic);
                return Expression.Negate(argument);
            }
            else if (operation is And and)
            {
                Expression argument1 = Expression.NotEqual(GenerateMethodBody(and.Argument1, functionRegistry, constantRegistry, parametersDic), Expression.Constant(0.0f));
                Expression argument2 = Expression.NotEqual(GenerateMethodBody(and.Argument2, functionRegistry, constantRegistry, parametersDic), Expression.Constant(0.0f));

                return Expression.Condition(Expression.And(argument1, argument2),
                    Expression.Constant(1.0f),
                    Expression.Constant(0.0f));
            }
            else if (operation is Or or)
            {
                Expression argument1 = Expression.NotEqual(GenerateMethodBody(or.Argument1, functionRegistry, constantRegistry, parametersDic), Expression.Constant(0.0f));
                Expression argument2 = Expression.NotEqual(GenerateMethodBody(or.Argument2, functionRegistry, constantRegistry, parametersDic), Expression.Constant(0.0f));

                return Expression.Condition(Expression.Or(argument1, argument2),
                    Expression.Constant(1.0f),
                    Expression.Constant(0.0f));
            }
            else if (operation is LessThan lessThan)
            {
                Expression argument1 = GenerateMethodBody(lessThan.Argument1, functionRegistry, constantRegistry, parametersDic);
                Expression argument2 = GenerateMethodBody(lessThan.Argument2, functionRegistry, constantRegistry, parametersDic);

                return Expression.Condition(Expression.LessThan(argument1, argument2),
                    Expression.Constant(1.0f),
                    Expression.Constant(0.0f));
            }
            else if (operation is LessOrEqualThan lessOrEqualThan)
            {
                Expression argument1 = GenerateMethodBody(lessOrEqualThan.Argument1, functionRegistry, constantRegistry, parametersDic);
                Expression argument2 = GenerateMethodBody(lessOrEqualThan.Argument2, functionRegistry, constantRegistry, parametersDic);

                return Expression.Condition(Expression.LessThanOrEqual(argument1, argument2),
                    Expression.Constant(1.0f),
                    Expression.Constant(0.0f));
            }
            else if (operation is GreaterThan greaterThan)
            {
                Expression argument1 = GenerateMethodBody(greaterThan.Argument1, functionRegistry, constantRegistry, parametersDic);
                Expression argument2 = GenerateMethodBody(greaterThan.Argument2, functionRegistry, constantRegistry, parametersDic);

                return Expression.Condition(Expression.GreaterThan(argument1, argument2),
                    Expression.Constant(1.0f),
                    Expression.Constant(0.0f));
            }
            else if (operation is GreaterOrEqualThan greaterOrEqualThan)
            {
                Expression argument1 = GenerateMethodBody(greaterOrEqualThan.Argument1, functionRegistry, constantRegistry, parametersDic);
                Expression argument2 = GenerateMethodBody(greaterOrEqualThan.Argument2, functionRegistry, constantRegistry, parametersDic);

                return Expression.Condition(Expression.GreaterThanOrEqual(argument1, argument2),
                    Expression.Constant(1.0f),
                    Expression.Constant(0.0f));
            }
            else if (operation is Equal equal)
            {
                Expression argument1 = GenerateMethodBody(equal.Argument1, functionRegistry, constantRegistry, parametersDic);
                Expression argument2 = GenerateMethodBody(equal.Argument2, functionRegistry, constantRegistry, parametersDic);

                return Expression.Condition(Expression.Equal(argument1, argument2),
                    Expression.Constant(1.0f),
                    Expression.Constant(0.0f));
            }
            else if (operation is NotEqual notEqual)
            {
                Expression argument1 = GenerateMethodBody(notEqual.Argument1, functionRegistry, constantRegistry, parametersDic);
                Expression argument2 = GenerateMethodBody(notEqual.Argument2, functionRegistry, constantRegistry, parametersDic);

                return Expression.Condition(Expression.NotEqual(argument1, argument2),
                    Expression.Constant(1.0f),
                    Expression.Constant(0.0f));
            }
            else if (operation is Function function)
            {
                FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);

                Type funcType = functionInfo.Function.GetType();
                Type[] parameterTypes = Enumerable.Repeat(typeof(float), functionInfo.NumberOfParameters).ToArray();

                Expression[] arguments = new Expression[functionInfo.NumberOfParameters];
                for (int i = 0; i < functionInfo.NumberOfParameters; i++)
                    arguments[i] = GenerateMethodBody(function.Arguments[i], functionRegistry, constantRegistry, parametersDic);

                ConstantExpression getFunctionInfo = Expression.Constant(functionInfo.Function, funcType);
                MethodInfo invokeMethod = funcType.GetRuntimeMethod("Invoke", parameterTypes);

                return Expression.Call(
                    getFunctionInfo,
                    invokeMethod,
                    arguments);
            }
            else
            {
                throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".", nameof(operation));
            }
        }
    }
}