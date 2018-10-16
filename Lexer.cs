using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Лабораторная_работа__6
{
    enum TokenType
    {
        Whitespace,
        Identifier,
        NumberLiteral,
        OperatorOrPunctuator,
        EOF
    }

    class Token
    {
        static readonly Dictionary<string, TokenType> table = new Dictionary<string, TokenType>() {
            { "ws", TokenType.Whitespace },
            { "id", TokenType.Identifier },
            { "num", TokenType.NumberLiteral },
            { "op", TokenType.OperatorOrPunctuator }
        };

        public readonly TokenType Type;
        public readonly string Lexeme;

        public Token(TokenType type, string lexeme)
        {
            Type = type;
            Lexeme = lexeme;
        }

        public override string ToString() => $"{Type} \"{Lexeme}\"";

        static public IEnumerable<Token> GetTokens(string text)
        {
            string pattern = @"
            (?<ws>\s+)|
            (?<id>[\w_-[\d]][\w_]*)|
            (?<num>\d+)|
            (?<op>==|[+-/*%!<,()=;{}])
            ";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            var matches = regex.Matches(text);
            int lastPos = 0;
            string[] groupNames = regex.GetGroupNames();
            foreach (Match m in matches)
            {
                if (lastPos < m.Index)
                {
                    throw new Exception($"Пропустили {text.Substring(lastPos, m.Index - lastPos)}");
                }
                bool Success = false;
                for (int i = 1; i < groupNames.Length && !Success; i++)
                {
                    if (m.Groups[groupNames[i]].Success)
                    {
                        Success = true;
                        yield return new Token(table[groupNames[i]], m.Value);
                    }
                }
                if (!Success)
                {
                    throw new Exception("Кривая регулярка");
                }
                lastPos = m.Index + m.Length;
            }
            if (lastPos < text.Length)
            {
                throw new Exception($"Пропустили {text.Substring(lastPos)}");
            }
        }
    }
}
