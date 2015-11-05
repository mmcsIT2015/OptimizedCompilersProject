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

            files.Add(@"..\..\a.txt");
            //files.Add(@"..\..\test_cso.txt"); // Тест для оптимизации: Устранение общих выражений

            foreach (var file in files)
            {
                try
                {
                    string content = File.ReadAllText(file);
                    Console.Write("File ...\\" + file.Substring(file.LastIndexOf('\\')) + "... ");

                    SimpleScannerC.Scanner scanner = new SimpleScannerC.Scanner();
                    scanner.SetSource(content, 0);

                    SimpleParserC.Parser parser = new SimpleParserC.Parser(scanner);

                    if (parser.Parse()) 
                    {
                        Console.WriteLine("Syntax tree ready");

                        Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
                        codeGenerator.Visit(parser.root);

                        // DEBUG Can watch result here
                        var code = codeGenerator.CreateCode();
                        Console.WriteLine(code);
                    }
                    else Console.WriteLine("Unexpected error");
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
            }

            Console.ReadLine();
        }

    }
}
