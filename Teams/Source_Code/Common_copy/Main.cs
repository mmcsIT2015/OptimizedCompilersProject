﻿using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SimpleLang;
using SimpleLang.Optimization;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main()
        {
            List<string> files = new List<string>();
            if (File.Exists(@"..\..\tests.txt"))
            {
                foreach (var file in File.ReadAllLines(@"..\..\tests.txt"))
                {
                    files.Add(file);
                }
            }

            if (files.Count == 0)
            {
                files.Add(@"..\..\a.txt");
            }

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

                        Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
                        codeGenerator.Visit(parser.root);

                        // DEBUG Can watch result here
                        //Console.WriteLine(codeGenerator.Code);

                        BaseBlocksPartition baseBlocksPartition = new BaseBlocksPartition(codeGenerator.Code);
                        Console.WriteLine(codeGenerator.Code);

                        Console.WriteLine("_______________________________________");

                        CommonSubexpressionsOptimization opt = new CommonSubexpressionsOptimization(codeGenerator.Code);
                        opt.Optimize();
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