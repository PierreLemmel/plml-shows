using Plml.Dmx.Scripting.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

using TokenType = Plml.Dmx.Scripting.Compilation.LightScriptTokenType;
using TokenizationContext = Plml.Dmx.Scripting.Compilation.LightScriptTokenizationContext;
using NumberType = Plml.Dmx.Scripting.Compilation.LightScriptTokenizationNumberTypeContext;


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
                IReadOnlyCollection<LightScriptToken> tokens = Tokenize(src);
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

        public static IReadOnlyCollection<LightScriptToken> Tokenize(string text)
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
                    TokenizationContext.Identifier => IsIdentifierCharacter(c),
                    TokenizationContext.Number => IsNumberCharacter(c),
                    _ => false
                };

                if (!charIsValidInCurrentContext)
                {
                    charContext = c switch
                    {
                        ' ' => TokenizationContext.TokenEnd,
                        '\n' or ';' => TokenizationContext.StatementEnding,
                        _ when char.IsDigit(c) => TokenizationContext.Number,
                        _ when IsSingleChar(c) => TokenizationContext.SingleChar,
                        _ when char.IsLetter(c) => TokenizationContext.Identifier,
                        _ when IsOperator(c) => TokenizationContext.Operator,
                        _ => throw new TokenizationException($"Unexpected character: '{c}'")
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
                            '=' => TokenType.Assignation,
                            _ => throw new TokenizationException($"Unexpected single char: '{sc}'")
                        };
                        result.Add(new(type));

                        break;
                    case TokenizationContext.Operator:
                        result.Add(new(TokenType.Operator, GetCurrentString()));
                        break;
                }
            }
            
            string GetCurrentString() => src[startPosition..i];

            bool IsNumberCharacter(char c)
            {
                if (char.IsDigit(c))
                    return true;
                else if (tokenContext == TokenizationContext.Number)
                {
                    if (numberType == NumberType.Integer)
                    {
                        if (c == '.' || c == 'x')
                            return true;
                    }
                    else if (numberType == NumberType.Hexadecimal)
                    {
                        if ("abcdef".Contains(char.ToLower(c)))
                            return true;
                    }
                }

                return false;
            }

            bool IsIdentifierCharacter(char c)
            {
                if (char.IsLetter(c))
                    return true;
                else if (tokenContext == TokenizationContext.Identifier)
                {
                    if (char.IsDigit(c))
                        return true;
                }

                return false;
            }

            return result;
        }

        public static AbstractSyntaxTree BuildAst(IReadOnlyCollection<LightScriptToken> tokens, LightScriptData data)
        {
            throw new NotImplementedException();
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