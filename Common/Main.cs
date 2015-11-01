using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SimpleLang;

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

                    Scanner scanner = new Scanner();
                    scanner.SetSource(content, 0);

                    Parser parser = new Parser(scanner);

                    if (parser.Parse()) 
                    {
                        Console.WriteLine("Синтаксическое дерево построено");

                        NewGen3AddrCodeVisitor codeGenerator = new NewGen3AddrCodeVisitor();
                        codeGenerator.Visit(parser.root);

                        // DEBUG Can watch result here
                        Console.WriteLine(codeGenerator.Code);
                    }
                    else Console.WriteLine("Ошибка");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Файл {0} не найден", file);
                }
                catch (LexException e)
                {
                    Console.WriteLine("Лексическая ошибка. " + e.Message);
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine("Синтаксическая ошибка. " + e.Message);
                }
            }

            Console.ReadLine();
        }

    }
}
