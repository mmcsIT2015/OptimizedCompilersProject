using System;
using System.IO;
using System.Collections.Generic;
using Compiler;
using CompilerExceptions;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void TestIterativeAlgo(ThreeAddrCode code) 
        {
            var semilattice = new ReachDefSemilattice(code);
            var funcs = TransferFuncFactory.TransferFuncsForReachDef(code);
            var alg = new IterativeAlgo<ThreeAddrCode.Index, TransferFunction<ThreeAddrCode.Index>>(semilattice, funcs);

            alg.Run(code);

            foreach (var block in code.blocks)
            {
                Console.WriteLine("===");
                Console.WriteLine(block);
                Console.Write("In: ");
                foreach (var v in alg.In[block])
                {
                    Console.Write(v + " ");
                }
                Console.WriteLine();
                Console.Write("Out: ");
                foreach (var v in alg.Out[block])
                {
                    Console.Write(v + " ");
                }
                Console.WriteLine();
            }
        }
        
        public static void Main()
        {
            List<string> files = new List<string>();

            files.Add(@"..\..\tests\test-1.cn");
            //files.Add(@"..\..\tests\test-dce2.cn");
            //files.Add(@"..\..\in.pasn");
            //files.Add(@"..\..\a.cn");
            //files.Add(@"..\..\test_cso.txt"); // Тест для оптимизации: Устранение общих выражений

            foreach (var file in files)
            {
                try
                {
                    var root = FileLoader.LoadFile(file, System.Text.Encoding.UTF8);
                    
                    Console.WriteLine("Syntax tree ready");

                    Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
                    codeGenerator.Visit(root);

                    var code = codeGenerator.CreateCode();
                    Console.WriteLine(code);

                    //DeadCodeElimination deadCodeElimination = new DeadCodeElimination(code/*, 1*/);
                    //deadCodeElimination.Optimize();                   
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found: "+ file);
                }
                catch (LexException e)
                {
                    Console.WriteLine("Lexer error: " + e.Message);
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine("Syntax error: " + e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected error: " + e.Message);
                }
            }

            Console.ReadLine();
        }

    }
}
