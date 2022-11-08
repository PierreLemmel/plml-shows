using Plml.Dmx.Scripting.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

using TokenType = Plml.Dmx.Scripting.Compilation.LightScriptTokenType;
using TokenizationContext = Plml.Dmx.Scripting.Compilation.LightScriptTokenizationContext;
using NumberType = Plml.Dmx.Scripting.Compilation.LightScriptTokenizationNumberTypeContext;

using Plml.Dmx.Scripting.Compilation.Nodes;
using Plml.Dmx.Scripting.Types;
using System.Linq.Expressions;
using System.Globalization;
using System.Reflection;

namespace Plml.Dmx.Scripting
{
    internal static class LightScriptCompilation
    {
        public static LightScriptCompilationResult Compile(LightScriptData data, LightScriptCompilationOptions options)
        {
            try
            {
                string src = data.text;

                Log($"Compiling source:\n'{src}'");

                Log("Tokenizing source");
                LightScriptToken[] tokens = Tokenize(src);
                Log("Source tokenized");

                Log("Building context");
                ILightScriptCompilationContext context = BuildContext(data);
                Log("Context built");

                Log("Building abstract syntax tree");
                AbstractSyntaxTree ast = BuildAst(tokens, context);
                Log("Abstract syntax tree built");

                Log("Compiling abstract syntax tree");
                LightScriptAction action = CompileAst(ast, context);
                Log("Abstract syntax tree compiled");

                Log($"Source compiled:\n'{src}'");

                return LightScriptCompilationResult.Ok(action);
            }
            catch (Exception ex)
            {
                return LightScriptCompilationResult.Error($"A compilation error occured:\n{ex.Message}");
            }

            void Log(string message)
            {
                if (options.log)
                    Debug.Log(message);
            }
        }

        public static LightScriptToken[] Tokenize(string text)
        {
            string src = Regex.Replace(text, "( )+", " ");

            List<LightScriptToken> result = new();

            int startPosition = 0;
            int i = 0;
            TokenizationContext tokenContext = TokenizationContext.NewToken;
            TokenizationContext charContext = TokenizationContext.UNINITIALIZED;
            NumberType numberType = NumberType.Integer;

            int length = src.Length;
            for (; i < length; i++)
            {
                char c = src[i];

                bool charIsValidInCurrentContext = tokenContext switch
                {
                    TokenizationContext.Identifier => char.IsLetterOrDigit(c),
                    TokenizationContext.Number => char.IsLetterOrDigit(c) || c == '.',
                    _ => false
                };

                if (!charIsValidInCurrentContext)
                {
                    charContext = c switch
                    {
                        ' ' => TokenizationContext.TokenEnd,
                        '\r' or '\n' or ';' => TokenizationContext.StatementEnding,
                        _ when char.IsDigit(c) => TokenizationContext.Number,
                        _ when IsSingleChar(c) => TokenizationContext.SingleChar,
                        _ when char.IsLetter(c) => TokenizationContext.Identifier,
                        _ when IsOperator(c) => TokenizationContext.Operator,
                        
                        _ => throw new TokenizationException(CompilationErrorType.InvalidCharacter, $"Unexpected character: '{c}'")
                    };
                }

                if (charContext == TokenizationContext.Number && numberType == NumberType.Integer)
                {
                    numberType = c switch
                    {
                        'x' => NumberType.Hexadecimal,
                        '.' => NumberType.Float,
                        _ => numberType
                    };
                }

                if (tokenContext == TokenizationContext.NewToken && charContext != TokenizationContext.TokenEnd)
                {
                    tokenContext = charContext;
                }

                if (charContext != tokenContext || tokenContext == TokenizationContext.SingleChar)
                {
                    AddCurrentTokenToResult();

                    startPosition = i;
                    tokenContext = charContext;
                }
            }

            AddCurrentTokenToResult();


            void AddCurrentTokenToResult()
            {
                switch (tokenContext)
                {
                    case TokenizationContext.Identifier:
                        result.Add(new(TokenType.Identifier, GetCurrentString()));
                        break;
                    case TokenizationContext.Number:
                        result.Add(new(TokenType.Number, GetCurrentString()));
                        numberType = NumberType.Integer;
                        break;
                    case TokenizationContext.TokenEnd:
                        break;
                    case TokenizationContext.StatementEnding:
                        result.Add(new(TokenType.StatementEnding));
                        break;
                    case TokenizationContext.SingleChar:

                        char sc = src[startPosition];
                        TokenType type = sc switch
                        {
                            '(' => TokenType.LeftBracket,
                            ')' => TokenType.RightBracket,
                            '.' => TokenType.DotNotation,
                            '=' => TokenType.Assignment,
                            _ => throw new TokenizationException(CompilationErrorType.InvalidCharacter, $"Unexpected single char: '{sc}'")
                        };
                        result.Add(new(type));

                        break;
                    case TokenizationContext.Operator:
                        result.Add(new(TokenType.Operator, GetCurrentString()));
                        break;
                }
            }
            
            string GetCurrentString() => src[startPosition..i];

            return result.ToArray();
        }


