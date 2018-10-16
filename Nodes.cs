using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа__6
{
    interface INode { }

    interface IExpression : INode
    {
        object Accept(IExpressionVisitor visitor);
    }

    interface IStatement : INode
    {
        void Accept(IStatementVisitor visitor);
    }

    enum OperationType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Remainder,
        UnaryMinus,
        LogicalNegation,
        Equality,
        Relation
    }

    class BinaryOperation : IExpression
    {
        static readonly IReadOnlyDictionary<OperationType, string> OperationSymbol = new Dictionary<OperationType, string>() {
            { OperationType.Addition, "+" },
            { OperationType.Subtraction, "-" },
            { OperationType.Multiplication, "*" },
            { OperationType.Division, "/" },
            { OperationType.Remainder, "%" },
            { OperationType.Equality, "==" },
            { OperationType.Relation, "<" }
        };

        static readonly IReadOnlyDictionary<OperationType, string> OperationShortName = new Dictionary<OperationType, string>() {
            { OperationType.Addition, "Sum" },
            { OperationType.Subtraction, "Sub" },
            { OperationType.Multiplication, "Mult" },
            { OperationType.Division, "Div" },
            { OperationType.Remainder, "Mod" },
            { OperationType.Equality, "Equal" },
            { OperationType.Relation, "Compare" }
        };

        public readonly IExpression Left;
        public readonly OperationType Operation;
        public readonly IExpression Right;

        public BinaryOperation(IExpression left, OperationType operation, IExpression right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public object Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }

    class UnaryOperation : IExpression
    {
        static readonly IReadOnlyDictionary<OperationType, string> OperationSymbol = new Dictionary<OperationType, string>() {
            { OperationType.UnaryMinus, "-" },
            { OperationType.LogicalNegation, "!" }
        };

        static readonly IReadOnlyDictionary<OperationType, string> OperationShortName = new Dictionary<OperationType, string>() {
            { OperationType.UnaryMinus, "Minus" },
            { OperationType.LogicalNegation, "Not" }
        };

        public readonly OperationType Operation;
        public readonly IExpression Expression;

        public UnaryOperation(OperationType operation, IExpression expression)
        {
            Operation = operation;
            Expression = expression;
        }

        public object Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }

    class Number : IExpression
    {
        public readonly Token Token;

        public Number(Token token)
        {
            Token = token;
        }

        public object Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }

    class Identifier : IExpression
    {
        public readonly string Value;

        public Identifier(string value)
        {
            Value = value;
        }

        public object Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }

    class Paren : IExpression
    {
        public readonly IExpression Value;

        public Paren(IExpression value)
        {
            Value = value;
        }

        public object Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }

    class CallExpression : IExpression
    {
        public readonly IExpression Function;

        public readonly IReadOnlyList<IExpression> Arguments;

        public CallExpression(IExpression function, IReadOnlyList<IExpression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public object Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }


    class IfStatement : IStatement
    {
        public readonly IExpression Condition;

        public readonly Block Block;

        public IfStatement(IExpression condition, Block block)
        {
            Condition = condition;
            Block = block;
        }

        public void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }

    class WhileStatement : IStatement
    {
        public readonly IExpression Condition;

        public readonly Block Block;

        public WhileStatement(IExpression condition, Block block)
        {
            Condition = condition;
            Block = block;
        }

        public void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }

    class ExpressionStatement : IStatement
    {
        public readonly IExpression Expression;

        public ExpressionStatement(IExpression expression)
        {
            Expression = expression;
        }

        public void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }

    class Assignment : IStatement
    {
        public readonly string Variable;

        public readonly IExpression Expression;

        public Assignment(string variable, IExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }

    class Break : IStatement
    {
        public void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }

    class Block : INode
    {
        public readonly IReadOnlyList<IStatement> Statements;

        public Block(IReadOnlyList<IStatement> statements)
        {
            Statements = statements;
        }
    }

    class ProgramCode : INode
    {
        public readonly IReadOnlyList<IStatement> Statements;

        public ProgramCode(IReadOnlyList<IStatement> statements)
        {
            Statements = statements;
        }
    }
}
