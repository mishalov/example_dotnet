using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*

*Program:
    Statement*
Statement:
    ExpressionStatement | Assignment | If | While | Break
ExpressionStatement:
    Expression ';'
Assignment:
    Identifier '=' Expression ';'
If:
    'if' '(' Expression ')' Block
While:
    'while' '(' Expression ')' Block
Block:
    '{' Statement* '}'
Break:
    'break' ';'
Expession:
    Equality
Equality:
    Equality "==" Relational
    Relational
Relational:
    Relational "<" Additive
    Additive
Additive:
    Additive ("+"|"-") Multiplicative
    Multiplicative
Multiplicative:
    Multiplicative ("*"|"/"|"%") Unary
    Unary
Unary:
    ("-"|"!") Unary
    Call
Call:
    Call '(' Parameters? ')'
    Primary
Primary:
    '(' Expression ')'
    Number
    Identifier
Parameters:
    Expression (',' Expression)*

*/

namespace Лабораторная_работа__6
{
    class Parser
    {
        readonly IReadOnlyList<Token> tokens;
        int position = 0;
        Token CurrentToken => tokens[position];

        public Parser(IEnumerable<Token> tokens)
        {
            var t = tokens.ToList();
            t.Add(new Token(TokenType.EOF, ""));
            this.tokens = t;
        }

        void ExpectEof()
        {
            if (!IsType(TokenType.EOF))
            {
                throw ParserError($"Не допарсили до конца, остался {CurrentToken}");
            }
        }

        void ReadNextToken()
        {
            position += 1;
        }

        void Reset()
        {
            position = 0;
        }

        Exception ParserError(string message)
        {
            var result = string.Join(" ", tokens.Select(x => x.Lexeme));
            var length = tokens.Take(position).Select(x => x.Lexeme.Length + 1).Sum();
            var pointer = new string(' ', length) + '^';
            return new Exception(string.Join("\n", message, result, pointer));
        }

        bool SkipIf(string s)
        {
            if (string.Equals(CurrentToken.Lexeme, s, StringComparison.Ordinal))
            {
                ReadNextToken();
                return true;
            }
            return false;
        }

        bool CurrentIs(string s) => string.Equals(CurrentToken.Lexeme, s, StringComparison.Ordinal);

        bool IsType(TokenType type) => CurrentToken.Type == type;

        void Expect(string s)
        {
            if (!SkipIf(s))
            {
                throw ParserError($"Ожидали \"{s}\", получили {CurrentToken}");
            }
        }

        Token ParseNumber()
        {
            if (!IsType(TokenType.NumberLiteral))
            {
                throw ParserError($"Ожидали число, получили {CurrentToken}");
            }
            var token = CurrentToken;
            ReadNextToken();
            return token;
        }

        string ParseIdentifier()
        {
            if (!IsType(TokenType.Identifier))
            {
                throw ParserError($"Ожидали идентификатор, получили {CurrentToken}");
            }
            var identifier = CurrentToken.Lexeme;
            ReadNextToken();
            return identifier;
        }

        IExpression ParsePrimaryExpression()
        {
            if (SkipIf("("))
            {
                var expression = new Paren(ParseEqualityExpression());
                Expect(")");
                return expression;
            }

            if (IsType(TokenType.NumberLiteral))
            {
                return new Number(ParseNumber());
            }

            return new Identifier(ParseIdentifier());
        }

        IExpression ParseCallExpression()
        {
            var expression = ParsePrimaryExpression();
            if (CurrentIs("("))
            {
                while (SkipIf("("))
                {
                    var arguments = new List<IExpression>();
                    if (!CurrentIs(")"))
                    {
                        arguments.Add(ParseExpression());
                        while (SkipIf(","))
                        {
                            arguments.Add(ParseExpression());
                        }
                    }
                    Expect(")");
                    expression = new CallExpression(expression, arguments);
                }
            }
            return expression;
        }