        public static void ValidateTokens(LightScriptToken[] tokens)
        {
            ValidateNumberFormats(tokens);
            ValidateBrackets(tokens);
        }

        private static void ValidateNumberFormats(LightScriptToken[] tokens) => tokens
            .Where(token => token.type == TokenType.Number)
            .Select(token => token.content)
            .ForEach(str =>
            {
                bool result = Regex.IsMatch(str, "^[0-9]+$")
                        || Regex.IsMatch(str, @"^[0-9]+\.[0-9]*$")
                        || Regex.IsMatch(str, @"^0x([0-9]|[a-f]|[A-F])+$");

                if (!result)
                    throw new TokenizationException(CompilationErrorType.InvalidNumberFormat, $"Invalid number format: '{str}'");
            });

        private static void ValidateBrackets(LightScriptToken[] tokens)
        {
            var statements = tokens.Split(t => t.type == TokenType.StatementEnding);

            const string ErrorMsg = "Error in brackets";

            foreach (var statement in statements)
            {
                int opened = 0;

                foreach (LightScriptToken token in statement)
                {
                    switch (token.type)
                    {
                        case TokenType.LeftBracket:
                            opened++;
                            break;
                        case TokenType.RightBracket:
                            if (--opened < 0)
                                throw new TokenizationException(CompilationErrorType.InvalidBrackets, ErrorMsg);
                            break;
                    }
                }

                if (opened != 0)
                    throw new TokenizationException(CompilationErrorType.InvalidBrackets, ErrorMsg);
            }
        }

        public static ILightScriptCompilationContext BuildContext(LightScriptData data)
        {
            LightScriptCompilationContext context = new();

            context.AddVariable(new(LightScriptType.Float, "t"));
            context.AddVariable(new(LightScriptType.Float, "dt"));
            context.AddVariable(new(LightScriptType.Integer, "frame"));

            data.fixtures.ForEach(fixture => context.AddVariable(new(LightScriptType.Fixture, fixture.name)));
            data.integers.ForEach(integer => context.AddVariable(new(LightScriptType.Integer, integer.name)));


            foreach (var func in LightScriptFunctions.DefaultFunctions)
                context.AddFunction(func);


            return context;
        }

        public static AbstractSyntaxTree BuildAst(LightScriptToken[] tokens, ILightScriptCompilationContext context)
        {
            LightScriptToken[][] scriptTokens = tokens
                .Split(token => token.type == TokenType.StatementEnding);

            SyntaxNode[] statements = scriptTokens.Select(t => BuildSyntaxNodeFromTokens(t, context));

            AbstractSyntaxTree result = new(statements);

            return result;
        }

        
        private enum OperationStackElementType
        {
            BinaryOperator,
            LeftBracket,
            RightBracket,
            Function,
        }

