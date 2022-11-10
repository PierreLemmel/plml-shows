using System;
using System.Collections.Generic;
using System.Linq;
using Plml.Jace.Compilation;
using Plml.Jace.Operations;
using Plml.Jace.Parsing;

namespace Plml.Jace.Compilation
{
    public class AstBuilder
    {
        private readonly IFunctionRegistry functionRegistry;
        private readonly Dictionary<char, int> operationPrecedence = new();
        private readonly Stack<Operation> resultStack = new();
        private readonly Stack<Token> operatorStack = new();
        private readonly Stack<int> parameterCount = new();

        public AstBuilder(IFunctionRegistry functionRegistry)
        {
            this.functionRegistry = functionRegistry ?? throw new ArgumentNullException(nameof(functionRegistry));

            operationPrecedence.Add('(', 0);
            operationPrecedence.Add('&', 1);
            operationPrecedence.Add('|', 1);
            operationPrecedence.Add('<', 2);
            operationPrecedence.Add('>', 2);
            operationPrecedence.Add('≤', 2);
            operationPrecedence.Add('≥', 2);
            operationPrecedence.Add('≠', 2);
            operationPrecedence.Add('=', 2);
            operationPrecedence.Add('+', 3);
            operationPrecedence.Add('-', 3);
            operationPrecedence.Add('*', 4);
            operationPrecedence.Add('/', 4);
            operationPrecedence.Add('%', 4);
            operationPrecedence.Add('_', 6);
            operationPrecedence.Add('^', 5);
        }

