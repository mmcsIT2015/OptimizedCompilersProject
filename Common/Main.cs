using System;
using System.IO;
using System.Collections.Generic;
using SimpleLang;
using CompilerExceptions;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main()
        {
            List<string> files = new List<string>();

            files.Add(@"..\..\in.pasn");
            //files.Add(@"..\..\test_cso.txt"); // Тест для оптимизации: Устранение общих выражений

            foreach (var file in files)
            {
                try
                {
                    var root = FileLoader.LoadFile(file, System.Text.Encoding.UTF8);
                    
                    Console.WriteLine("Syntax tree ready");

                    Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
                    codeGenerator.Visit(root);

                        // DEBUG Can watch result here
                    var code = codeGenerator.CreateCode();
                    Console.WriteLine(code);                    
                }
                catch (FileNotFoundException e)
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
