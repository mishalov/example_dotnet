using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа__6
{
    interface ICallable
    {
        object Call(params object[] args);
    }

    class DumpFunction : ICallable
    {
        public object Call(params object[] args)
        {
            Console.WriteLine(string.Join(" ", args.Select(x => x ?? "null")));
            return this;
        }
    }

    interface IExpressionVisitor
    {
        object Visit(BinaryOperation expression);
        object Visit(UnaryOperation expression);
        object Visit(CallExpression expression);
        object Visit(Paren expression);
        object Visit(Number expression);
        object Visit(Identifier expression);
    }

    class Calculator : IExpressionVisitor
    {
        static readonly IReadOnlyDictionary<string, object> constants = new Dictionary<string, object>() {
            { "true", true },
            { "false", false },
            { "null", null },
            { "dump", new DumpFunction() }
        };

        public Dictionary<string, object> variables;

        public Calculator(Dictionary<string, object> variables)
        {
            this.variables = variables.Concat(constants.Where(x => !variables.ContainsKey(x.Key))).ToDictionary(x => x.Key, x => x.Value);
        }

        object Calc(IExpression expression) => expression.Accept(this);

        public object Visit(BinaryOperation expression)
        {
            var left = Calc(expression.Left);
            var right = Calc(expression.Right);
            if (left is null || right is null)
            {
                switch (expression.Operation)
                {
                    case OperationType.Equality:
                        return left == right;
                }
                throw new Exception("Неизвестная операция");
            }
            if (left is int && right is int)
            {
                int leftInt = (int)left;
                int rightInt = (int)right;
                switch (expression.Operation)
                {
                    case OperationType.Addition:
                        return leftInt + rightInt;
                    case OperationType.Subtraction:
                        return leftInt - rightInt;
                    case OperationType.Multiplication:
                        return leftInt * rightInt;
                    case OperationType.Division:
                        return leftInt / rightInt;
                    case OperationType.Remainder:
                        return leftInt % rightInt;
                    case OperationType.Equality:
                        return leftInt == rightInt;
                    case OperationType.Relation:
                        return leftInt < rightInt;
                }
                throw new Exception("Неизвестная операция");
            }
            if (left is bool && right is bool)
            {
                bool leftBool = (bool)left;
                bool rightBool = (bool)right;
                switch (expression.Operation)
                {
                    case OperationType.Equality:
                        return leftBool == rightBool;
                    case OperationType.Relation:
                        return (!leftBool && rightBool);
                }
                throw new Exception("Неизвестная операция");
            }
            throw new Exception("Неверный тип операндов");
        }

        public object Visit(UnaryOperation expression)
        {
            switch (expression.Operation)
            {
                case OperationType.UnaryMinus:
                    return -(int)Calc(expression.Expression);
                case OperationType.LogicalNegation:
                    return !(bool)Calc(expression.Expression);
            }
            throw new Exception("Неизвестная операция");
        }

        public object Visit(Paren expression) => Calc(expression.Value);

        public object Visit(CallExpression expression)
        {
            var function = Calc(expression.Function);
            if(function is ICallable)
            {
                return ((ICallable)function).Call(expression.Arguments.Select(x => Calc(x)).ToArray());
            }
            throw new Exception("Вызвали не функцию");
        }

        public object Visit(Number expression) => int.Parse(expression.Token.Lexeme);

        public object Visit(Identifier expression)
        {
            if (variables.TryGetValue(expression.Value, out object value))
            {
                return value;
            }
            throw new Exception("Неизвестная переменная");
        }
    }
}