        IExpression ParseUnaryExpression()
        {
            if (SkipIf("-"))
            {
                var expression = ParseUnaryExpression();
                return new UnaryOperation(OperationType.UnaryMinus, expression);
            }
            else if (SkipIf("!"))
            {
                var expression = ParseUnaryExpression();
                return new UnaryOperation(OperationType.LogicalNegation, expression);
            }
            return ParseCallExpression();
        }

        IExpression ParseMultiplicativeExpression()
        {
            var left = ParseUnaryExpression();
            while (true)
            {
                if (SkipIf("*"))
                {
                    var right = ParseUnaryExpression();
                    left = new BinaryOperation(left, OperationType.Multiplication, right);
                }
                else if (SkipIf("/"))
                {
                    var right = ParseUnaryExpression();
                    left = new BinaryOperation(left, OperationType.Division, right);
                }
                else if (SkipIf("%"))
                {
                    var right = ParseUnaryExpression();
                    left = new BinaryOperation(left, OperationType.Remainder, right);
                }
                else break;
            }
            return left;
        }

        IExpression ParseAdditiveExpression()
        {
            var left = ParseMultiplicativeExpression();
            while (true)
            {
                if (SkipIf("+"))
                {
                    var right = ParseMultiplicativeExpression();
                    left = new BinaryOperation(left, OperationType.Addition, right);
                }
                else if (SkipIf("-"))
                {
                    var right = ParseMultiplicativeExpression();
                    left = new BinaryOperation(left, OperationType.Subtraction, right);
                }
                else break;
            }
            return left;
        }

        IExpression ParseRelationalExpression()
        {
            var left = ParseAdditiveExpression();
            while (true)
            {
                if (SkipIf("<"))
                {
                    var right = ParseAdditiveExpression();
                    left = new BinaryOperation(left, OperationType.Relation, right);
                }
                else break;
            }
            return left;
        }

        IExpression ParseEqualityExpression()
        {
            var left = ParseRelationalExpression();
            while (true)
            {
                if (SkipIf("=="))
                {
                    var right = ParseRelationalExpression();
                    left = new BinaryOperation(left, OperationType.Equality, right);
                }
                else break;
            }
            return left;
        }

        IExpression ParseExpression() => ParseEqualityExpression();

        Block ParseBlock()
        {
            Expect("{");
            var statements = new List<IStatement>();
            while (!SkipIf("}"))
            {
                statements.Add(ParseStatement());
            }
            return new Block(statements);
        }

        WhileStatement ParseWhileStatement()
        {
            Expect("while");
            Expect("(");
            var condition = ParseExpression();
            Expect(")");
            var block = ParseBlock();
            return new WhileStatement(condition, block);
        }

        IfStatement ParseIfStatement()
        {
            Expect("if");
            Expect("(");
            var condition = ParseExpression();
            Expect(")");
            var block = ParseBlock();
            return new IfStatement(condition, block);
        }

        IStatement ParseStatement()
        {
            if (CurrentIs("if"))
            {
                return ParseIfStatement();
            }
            if (CurrentIs("while"))
            {
                return ParseWhileStatement();
            }
            if (SkipIf("break"))
            {
                Expect(";");
                return new Break();
            }
            var expression = ParseExpression();
            if (SkipIf("="))
            {
                if (!(expression is Identifier))
                {
                    throw ParserError("Присваивание не в переменную");
                }
                var restAssigmentExpression = ParseExpression();
                Expect(";");
                return new Assignment((expression as Identifier).Value, restAssigmentExpression);
            }
            else
            {
                Expect(";");
                return new ExpressionStatement(expression);
            }
        }

        ProgramCode ParseProgramCode()
        {
            var statements = new List<IStatement>();
            while (!IsType(TokenType.EOF))
            {
                statements.Add(ParseStatement());
            }
            return new ProgramCode(statements);
        }

        public ProgramCode ParseProgram()
        {
            Reset();
            var result = ParseProgramCode();
            ExpectEof();
            return result;
        }
    }
}
