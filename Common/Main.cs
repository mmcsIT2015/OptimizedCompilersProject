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
        public static void TestIterativeAlgo(ThreeAddrCode code) 
        {
            var semilattice = new ReachableDefSemilattice(code);
            var funcs = TransferFuncFactory.TransferFuncsForReachDef(code);
            var alg = new IterativeAlgo<ThreeAddrCode.Index, TransferFunction<ThreeAddrCode.Index>>(semilattice, funcs);

            alg.Run(code);

            foreach (var block in code.blocks)
            {
                Console.WriteLine();
                Console.WriteLine("---");
                Console.WriteLine("block: " + code.GetBlockId(block));
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

        public static void TestDomIterativeAlogrithm(ProgramTree.BlockNode root)
        {
            Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
            codeGenerator.Visit(root);

            var code = codeGenerator.CreateCode();

            WriteBlocksToConsole(code);

            Console.WriteLine("Doms");
            Dictionary<Block, IEnumerable<Block>> blockDoms = DomGraph.GenerateDomOut(code);
            foreach (Block block in blockDoms.Keys)
            {
                Console.Write("Dom({0}) =", code.blocks.IndexOf(block) + 1);

                foreach(Block domBlock in blockDoms[block])
                {
                    Console.Write(" {0};", code.blocks.IndexOf(domBlock) + 1);
                }

                Console.WriteLine();
            }

            Console.WriteLine("\nDom Tree");
            Dictionary<Block, List<Block>> tree = DomGraph.GenerateDomTree(code);
            foreach (Block block in tree.Keys)
            {
                Console.Write("Block({0}) <===> Childs:", code.blocks.IndexOf(block) + 1);

                foreach (Block domBlock in tree[block])
                {
                    Console.Write(" {0};", code.blocks.IndexOf(domBlock) + 1);
                }

                Console.WriteLine();
            }

            Console.WriteLine("\nReversed Edges");
            List<DomGraph.ValPair<Block>> listEdges = DomGraph.ReverseEdges(blockDoms, code.graph) as List<DomGraph.ValPair<Block>>;

            if (listEdges.Count == 0)
            {
                Console.WriteLine("There are no reversed edges");
                return;
            }

            for (int i = 0; i < listEdges.Count; ++i)
                Console.WriteLine("Reversed edge [{0}] == ({1} , {2} );", i + 1, code.blocks.IndexOf(listEdges[i].valBegin) + 1,
                    code.blocks.IndexOf(listEdges[i].valEnd) + 1);
        }

        public static void TestExpressionsGenKill(ProgramTree.BlockNode root)
        {
            Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
            codeGenerator.Visit(root);

            var code = codeGenerator.CreateCode();

            WriteBlocksToConsole(code);

            var exprGenKill = ReachableExprsGenerator.BuildReachableExpressionsGenKill(code);
            foreach (Block block in code.blocks)
            {
                Console.WriteLine();
                Console.WriteLine("block: {0}", code.blocks.IndexOf(block) + 1);

                Console.WriteLine("gen:");
                foreach (Expr expr in exprGenKill.In[block])
                {
                    Console.WriteLine("\t" + expr.ToString());
                }

                Console.WriteLine("kill:");
                foreach (Expr expr in exprGenKill.Out[block])
                                {
                    Console.WriteLine("\t" + expr.ToString());
                }
            }
        }

        public static void TestReachableExpressions(ProgramTree.BlockNode root)
        {
            Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
            codeGenerator.Visit(root);

            var code = codeGenerator.CreateCode();

            WriteBlocksToConsole(code);

            InOutData<Expr> reachableExpressions = DataFlowAnalysis.BuildReachableExpressions(code);

            foreach (Block block in code.blocks)
            {
                Console.WriteLine();
                Console.WriteLine("block: {0}", code.blocks.IndexOf(block) + 1);

                Console.WriteLine("in:");
                foreach (Expr expr in reachableExpressions.In[block])
                {
                    Console.WriteLine("\t" + expr.ToString());
                }

                Console.WriteLine("out:");
                foreach (Expr expr in reachableExpressions.Out[block])
                {
                    Console.WriteLine("\t" + expr.ToString());
                }
            }
        }

        public static bool IsGraphGiven(ProgramTree.BlockNode root)
        {
            Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
            codeGenerator.Visit(root);

            var code = codeGenerator.CreateCode();

            WriteBlocksToConsole(code);

            Dictionary<Block, IEnumerable<Block>> blockDoms = DomGraph.GenerateDomOut(code);
            ControlFlowGraph CFG = new ControlFlowGraph(code.blocks);
            SpanningTreeWithoutRecursive<Block> spanningTree = new SpanningTreeWithoutRecursive<Block>(code.blocks, CFG);
            GraphEdges<Block> graphEdges = new GraphEdges<Block>(spanningTree, blockDoms);
            return graphEdges.IsGraphGiven();

        }

        public static void GraphIntTesting1()
        {
            Dictionary<int, List<int>> testDictionary = new Dictionary<int,List<int>>();
            for (int i = 1; i <= 3; ++i)
                testDictionary[i] = new List<int>();

            testDictionary[1].Add(2);
            testDictionary[3].AddRange(new int [] {1, 2});

            AllCyclesTesting.TestGraph testGraph = new AllCyclesTesting.TestGraph(testDictionary);
            AllCyclesTesting.TestDominatorTree testDom = new AllCyclesTesting.TestDominatorTree(testDictionary);
            Dictionary<int, IEnumerable<int>> intDoms = new Dictionary<int, IEnumerable<int>>();

            for (int i = 1; i <= 3; ++i)
                intDoms[i] = testDom.UpperDominators(i) as List<int>;

            SpanningTreeWithoutRecursive<int> spanningTree = new SpanningTreeWithoutRecursive<int>(new int[] { 1, 2, 3 }, testGraph);
            GraphEdges<int> graphEdges = new GraphEdges<int>(spanningTree, intDoms);

            if (graphEdges.IsGraphGiven())
                Console.WriteLine("Test GraphInt 1 passed");
            else
                Console.WriteLine("Test GraphInt 1 did not passed");
        }

        public static void GraphIntTesting2()
        {
            Dictionary<int, List<int>> testDictionary = new Dictionary<int, List<int>>();
            for (int i = 1; i <= 3; ++i)
                testDictionary[i] = new List<int>();

            testDictionary[1].AddRange(new int[] { 2, 3 });

            AllCyclesTesting.TestGraph testGraph = new AllCyclesTesting.TestGraph(testDictionary);
            AllCyclesTesting.TestDominatorTree testDom = new AllCyclesTesting.TestDominatorTree(testDictionary);
            Dictionary<int, IEnumerable<int>> intDoms = new Dictionary<int, IEnumerable<int>>();

            for (int i = 1; i <= 3; ++i)
                intDoms[i] = testDom.UpperDominators(i) as List<int>;

            SpanningTreeWithoutRecursive<int> spanningTree = new SpanningTreeWithoutRecursive<int>(new int[] { 1, 2, 3 }, testGraph);
            GraphEdges<int> graphEdges = new GraphEdges<int>(spanningTree, intDoms);

            if (graphEdges.IsGraphGiven())
                Console.WriteLine("Test GraphInt 2 passed");
            else
                Console.WriteLine("Test GraphInt 2 did not passed");



        }

        //пример из лекции неприводимого графа
        public static void GraphIntTesting3()
        {
            Dictionary<int, List<int>> testDictionary = new Dictionary<int, List<int>>();
            for (int i = 1; i <= 3; ++i)
                testDictionary[i] = new List<int>();

            testDictionary[1].AddRange(new int[] { 2, 3 });
            testDictionary[2].Add(3);
            testDictionary[3].Add(2);

            AllCyclesTesting.TestGraph testGraph = new AllCyclesTesting.TestGraph(testDictionary);
            AllCyclesTesting.TestDominatorTree testDom = new AllCyclesTesting.TestDominatorTree(testDictionary);
            Dictionary<int, IEnumerable<int>> intDoms = new Dictionary<int, IEnumerable<int>>();

            for (int i = 1; i <= 3; ++i)
                intDoms[i] = testDom.UpperDominators(i) as List<int>;

            SpanningTreeWithoutRecursive<int> spanningTree = new SpanningTreeWithoutRecursive<int>(new int[] { 1, 2, 3 }, testGraph);
            GraphEdges<int> graphEdges = new GraphEdges<int>(spanningTree, intDoms);

            if (graphEdges.IsGraphGiven())
                Console.WriteLine("Test GraphInt 3 passed");
            else
                Console.WriteLine("Test GraphInt 3 did not passed");



        }

        private static void WriteBlocksToConsole(ThreeAddrCode code)
        {
            int blockIndex = 0;
            foreach (Block block in code.blocks)
            {
                Console.WriteLine();
                Console.WriteLine("Block {0} :", ++blockIndex);
                Console.WriteLine(block);
            }
        }

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
                RunAllTests();
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
                files.Add(@"..\..\tests\test-graph1.pasn");
                //files.Add(@"..\..\tests\test-exprgenkill-3.cn");
                //files.Add(@"..\..\tests\test-exprgenkill-5.cn");

                foreach (var file in files)
                {
                    try
                    {
                        var root = FileLoader.LoadFile(file, System.Text.Encoding.UTF8);

                        Console.WriteLine("Syntax tree ready");

                        //Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
                        //codeGenerator.Visit(root);

                        //var code = codeGenerator.CreateCode();

                        //ActiveVarsOptimization AVO = new ActiveVarsOptimization(code);
                        //AVO.Optimize();

                        //DeadCodeElimination deadCodeElimination = new DeadCodeElimination(code/*, 1*/);
                        //deadCodeElimination.Optimize();

                        //Console.WriteLine(code);

                        if (IsGraphGiven(root))
                            Console.WriteLine("Это приводимый граф");
                        else
                            Console.WriteLine("Это неприводимый граф");

                        GraphIntTesting1();
                        GraphIntTesting2();
                        GraphIntTesting3();

                        //TestDomIterativeAlogrithm(root);
                        //TestReachableExpressions(root);
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
                    catch (SemanticException e)
                    {
                        Console.WriteLine("Semantic error: " + e.Message);
                    }
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine("Unexpected error: " + e.Message);
                    //    Console.WriteLine("Call stack:");
                    //    var stackObject = new System.Diagnostics.StackTrace(e);
                    //    string stackTrace = stackObject.ToString();
                    //    Console.WriteLine(stackTrace);
                    //}
                }
            }

            Console.ReadLine();
        }

    }
}
