using System;
using System.Collections.Generic;
using System.Globalization;

namespace Plml.Jace.Parsing
{
    public class TokenReader
    {
        private readonly CultureInfo cultureInfo;
        private readonly char decimalSeparator;
        private readonly char argumentSeparator;

        public TokenReader()
        {
            cultureInfo = CultureInfo.InvariantCulture;
            this.decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator[0];
            this.argumentSeparator = cultureInfo.TextInfo.ListSeparator[0];
        }

        public List<Token> Read(string formula)
        {
            if (string.IsNullOrEmpty(formula))
                throw new ArgumentNullException("formula");

            List<Token> tokens = new List<Token>();

            char[] characters = formula.ToCharArray();

            bool isFormulaSubPart = true;
            bool isScientific = false;

            for(int i = 0; i < characters.Length; i++)
            {
                if (IsPartOfNumeric(characters[i], true, isFormulaSubPart))
                {
                    string buffer = "" + characters[i];
                    int startPosition = i;
                                       

                    while (++i < characters.Length && IsPartOfNumeric(characters[i], false, isFormulaSubPart))
                    {
                        if (isScientific && IsScientificNotation(characters[i]))
                            throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");

                        if (IsScientificNotation(characters[i]))
                        {
                            isScientific = IsScientificNotation(characters[i]);

                            if (characters[i + 1] == '-')
                            {
                                buffer += characters[i++];
                            }
                        }

                        buffer += characters[i];
                    }

                    if (float.TryParse(buffer, NumberStyles.Float | NumberStyles.AllowThousands,
                        cultureInfo, out float doubleValue))
                    {
                        tokens.Add(new Token() { TokenType = TokenType.FloatingPoint, Value = doubleValue, StartPosition = startPosition, Length = i - startPosition });
                        isFormulaSubPart = false;
                    }
                    else if (buffer == "-")
                    {
                        // Verify if we have a unary minus, we use the token '_' for a unary minus in the AST builder
                        tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '_', StartPosition = startPosition, Length = 1 });
                    }

                    if (i == characters.Length)
                    {
                        // Last character read
                        continue;
                    }
                }

                if (IsPartOfVariable(characters[i], true))
                {
                    string buffer = "" + characters[i];
                    int startPosition = i;

                    while (++i < characters.Length && IsPartOfVariable(characters[i], false))
                    {
                        buffer += characters[i];
                    }

                    tokens.Add(new Token() { TokenType = TokenType.Text, Value = buffer, StartPosition = startPosition, Length = i -startPosition });
                    isFormulaSubPart = false;

                    if (i == characters.Length)
                    {
                        // Last character read
                        continue;
                    }
                }
                if (characters[i] == this.argumentSeparator)
                {
                    tokens.Add(new Token() { TokenType = TokenType.ArgumentSeparator, Value = characters[i], StartPosition = i, Length = 1 });
                    isFormulaSubPart = false;
                }
                else
                {
                    switch (characters[i])
                    { 
                        case ' ':
                            continue;
                        case '+':
                        case '-':
                        case '*':
                        case '/':
                        case '^':
                        case '%':
                        case '≤':
                        case '≥':
                        case '≠':
                            if (IsUnaryMinus(characters[i], tokens))
                            {
                                // We use the token '_' for a unary minus in the AST builder
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '_', StartPosition = i, Length = 1 });
                            }
                            else
                            {
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = characters[i], StartPosition = i, Length = 1 });                            
                            }
                            isFormulaSubPart = true;
                            break;
                        case '(':
                            tokens.Add(new Token() { TokenType = TokenType.LeftBracket, Value = characters[i], StartPosition = i, Length = 1 });
                            isFormulaSubPart = true;
                            break;
                        case ')':
                            tokens.Add(new Token() { TokenType = TokenType.RightBracket, Value = characters[i], StartPosition = i, Length = 1 });
                            isFormulaSubPart = false;
                            break;
                        case '<':
                            if (i + 1 < characters.Length && characters[i + 1] == '=')
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '≤', StartPosition = i++, Length = 2 });
                            else
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '<', StartPosition = i, Length = 1 });
                            isFormulaSubPart = false;
                            break;
                        case '>':
                            if (i + 1 < characters.Length && characters[i + 1] == '=')
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '≥', StartPosition = i++, Length = 2 });
                            else
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '>', StartPosition = i, Length = 1 });
                            isFormulaSubPart = false;
                            break;
                        case '!':
                            if (i + 1 < characters.Length && characters[i + 1] == '=')
                            {
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '≠', StartPosition = i++, Length = 2 });
                                isFormulaSubPart = false;
                            }
                            else
                                throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                            break;
                        case '&':
                            if (i + 1 < characters.Length && characters[i + 1] == '&')
                            {
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '&', StartPosition = i++, Length = 2 });
                                isFormulaSubPart = false;
                            }
                            else
                                throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                            break;
                        case '|':
                            if (i + 1 < characters.Length && characters[i + 1] == '|')
                            {
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '|', StartPosition = i++, Length = 2 });
                                isFormulaSubPart = false;
                            }
                            else
                                throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                            break;
                        case '=':
                            if (i + 1 < characters.Length && characters[i + 1] == '=')
                            {
                                tokens.Add(new Token() { TokenType = TokenType.Operation, Value = '=', StartPosition = i++, Length = 2 });
                                isFormulaSubPart = false;
                            }
                            else
                                throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                            break;
                        default:
                            throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                    }
                }
            }

            return tokens;
        }

        private bool IsPartOfNumeric(char character, bool isFirstCharacter, bool isFormulaSubPart)
        {
            return character == decimalSeparator || (character >= '0' && character <= '9') || (isFormulaSubPart && isFirstCharacter && character == '-') || (!isFirstCharacter && character == 'e') || (!isFirstCharacter && character == 'E');
        }

        private bool IsPartOfVariable(char character, bool isFirstCharacter)
        {
            return (character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z') || (!isFirstCharacter && character >= '0' && character <= '9') || (!isFirstCharacter && character == '_');
        }

        private bool IsUnaryMinus(char currentToken, List<Token> tokens)
        {
            if (currentToken == '-')
            {
                Token previousToken = tokens[tokens.Count - 1];

                return !(previousToken.TokenType == TokenType.FloatingPoint ||
                         previousToken.TokenType == TokenType.FloatingPoint ||
                         previousToken.TokenType == TokenType.Text ||
                         previousToken.TokenType == TokenType.RightBracket);
            }
            else
                return false;
        }

        private bool IsScientificNotation(char currentToken)
        {
            return currentToken == 'e' || currentToken == 'E';
        }
    }
}