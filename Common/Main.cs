using System;
using System.IO;
using System.Collections.Generic;
using iCompiler;
using CompilerExceptions;
using iCompiler.Line;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {        
        
        public static void RunAllTests()
        {
            var extensions = new string[] { "*.cn", "*.pasn" };

            var directory = Directory.GetCurrentDirectory();
            directory = directory.Substring(0, directory.LastIndexOf('\\'));
            directory = directory.Substring(0, directory.LastIndexOf('\\'));

            var files = new List<string>();
            foreach (var ext in extensions)
            {
                var temp = Directory.GetFiles(directory, ext, SearchOption.AllDirectories);
                files.InsertRange(files.Count, temp);
            }

            ;
            foreach (var file in files)
            {
                var view = "...\\" + file.Substring(file.LastIndexOf("Common"));
                Console.Write("Parse file " + view + "... ");
                try
                {
                    var root = FileLoader.LoadFile(file, System.Text.Encoding.Default);
                    Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
                    codeGenerator.Visit(root);

                    var code = codeGenerator.CreateCode();
                    Console.WriteLine("done!");      
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found: " + file);
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
        }

        public static void Main()
        {
            bool runTests = false;
            if (runTests)
            {
                Testing.PerformAllTests();
            }
            else
            {
                List<string> files = new List<string>();
                //files.Add(@"..\..\tests\test-validation3.cn");
                //files.Add(@"..\..\tests\test-activevars1.cn");
                //files.Add(@"..\..\tests\test-realnum1.pasn");
                //files.Add(@"..\..\tests\test-dce2.cn");
                //files.Add(@"..\..\in.pasn");
                //files.Add(@"..\..\a.cn");
                //files.Add(@"..\..\test_cso.txt"); // Тест для оптимизации: Устранение общих выражений
                files.Add(@"..\..\tests\test-cp.cn");  
              //  files.Add(@"..\..\tests\test-graph1.pasn");
                //files.Add(@"..\..\tests\test-exprgenkill-3.cn");
                //files.Add(@"..\..\tests\test-exprgenkill-5.cn");

                List<Testing.RootTest> root_tests = new List<Testing.RootTest>();
                // Add your root tests here
                //root_tests.Add(Testing.TestConstantsPropagation_sets);
                root_tests.Add(Testing.TestConstantsPropagation);
                //root_tests.Add(Testing.TestDomIterativeAlogrithm);
                
                //\
                List<Testing.CodeTest> code_tests = new List<Testing.CodeTest>();
                // Add your code tests here

                //\

                Testing.PerformTests(files, root_tests, code_tests);
            }
            Console.ReadLine();
        }

    }
}