        private static SyntaxNode BuildSyntaxNodeFromTokens(LightScriptToken[] tokens, ILightScriptCompilationContext context)
        {
            Stack<(OperationStackElementType Type, BinaryOperatorType? Operator, string Function)> operationStack = new();
            Stack<SyntaxNode> resultStack = new();
            Stack<int> argcountStack = new();

            ASTBuilderContext astContext = ASTBuilderContext.Default;
            LightScriptType currentObjectType = LightScriptType.Undefined;

            foreach (var token in tokens)
            {
                string content = token.content;
                TokenType type = token.type;

                ASTBuilderContext newContext;
                LightScriptType newObjectType = LightScriptType.Undefined;

                switch (type)
                {
                    case TokenType.Identifier:
                        
                        switch (astContext)
                        {
                            case ASTBuilderContext.Default:

                                if (context.TryGetVariable(content, out var variable))
                                {
                                    VariableNode variableNode = new(variable.Type, variable.Name);
                                    newObjectType = variable.Type;

                                    PushResultToStack(variableNode);
                                }
                                else if (context.HasFunction(content))
                                {
                                    PushFunctionToStack(content);
                                }
                                else
                                    throw new SyntaxTreeException(CompilationErrorType.UnknownVariable, $"Unknown identifier '{content}'");

                                break;

                            case ASTBuilderContext.MemberAccess:

                                if (currentObjectType.HasProperty(content, out LightScriptType propertyType))
                                {
                                    newObjectType = propertyType;
                                    newContext = ASTBuilderContext.Object;

                                    MemberAccessNode memberAccess = new(resultStack.Pop(), propertyType, content);
                                    PushResultToStack(memberAccess);
                                }
                                else
                                    throw new SyntaxTreeException(CompilationErrorType.MissingProperty, $"Missing property '{content}' on type {currentObjectType}");
                                
                                break;
                        }
                        newContext = ASTBuilderContext.Object;

                        break;
                    case TokenType.Number:
                        ConstantNode constantNode = int.TryParse(content, out int intResult) ?
                            new(intResult) :
                            new(float.Parse(content, CultureInfo.InvariantCulture));

                        PushResultToStack(constantNode);
                        newContext = ASTBuilderContext.Default;
                        
                        break;

                    case TokenType.Operator:

                        BinaryOperatorType @operator = content switch
                        {
                            "+" => BinaryOperatorType.Addition,
                            "-" => BinaryOperatorType.Substraction,
                            "/" => BinaryOperatorType.Division,
                            "*" => BinaryOperatorType.Multiplication,
                            "%" => BinaryOperatorType.Modulo,
                            "^" => BinaryOperatorType.Exponentiation,
                            _ => throw new SyntaxTreeException(CompilationErrorType.UnknownOperator, $"Unknown operator '{content}'")
                        };

                        PushOperatorToStack(@operator);

                        newContext = ASTBuilderContext.Default;

                        break;

                    case TokenType.Assignment:
                        PushOperatorToStack(BinaryOperatorType.Assignment);
                        newContext = ASTBuilderContext.Default;

                        break;

                    case TokenType.DotNotation:

                        if (astContext != ASTBuilderContext.Object)
                            throw new SyntaxTreeException(CompilationErrorType.InvalidContext, "Member access is not valid in the current context");

                        newContext = ASTBuilderContext.MemberAccess;
                        newObjectType = currentObjectType;

                        break;

                    case TokenType.LeftBracket:

                        PushLeftBracket();
                        newContext = ASTBuilderContext.Default;

                        break;

                    case TokenType.RightBracket:
                        
                        PushRightBracket();
                        newContext = ASTBuilderContext.Default;

                        break;

                    default:
                        throw new SyntaxTreeException(CompilationErrorType.UnsupportedToken, $"Unsupported token '{token.type}'");
                }

                astContext = newContext;
                currentObjectType = newObjectType;
            }

            while (operationStack.Any())
                PopOperationStack();

            void PushResultToStack(SyntaxNode node) => resultStack.Push(node);

            void PushOperatorToStack(BinaryOperatorType @operator)
            {
                var operatorInfo = @operator.GetOperatorInfo();
                int operatorPrecedence = operatorInfo.Precedence;
                
                while (operationStack.TryPeek(out var lastElt))
                {
                    if (lastElt.Type != OperationStackElementType.BinaryOperator)
                        break;

                    BinaryOperatorType lastOperator = lastElt.Operator.Value;
                    int lastPrecedence = lastOperator.GetOperatorInfo().Precedence;
                    if (!(operatorPrecedence < lastPrecedence || operatorInfo.IsLeftAssociative && operatorPrecedence == lastPrecedence))
                        break;

                    PopOperationStack();
                }
                
                operationStack.Push((OperationStackElementType.BinaryOperator, @operator, null));
            }

            void PushFunctionToStack(string function)
            {
                operationStack.Push((OperationStackElementType.Function, null, function));
                argcountStack.Push(1);
            }

            void PushLeftBracket() => operationStack.Push((OperationStackElementType.LeftBracket, null, null));

            void PushRightBracket()
            {
                while (operationStack.Peek().Type != OperationStackElementType.LeftBracket)
                    PopOperationStack();

                operationStack.Pop();

                if (operationStack.Peek().Type == OperationStackElementType.Function)
                    PopOperationStack();
            }

            void PopOperationStack()
            {
                var topElt = operationStack.Pop();

                switch (topElt.Type)
                {
                    case OperationStackElementType.BinaryOperator:
                        var @operator = topElt.Operator.Value;

                        if (!resultStack.TryPop(out SyntaxNode rhs))
                            throw new SyntaxTreeException(CompilationErrorType.NoLeftHandSideForOperator, $"Missing left hand side for {@operator} operator");
                        if (!resultStack.TryPop(out SyntaxNode lhs))
                            throw new SyntaxTreeException(CompilationErrorType.NoLeftHandSideForOperator, $"Missing right hand side for {@operator} operator");

                        if (LightScriptTypeSystem.IsValidOperatorType(@operator, lhs.Type, rhs.Type, out var operatorResultType))
                        {
                            PushOperationNode(@operator, lhs, rhs, operatorResultType);
                        }
                        else if (LightScriptTypeSystem.HasImplicitConversion(rhs.Type, lhs.Type))
                        {
                            PushOperationNode(@operator, lhs, new ImplicitConversionNode(rhs, lhs.Type), lhs.Type);
                        }
                        // Auto-conversion (rounding) for assignments 
                        else if (@operator == BinaryOperatorType.Assignment && LightScriptTypeSystem.HasExplicitConversion(rhs.Type, lhs.Type))
                        {
                            PushOperationNode(BinaryOperatorType.Assignment, lhs, new ExplicitConversionNode(rhs, lhs.Type), lhs.Type);
                        }
                        else
                            throw new SyntaxTreeException(CompilationErrorType.TypeError, $"Operator '{@operator}' does not exist between types '{lhs.Type}' and '{rhs.Type}'");

                        break;

                    case OperationStackElementType.Function:

                        var functionName = topElt.Function;

                        int argc = argcountStack.Pop();
                        SyntaxNode[] arguments = new SyntaxNode[argc];

                        for (int i = argc - 1; i >= 0; i--)
                            arguments[i] = resultStack.Pop();

                        LightScriptType[] argTypes = arguments.Select(arg => arg.Type);


                        if (TryFindFunction(functionName, argTypes, out LightScriptFunction function, out bool[] conversions))
                        {
                            SyntaxNode[] convertedArguments = arguments.Select((arg, i) => conversions[i] ?
                                new ImplicitConversionNode(arg, function.Arguments[i].Type)
                                : arg);

                            FunctionNode functionNode = new(function, convertedArguments);
                            resultStack.Push(functionNode);
                        }
                        else
                        {
                            string argsStr = string.Join(", ", argTypes);
                            throw new SyntaxTreeException(CompilationErrorType.InvalidArgumentType, $"Impossible to find a function named '{functionName}' with the following arguments: ({argsStr})");
                        }

                        break;

                    default:
                        throw new SyntaxTreeException(CompilationErrorType.InternalSyntaxTreeError, $"Unexpected operation element type: '{topElt.Type}'");
                }

                void PushOperationNode(BinaryOperatorType @operator, SyntaxNode lhs, SyntaxNode rhs, LightScriptType resultType)
                {
                    SyntaxNode operationNode = @operator switch
                    {
                        BinaryOperatorType.Addition => new AdditionNode(resultType, lhs, rhs),
                        BinaryOperatorType.Substraction => new SubstractionNode(resultType, lhs, rhs),
                        BinaryOperatorType.Multiplication => new MultiplicationNode(resultType, lhs, rhs),
                        BinaryOperatorType.Division => new DivisionNode(resultType, lhs, rhs),
                        BinaryOperatorType.Assignment => new AssignmentNode(resultType, lhs, rhs),
                        BinaryOperatorType.Modulo => new ModuloNode(resultType, lhs, rhs),
                        BinaryOperatorType.Exponentiation => new ExponentiationNode(resultType, lhs, rhs),
                        _ => throw new SyntaxTreeException(CompilationErrorType.UnknownOperator, $"Unexpected operator '{@operator}'")
                    };

                    resultStack.Push(operationNode);
                }

                bool TryFindFunction(string name, LightScriptType[] inputTypes, out LightScriptFunction result, out bool[] conversions)
                {
                    int argc = inputTypes.Length;
                    conversions = new bool[argc];

                    if (context.TryGetFunction(name, out result, inputTypes))
                        return true;

                    foreach (var function in context.GetFunctions(name))
                    {
                        conversions.Set(false);
                        var funcArgs = function.Arguments;

                        if (funcArgs.Length != argc)
                            continue;

                        bool isValid = true;
                        for (int i = 0; i < argc; i++)
                        {
                            var funcArg = funcArgs[i];

                            if (funcArg.Type != inputTypes[i])
                            {
                                if (LightScriptTypeSystem.HasImplicitConversion(inputTypes[i], funcArg.Type))
                                    conversions[i] = true;
                                else
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                        }

                        if (isValid)
                        {
                            result = function;
                            return true;
                        }
                    }

                    result = null;
                    return false;
                }
            }

            return resultStack.First();
        }


        public static AbstractSyntaxTree OptimizeAst(AbstractSyntaxTree syntaxTree)
        {
            SyntaxNode[] optimizedStatements = syntaxTree.Statements.Select(OptimizeNode);

            static SyntaxNode OptimizeNode(SyntaxNode node)
            {
                if (node is BinaryOperatorNode binaryOperator)
                {
                    if (binaryOperator.Operator != BinaryOperatorType.Assignment)
                    {
                        var lhs = OptimizeNode(binaryOperator.LeftHandSide);
                        var rhs = OptimizeNode(binaryOperator.RightHandSide);

                        if (lhs is ConstantNode lhsConstant && rhs is ConstantNode rhsConstant)
                        {
                            var resultType = binaryOperator.Operator.GetOperatorResultType(lhs.Type, rhs.Type);
                            if (resultType == LightScriptType.Integer)
                            {
                                int lhsVal = lhsConstant.IntValue;
                                int rhsVal = rhsConstant.IntValue;

                                int constValue = binaryOperator.Operator switch
                                {
                                    BinaryOperatorType.Addition => lhsVal + rhsVal,
                                    BinaryOperatorType.Substraction => lhsVal - rhsVal,
                                    BinaryOperatorType.Multiplication => lhsVal * rhsVal,
                                    BinaryOperatorType.Division => lhsVal * rhsVal,
                                    BinaryOperatorType.Modulo => lhsVal % rhsVal,
                                    BinaryOperatorType.Exponentiation => (int)Mathf.Pow(lhsVal, rhsVal),
                                    _ => throw new SyntaxTreeException(CompilationErrorType.UnknownOperator, $"Unsupported operator: {binaryOperator.Operator}")
                                };

                                return new ConstantNode(constValue);
                            }
                            else if (resultType == LightScriptType.Float)
                            {
                                float lhsVal = lhsConstant.Type == LightScriptType.Float ? lhsConstant.FloatValue : lhsConstant.IntValue;
                                float rhsVal = rhsConstant.Type == LightScriptType.Float ? rhsConstant.FloatValue : rhsConstant.IntValue;

                                float constValue = binaryOperator.Operator switch
                                {
                                    BinaryOperatorType.Addition => lhsVal + rhsVal,
                                    BinaryOperatorType.Substraction => lhsVal - rhsVal,
                                    BinaryOperatorType.Multiplication => lhsVal * rhsVal,
                                    BinaryOperatorType.Division => lhsVal * rhsVal,
                                    BinaryOperatorType.Modulo => lhsVal % rhsVal,
                                    BinaryOperatorType.Exponentiation => Mathf.Pow(lhsVal, rhsVal),
                                    _ => throw new SyntaxTreeException(CompilationErrorType.UnknownOperator, $"Unsupported operator: {binaryOperator.Operator}")
                                };

                                return new ConstantNode(constValue);
                            }
                        }

                        BinaryOperatorNode resultNode = binaryOperator.Operator switch
                        {
                            BinaryOperatorType.Addition => new AdditionNode(lhs, rhs),
                            BinaryOperatorType.Substraction => new SubstractionNode(lhs, rhs),
                            BinaryOperatorType.Multiplication => new MultiplicationNode(lhs, rhs),
                            BinaryOperatorType.Division => new DivisionNode(lhs, rhs),
                            BinaryOperatorType.Modulo => new ModuloNode(lhs, rhs),
                            BinaryOperatorType.Exponentiation => new ExponentiationNode(lhs, rhs),
                            _ => throw new SyntaxTreeException(CompilationErrorType.UnknownOperator, $"Unsupported operator: {binaryOperator.Operator}")
                        };

                        return resultNode;
                    }
                    else
                    {
                        return new AssignmentNode(
                            binaryOperator.LeftHandSide.Clone(),
                            OptimizeNode(binaryOperator.RightHandSide)
                        );
                    }
                }
                else if (node is FunctionNode functionNode)
                {
                    var optimizedArgs = functionNode.Arguments.Select(arg => OptimizeNode(arg));

                    var func = functionNode.Function;
                    if (func.IsPure && optimizedArgs.All(arg => arg is ConstantNode))
                    {
                        object val = func.Function.DynamicInvoke(optimizedArgs.Select(arg => ((ConstantNode)arg).Value));
                        return func.ReturnType switch
                        {
                            LightScriptType.Integer => new ConstantNode((int)val),
                            LightScriptType.Float => new ConstantNode((float)val),
                            LightScriptType.Color => new ConstantNode((Color24)val),
                            _ => throw new CompilationException(CompilationErrorType.OptimizationError, $"Unsupported return type in pure function: {func.ReturnType}")
                        };
                    }
                    else
                        return new FunctionNode(functionNode.Function, optimizedArgs);
                }
                else
                {
                    return node.Clone();
                }
                
            }

            return new(optimizedStatements);
        }


        private static LightScriptAction CompileAst(AbstractSyntaxTree ast, ILightScriptCompilationContext context)
        {
            ParameterExpression contextExpression = Expression.Parameter(typeof(ILightScriptContext), "context");

            Expression[] compiledStatements = ast.Statements.Select(CompileNode);

            Expression block = Expression.Block(compiledStatements);

            Expression<LightScriptAction> lambda = Expression.Lambda<LightScriptAction>(block, contextExpression);
            LightScriptAction result = lambda.Compile();

            return result;

            Expression CompileNode(SyntaxNode node) => node switch
            {
                ConstantNode constant when constant.Type is LightScriptType.Integer => Expression.Constant(constant.IntValue, typeof(int)),
                ConstantNode constant when constant.Type is LightScriptType.Float => Expression.Constant(constant.FloatValue, typeof(float)),

                AdditionNode addition => CompileAddition(addition),
                SubstractionNode substraction => CompileSubstraction(substraction),
                MultiplicationNode multiplication => CompileMultiplication(multiplication),
                DivisionNode division => CompileDivision(division),
                ModuloNode modulo => CompileModulo(modulo),
                ExponentiationNode exponentiation => CompileExponentiation(exponentiation),

                AssignmentNode assignment => CompileAssignment(assignment),

                MemberAccessNode memberAccess => Expression.PropertyOrField(
                    CompileNode(memberAccess.Target),
                    LightScriptTypeSystem.GetPropertyInfo(
                        memberAccess.Target.Type,
                        memberAccess.Property
                    ).UnderlyingProperty
                ),

                VariableNode variable => CompileVariable(variable),

                _ => throw new CompilationException(CompilationErrorType.UnsupportedSyntaxNode, $"Unsupported syntax node in compilation '{node.GetType().Name}'")
            };

            void ConvertExpressionsForArithmeticOperatorsIfNeeded(ref Expression leftExpression, ref Expression rightExpression)
            {
                if (leftExpression.Type == typeof(int) && rightExpression.Type == typeof(float))
                {
                    leftExpression = Expression.Convert(leftExpression, typeof(float));
                }

                if (leftExpression.Type == typeof(float) && rightExpression.Type == typeof(int))
                {
                    rightExpression = Expression.Convert(rightExpression, typeof(float));
                }
            }

            Expression CompileAddition(AdditionNode addition)
            {
                Expression leftExpression = CompileNode(addition.LeftHandSide);
                Expression rightExpression = CompileNode(addition.RightHandSide);

                ConvertExpressionsForArithmeticOperatorsIfNeeded(ref leftExpression, ref rightExpression);

                return Expression.Add(
                    leftExpression,
                    rightExpression
                );
            }

            Expression CompileSubstraction(SubstractionNode substraction)
            {
                Expression leftExpression = CompileNode(substraction.LeftHandSide);
                Expression rightExpression = CompileNode(substraction.RightHandSide);

                ConvertExpressionsForArithmeticOperatorsIfNeeded(ref leftExpression, ref rightExpression);

                return Expression.Subtract(
                    leftExpression,
                    rightExpression
                );
            }

            Expression CompileMultiplication(MultiplicationNode multiply)
            {
                Expression leftExpression = CompileNode(multiply.LeftHandSide);
                Expression rightExpression = CompileNode(multiply.RightHandSide);

                ConvertExpressionsForArithmeticOperatorsIfNeeded(ref leftExpression, ref rightExpression);

                return Expression.Multiply(
                    leftExpression,
                    rightExpression
                );
            }

            Expression CompileDivision(DivisionNode division)
            {
                Expression leftExpression = CompileNode(division.LeftHandSide);
                Expression rightExpression = CompileNode(division.RightHandSide);

                ConvertExpressionsForArithmeticOperatorsIfNeeded(ref leftExpression, ref rightExpression);

                return Expression.Divide(
                    leftExpression,
                    rightExpression
                );
            }

            Expression CompileModulo(ModuloNode modulo)
            {
                Expression leftExpression = CompileNode(modulo.LeftHandSide);
                Expression rightExpression = CompileNode(modulo.RightHandSide);

                ConvertExpressionsForArithmeticOperatorsIfNeeded(ref leftExpression, ref rightExpression);

                return Expression.Modulo(
                    leftExpression,
                    rightExpression
                );
            }

            Expression CompileExponentiation(ExponentiationNode exponentiation)
            {
                Expression leftExpression = CompileNode(exponentiation.LeftHandSide);
                Expression rightExpression = CompileNode(exponentiation.RightHandSide);
                
                Expression result = Expression.Call(
                    null,
                    Methods.Pow,
                    ConvertIfNeeded(leftExpression, typeof(float)),
                    ConvertIfNeeded(rightExpression, typeof(float))
                );

                if (leftExpression.Type == typeof(int) && rightExpression.Type == typeof(int))
                    result = Expression.Convert(result, typeof(int));

                return result;
            }

            Expression CompileAssignment(AssignmentNode assignment)
            {
                var leftExpression = CompileNode(assignment.LeftHandSide);
                var rightExpression = CompileNode(assignment.RightHandSide);

                return Expression.Assign(
                    leftExpression,
                    ConvertIfNeeded(rightExpression, leftExpression.Type)
                );
            }

            Expression ConvertIfNeeded(Expression expression, Type type)
            {
                if (expression.Type != type)
                    expression = Expression.Convert(expression, type);

                return expression;
            }

            Expression CompileVariable(VariableNode variable)
            {
                Expression ctx = contextExpression;

                Expression bag = variable.Type switch
                {
                    LightScriptType.Integer => Expression.Property(ctx, nameof(ILightScriptContext.Integers)),
                    LightScriptType.Float => Expression.Property(ctx, nameof(ILightScriptContext.Floats)),
                    LightScriptType.Color => Expression.Property(ctx, nameof(ILightScriptContext.Colors)),
                    LightScriptType.Fixture => Expression.Property(ctx, nameof(ILightScriptContext.Fixtures)),
                    _ => throw new CompilationException(CompilationErrorType.UnsupportedVariableType, $"Unsupported variable type: '{variable.Type}'")
                };

                return Expression.Property(bag, "Item", Expression.Constant(variable.Name));
            }
        }

        private static class Methods
        {
            public static readonly MethodInfo Pow = typeof(Mathf).GetMethod(nameof(Mathf.Pow));
        }

        private static readonly char[] operatorChars = "-+*/%^<>".ToCharArray();
        private static bool IsOperator(char c) => operatorChars.Contains(c);

        private static readonly char[] singleChars = "().=".ToCharArray();
        private static bool IsSingleChar(char c) => singleChars.Contains(c);

    }
}