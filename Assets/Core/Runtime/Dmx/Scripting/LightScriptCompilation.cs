using Plml.Dmx.Scripting.Compilation;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using TokenType = Plml.Dmx.Scripting.Compilation.LightScriptTokenType;

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
                IReadOnlyCollection<LightScriptToken> tokens = Tokenize(data);
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

        public static IReadOnlyCollection<LightScriptToken> Tokenize(LightScriptData data)
        {
            List<LightScriptToken> result = new();

            string src = Regex.Replace(data.text, "( )+", " ");
            int startPosition = 0;
            int i = 0;
            for (; i < src.Length; i++)
            {
                switch (src[i])
                {
                    case '+':
                    case '-':
                    case '*':
                    case '%':
                    case '<':
                    case '>':
                        PushValue(new(TokenType.Operator));
                        break;
                    case ' ':
                        PushTextValue();
                        break;
                    case ';':
                    case '\n':
                        PushValue(new(TokenType.StatementEnding));
                        break;
                    default:
                        startPosition++;
                        continue;
                }
            }

            void PushTextValue()
            {
                if (i != startPosition)
                {
                    LightScriptToken textToken = new(TokenType.Text, src.Substring(startPosition, startPosition - i));
                    result.Add(textToken);
                }
            }

            void PushValue(LightScriptToken token)
            {
                PushTextValue();

                result.Add(token);
                startPosition = 0;
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
    }
}