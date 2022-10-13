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

                Log("Building abstract syntax tree");
                AbstractSyntaxTree ast = BuildAst(tokens, data);
                Log("Abstract syntax tree built");

                Log("Compiling abstract syntax tree");
                LightScriptAction action = CompileAst(ast, data);
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

        public static void ValidateTokens(LightScriptToken[] tokens) => tokens
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

        public static ILightScriptContext BuildContext(LightScriptData data)
        {
            LightScriptContext context = new();

            context.AddVariable(new(LightScriptType.Float, "t"));
            context.AddVariable(new(LightScriptType.Float, "dt"));
            context.AddVariable(new(LightScriptType.Integer, "frame"));

            data.fixtures.ForEach(fixture => context.AddVariable(new(LightScriptType.Fixture, fixture.name)));

            return context;
        }

        public static AbstractSyntaxTree BuildAst(LightScriptToken[] tokens, LightScriptData data)
        {
            LightScriptToken[][] scriptTokens = tokens
                .Split(token => token.type == TokenType.StatementEnding);

            SyntaxNode[] statements = scriptTokens.Select(BuildSyntaxNodeFromTokens);

            AbstractSyntaxTree result = new(statements);

            return result;
        }

        

        private static SyntaxNode BuildSyntaxNodeFromTokens(LightScriptToken[] tokens)
        {
            // Assertion made here : all statements are assignments
            (var leftSegment, var rightSegment) = tokens.Separate(token => token.type == TokenType.Assignment);

            var lhs = BuildSyntaxNodeFromArraySegment(leftSegment);
            var rhs = BuildSyntaxNodeFromArraySegment(rightSegment);

            return new AssignmentNode(
                lhs,
                rhs
            );
        }

        private enum OperationStack
        {
            Addition,
            Substraction,
            Multiplication,
            Division,
            Group
        }

        private static SyntaxNode BuildSyntaxNodeFromArraySegment(ArraySegment<LightScriptToken> tokens)
        {
            Stack<TokenType> operationStack = new();
            Stack<SyntaxNode> resultStack = new();

            foreach (var token in tokens)
            {
                string content = token.content;

                switch (token.type)
                {
                    case TokenType.Identifier:
                        throw new NotImplementedException();
                        break;
                    case TokenType.Number:
                        ConstantNode constantNode = int.TryParse(content, out int intResult) ?
                            new(intResult) :
                            new(float.Parse(content));
                            
                        resultStack.Push(constantNode);
                        
                        break;
                    case TokenType.Operator:
                        throw new NotImplementedException();
                        break;
                    case TokenType.DotNotation:
                        throw new NotImplementedException();
                        break;
                    case TokenType.LeftBracket:
                        throw new NotImplementedException();
                        break;
                    case TokenType.RightBracket:
                        throw new NotImplementedException();
                        break;
                }
            }

            return resultStack.First();
        }


        private static LightScriptAction CompileAst(AbstractSyntaxTree ast, LightScriptData data)
        {
            throw new NotImplementedException();
        }

        private static char[] operatorChars = "-+*/%<>".ToCharArray();
        private static bool IsOperator(char c) => operatorChars.Contains(c);

        private static char[] singleChars = "().=".ToCharArray();
        private static bool IsSingleChar(char c) => singleChars.Contains(c);

    }
}