using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PascalABCCompiler.Errors;
using PascalABCCompiler;
using PascalABCCompiler.SyntaxTree;
using PascalABCCompiler.ParserTools;

using SyntaxVisitors;

namespace ParsePABC
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = System.IO.Path.GetFullPath(@"..\\..\\a.pas");

            Compiler compiler = new Compiler();
            compiler.SyntaxTreeChanger = new SyntaxTreeChanger();
            var opts = new CompilerOptions(filename, CompilerOptions.OutputType.ConsoleApplicaton);
            opts.Optimise = false;
            opts.GenerateCode = true;

            var output = compiler.Compile(opts);

            if (compiler.ErrorsList.Count > 0)
            {
                foreach (var error in compiler.ErrorsList)
                {
                    Console.WriteLine(error.ToString());
                }
            }

            Console.WriteLine("Output: " + output);

            Console.ReadKey();
        }
    }
}
