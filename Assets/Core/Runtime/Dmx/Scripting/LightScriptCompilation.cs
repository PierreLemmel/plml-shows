﻿using Plml.Dmx.Scripting.Compilation;
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
        }

        private static SyntaxNode BuildSyntaxNodeFromTokens(LightScriptToken[] tokens, ILightScriptCompilationContext context)
        {
            Stack<(OperationStackElementType Type, BinaryOperatorType? Operator)> operationStack = new();
            Stack<SyntaxNode> resultStack = new();

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

                                if (!context.TryGetVariable(content, out var variable))
                                    throw new SyntaxTreeException(CompilationErrorType.UnknownVariable, $"Unknown variable '{content}'");

                                VariableNode variableNode = new(variable.Type, variable.Name);
                                newObjectType = variable.Type;

                                PushResultToStack(variableNode);

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
                            new(float.Parse(content));

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
                
                operationStack.Push((OperationStackElementType.BinaryOperator, @operator));
            }

            void PushLeftBracket() => operationStack.Push((OperationStackElementType.LeftBracket, null));

            void PushRightBracket()
            {
                while (operationStack.Peek().Type != OperationStackElementType.LeftBracket)
                    PopOperationStack();

                operationStack.Pop();
            }

            void PopOperationStack()
            {
                var topElt = operationStack.Pop();

                if (topElt.Type != OperationStackElementType.BinaryOperator)
                    throw new SyntaxTreeException(CompilationErrorType.InternalSyntaxTreeError, $"Unexpected operation element type: '{topElt.Type}'");

                var @operator = topElt.Operator.Value;

                if (!resultStack.TryPop(out SyntaxNode rhs))
                    throw new SyntaxTreeException(CompilationErrorType.NoLeftHandSideForOperator, $"Missing left hand side for {@operator} operator");
                if (!resultStack.TryPop(out SyntaxNode lhs))
                    throw new SyntaxTreeException(CompilationErrorType.NoLeftHandSideForOperator, $"Missing right hand side for {@operator} operator");

                if (LightScriptTypeSystem.IsValidOperatorType(@operator, lhs.Type, rhs.Type, out var operatorResultType))
                {
                    SyntaxNode operationNode = @operator switch
                    {
                        BinaryOperatorType.Addition => new AdditionNode(operatorResultType, lhs, rhs),
                        BinaryOperatorType.Substraction => new SubstractionNode(operatorResultType, lhs, rhs),
                        BinaryOperatorType.Multiplication => new MultiplicationNode(operatorResultType, lhs, rhs),
                        BinaryOperatorType.Division => new DivisionNode(operatorResultType, lhs, rhs),
                        BinaryOperatorType.Assignment => new AssignmentNode(operatorResultType, lhs, rhs),
                        BinaryOperatorType.Modulo => new ModuloNode(operatorResultType, lhs, rhs),
                        BinaryOperatorType.Exponentiation => new ExponentiationNode(operatorResultType, lhs, rhs),
                        _ => throw new SyntaxTreeException(CompilationErrorType.UnknownOperator, $"Unexpected operator '{@operator}'")
                    };

                    resultStack.Push(operationNode);
                }
                else
                    throw new SyntaxTreeException(CompilationErrorType.TypeError, $"Operator '{@operator}' does not exist between types '{lhs.Type}' and '{rhs.Type}'");
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
                                float lhsVal = lhsConstant.FloatValue;
                                float rhsVal = rhsConstant.FloatValue;

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

                AdditionNode addition => Expression.Add(
                    CompileNode(addition.LeftHandSide),
                    CompileNode(addition.RightHandSide)
                ),

                SubstractionNode substraction => Expression.Subtract(
                    CompileNode(substraction.LeftHandSide),
                    CompileNode(substraction.RightHandSide)
                ),

                MultiplicationNode multiplication => Expression.Multiply(
                    CompileNode(multiplication.LeftHandSide),
                    CompileNode(multiplication.RightHandSide)
                ),

                DivisionNode division => Expression.Subtract(
                    CompileNode(division.LeftHandSide),
                    CompileNode(division.RightHandSide)
                ),

                AssignmentNode assignment => Expression.Assign(
                    CompileNode(assignment.LeftHandSide),
                    CompileNode(assignment.RightHandSide)
                ),

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

        private static readonly char[] operatorChars = "-+*/%^<>".ToCharArray();
        private static bool IsOperator(char c) => operatorChars.Contains(c);

        private static readonly char[] singleChars = "().=".ToCharArray();
        private static bool IsSingleChar(char c) => singleChars.Contains(c);

    }
}