        public Operation Build(IList<Token> tokens)
        {
            resultStack.Clear();
            operatorStack.Clear();

            parameterCount.Clear();

            foreach (Token token in tokens)
            {
                object value = token.Value;

                switch (token.TokenType)
                {
                    case TokenType.FloatingPoint:
                        resultStack.Push(new FloatingPointConstant((float)token.Value));
                        break;
                    case TokenType.Text:
                        if (functionRegistry.IsFunctionName((string)token.Value))
                        {
                            operatorStack.Push(token);
                            parameterCount.Push(1);
                        }
                        else
                        {
                            string tokenValue = (string)token.Value;
                            resultStack.Push(new Variable(tokenValue));
                        }
                        break;
                    case TokenType.LeftBracket:
                        operatorStack.Push(token);
                        break;
                    case TokenType.RightBracket:
                        PopOperations(true, token);
                        //parameterCount.Pop();
                        break;
                    case TokenType.ArgumentSeparator:
                        PopOperations(false, token);
                        parameterCount.Push(parameterCount.Pop() + 1);
                        break;
                    case TokenType.Operation:
                        Token operation1Token = token;
                        char operation1 = (char)operation1Token.Value;

                        while (operatorStack.Count > 0 && (operatorStack.Peek().TokenType == TokenType.Operation ||
                            operatorStack.Peek().TokenType == TokenType.Text))
                        {
                            Token operation2Token = operatorStack.Peek();
                            bool isFunctionOnTopOfStack = operation2Token.TokenType == TokenType.Text;

                            if (!isFunctionOnTopOfStack)
                            {
                                char operation2 = (char)operation2Token.Value;

                                if ((IsLeftAssociativeOperation(operation1) &&
                                        operationPrecedence[operation1] <= operationPrecedence[operation2]) ||
                                    (operationPrecedence[operation1] < operationPrecedence[operation2]))
                                {
                                    operatorStack.Pop();
                                    resultStack.Push(ConvertOperation(operation2Token));
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                operatorStack.Pop();
                                resultStack.Push(ConvertFunction(operation2Token));
                            }
                        }

                        operatorStack.Push(operation1Token);
                        break;
                }
            }

            PopOperations(false, null);

            VerifyResultStack();

            return resultStack.First();
        }

        private void PopOperations(bool untillLeftBracket, Token? currentToken)
        {
            if (untillLeftBracket && !currentToken.HasValue)
                throw new ArgumentNullException("currentToken", "If the parameter \"untillLeftBracket\" is set to true, " +
                    "the parameter \"currentToken\" cannot be null.");

            while (operatorStack.Count > 0 && operatorStack.Peek().TokenType != TokenType.LeftBracket)
            {
                Token token = operatorStack.Pop();

                switch (token.TokenType)
                {
                    case TokenType.Operation:
                        resultStack.Push(ConvertOperation(token));
                        break;
                    case TokenType.Text:
                        resultStack.Push(ConvertFunction(token));
                        break;
                }
            }

            if (untillLeftBracket)
            {
                if (operatorStack.Count > 0 && operatorStack.Peek().TokenType == TokenType.LeftBracket)
                    operatorStack.Pop();
                else
                    throw new ParseException($"No matching left bracket found for the right bracket at position {currentToken.Value.StartPosition}.");
            }
            else
            {
                if (operatorStack.Count > 0 && operatorStack.Peek().TokenType == TokenType.LeftBracket 
                    && !(currentToken.HasValue && currentToken.Value.TokenType == TokenType.ArgumentSeparator))
                    throw new ParseException($"No matching right bracket found for the left bracket at position {operatorStack.Peek().StartPosition}.");
            }
        }

        private Operation ConvertOperation(Token operationToken)
        {
            try
            {
                Operation argument1;
                Operation argument2;
                Operation divisor;
                Operation divident;

                switch ((char)operationToken.Value)
                {
                    case '+':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new Addition(argument1, argument2);
                    case '-':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new Substraction(argument1, argument2);
                    case '*':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new Multiplication(argument1, argument2);
                    case '/':
                        divisor = resultStack.Pop();
                        divident = resultStack.Pop();

                        return new Division(divident, divisor);
                    case '%':
                        divisor = resultStack.Pop();
                        divident = resultStack.Pop();

                        return new Modulo(divident, divisor);
                    case '_':
                        argument1 = resultStack.Pop();

                        return new UnaryMinus(argument1);
                    case '^':
                        Operation exponent = resultStack.Pop();
                        Operation @base = resultStack.Pop();

                        return new Exponentiation(@base, exponent);
                    case '&':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new And(argument1, argument2);
                    case '|':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new Or(argument1, argument2);
                    case '<':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new LessThan(argument1, argument2);
                    case '≤':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new LessOrEqualThan(argument1, argument2);
                    case '>':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new GreaterThan(argument1, argument2);
                    case '≥':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new GreaterOrEqualThan(argument1, argument2);
                    case '=':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new Equal(argument1, argument2);
                    case '≠':
                        argument2 = resultStack.Pop();
                        argument1 = resultStack.Pop();

                        return new NotEqual(argument1, argument2);
                    default:
                        throw new ArgumentException($"Unknown operation \"{operationToken}\".", "operation");
                }
            }
            catch (InvalidOperationException)
            {
                // If we encounter a Stack empty issue this means there is a syntax issue in 
                // the mathematical formula
                throw new ParseException($"There is a syntax issue for the operation \"{operationToken.Value}\" at position {operationToken.StartPosition}. " +
                    "The number of arguments does not match with what is expected.");
            }
        }

        private Operation ConvertFunction(Token functionToken)
        {
            try
            {
                string functionName = ((string)functionToken.Value).ToLowerInvariant();

                if (functionRegistry.IsFunctionName(functionName))
                {
                    FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(functionName);

                    parameterCount.Pop();
                    int numberOfParameters = functionInfo.NumberOfParameters;
                    
                    List<Operation> operations = new List<Operation>();
                    for (int i = 0; i < numberOfParameters; i++)
                        operations.Add(resultStack.Pop());
                    operations.Reverse();

                    return new Function(functionName, operations);
                }
                else
                {
                    throw new ArgumentException($"Unknown function \"{functionToken.Value}\".", "function");
                }
            }
            catch (InvalidOperationException)
            {
                // If we encounter a Stack empty issue this means there is a syntax issue in 
                // the mathematical formula
                throw new ParseException($"There is a syntax issue for the function \"{functionToken.Value}\" at position {functionToken.StartPosition}. " +
                    "The number of arguments does not match with what is expected.");
            }
        }

        private void VerifyResultStack()
        {
            if(resultStack.Count > 1)
            {
                Operation[] operations = resultStack.ToArray();

                for (int i = 1; i < operations.Length; i++)
                {
                    Operation operation = operations[i];

                    if (operation is FloatingPointConstant constant)
                    {
                        throw new ParseException($"Unexpected floating point constant \"{constant.Value}\" found."); 
                    }
                }

                throw new ParseException("The syntax of the provided formula is not valid.");
            }
        }

        private bool IsLeftAssociativeOperation(char character)
        {
            return character == '*' || character == '+' || character == '-' || character == '/';
        }
    }
}