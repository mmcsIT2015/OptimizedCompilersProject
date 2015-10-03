﻿using System;
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
                files.Add(@"..\..\test_cso.txt");
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

                        // DEBUG Разбиение на внутренние блоки + построение графа переходов
                        BaseBlocksPartition.Partition(codeGenerator.Code);
                        // DEBUG Получить обратный граф переходов
                        // codeGenerator.Code.graph = codeGenerator.Code.GetReversedGraph();

                        // DEBUG Can watch result here
                        //Console.WriteLine(codeGenerator.Code);

                        DeadCodeElimination deadCodeElimination = new DeadCodeElimination(codeGenerator.Code, 1);
                        deadCodeElimination.Optimize();
                        Console.WriteLine(codeGenerator.Code);

                        // DEBUG Оптимизация: Устранение общих выражений
                        /*
                        SimpleLang.Optimization.CommonSubexpressionsOptimization cso = new SimpleLang.Optimization.CommonSubexpressionsOptimization(codeGenerator.Code);
                        cso.Optimize();
                        Console.WriteLine("Optimization:\n" + codeGenerator.Code);
                        */

                        //DEBUG def-use data view
                        /*               
                        codeGenerator.Code.blocks[0].CalculateDefUseData();
                        for (int i = 0; i < codeGenerator.Code.blocks[0].Count; i++)
                        {
                            foreach (string variable in codeGenerator.Code.blocks[0].GetAliveVariables(i))
                            {
                                Console.Write(variable + " ");
                            }
                            Console.WriteLine();
                        }
                        */

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
