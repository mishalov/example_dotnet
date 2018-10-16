using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа__6
{
    interface IStatementVisitor
    {
        void Visit(IfStatement statement);
        void Visit(WhileStatement statement);
        void Visit(ExpressionStatement statement);
        void Visit(Assignment statement);
        void Visit(Break statement);
    }

    class Interpreter : IStatementVisitor
    {
        Calculator calc;

        bool hasBreak;

        public Interpreter(Dictionary<string, object> variables)
        {
            calc = new Calculator(variables);
        }

        public void Run(IStatement statement) => statement.Accept(this);

        public void RunBlock(Block block) => Run(block.Statements);

        public void Run(IReadOnlyList<IStatement> statements)
        {
            foreach (var item in statements)
            {
                Run(item);
                if (hasBreak) break;
            }
        }

        public void Visit(IfStatement statement)
        {
            if ((bool)statement.Condition.Accept(calc))
            {
                RunBlock(statement.Block);
            }
        }

        public void Visit(WhileStatement statement)
        {
            while ((bool)statement.Condition.Accept(calc))
            {
                RunBlock(statement.Block);
                if (hasBreak)
                {
                    hasBreak = false;
                    break;
                }
            }
        }

        public void Visit(ExpressionStatement statement) => statement.Expression.Accept(calc);

        public void Visit(Assignment statement)
        {
            calc.variables[statement.Variable] = statement.Expression.Accept(calc);
        }

        public void Visit(Break statement)
        {
            hasBreak = true;
        }
    }
}
