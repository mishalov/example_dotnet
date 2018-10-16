using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа__6
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = File.ReadAllText(@"..\..\code.txt");
            var parser = new Parser(Token.GetTokens(code).Where(x => x.Type != TokenType.Whitespace).ToList());
            var programCode = parser.ParseProgram();
            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "x", 2 },
                { "y", 10 },
                { "z", 4 }
            };
            Run(programCode, variables);
        }

        static void Run(ProgramCode node, Dictionary<string, object> variables) => new Interpreter(variables).Run(node.Statements);

    }
}
