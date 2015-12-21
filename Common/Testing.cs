using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iCompiler;
using iCompiler.Line;
using CompilerExceptions;
using System.IO;

namespace SimpleCompiler
{
    public static class Testing
    {
        public delegate void CodeTest(ThreeAddrCode code);
        public delegate void RootTest(ProgramTree.BlockNode root);

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

                foreach (Block domBlock in blockDoms[block])
                {
                    Console.Write(" {0};", code.blocks.IndexOf(domBlock) + 1);
                }

                Console.WriteLine();
            }

            Console.WriteLine("\nDom Tree Algorithm");
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

            Console.WriteLine("\nDom Tree Class");
            Console.WriteLine("\nTesting FirstDomSecond function:");
            DomTree domTree = new DomTree(code);
            for (int i = 0; i < code.blocks.Count; ++i)
                for (int j = 0; j < code.blocks.Count; ++j )
                {
                    bool res = domTree.FirstDomSeccond(code.blocks[i], code.blocks[j]);
                    Console.WriteLine((i + 1).ToString() + " dom " + (j + 1).ToString() + " = " + res);
                }
            Console.WriteLine("\nTesting UpperDominators function:");
            for (int i = 0; i < code.blocks.Count; ++i)
            {
                IEnumerable<Block> list = domTree.UpperDominators(code.blocks[i]);
                Console.Write("UpperDominators({0}) = ( ", (i + 1));
                foreach (Block block in list)
                    Console.Write("{0} ", code.blocks.IndexOf(block) + 1);
                Console.WriteLine(")");
            }
            Console.WriteLine("\nTesting DownDominators function:");
            for (int i = 0; i < code.blocks.Count; ++i)
            {
                IEnumerable<Block> list = domTree.DownDominators(code.blocks[i]);
                Console.Write("DownDominators({0}) = ( ", (i + 1));
                foreach (Block block in list)
                    Console.Write("{0} ", code.blocks.IndexOf(block) + 1);
                Console.WriteLine(")");
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

        public static void TestGraphEdges(ProgramTree.BlockNode root)
        {
            Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
            codeGenerator.Visit(root);
            var code = codeGenerator.CreateCode();
            WriteBlocksToConsole(code);
            Dictionary<Block, IEnumerable<Block>> blockDoms = DomGraph.GenerateDomOut(code);
            ControlFlowGraph CFG = new ControlFlowGraph(code.blocks);
            SpanningTreeWithoutRecursive<Block> spanningTree = new SpanningTreeWithoutRecursive<Block>(code.blocks, CFG);
            GraphEdges<Block> graphEdges = new GraphEdges<Block>(spanningTree, blockDoms);

            Console.WriteLine("\nStraight Edges");
            List<DomGraph.ValPair<Block>> listEdges = graphEdges.StraightEdges(blockDoms) as List<DomGraph.ValPair<Block>>;
            if (listEdges.Count == 0)
                Console.WriteLine("There are no straight edges");
            else
                for (int i = 0; i < listEdges.Count; ++i)
                    Console.WriteLine("Straight edge [{0}] == ({1} , {2} );", i + 1, code.blocks.IndexOf(listEdges[i].valBegin) + 1,
                        code.blocks.IndexOf(listEdges[i].valEnd) + 1);

            Console.WriteLine("\nReversed Edges");
            listEdges = graphEdges.ReversedEdges(blockDoms) as List<DomGraph.ValPair<Block>>;
            if (listEdges.Count == 0)
                Console.WriteLine("There are no reversed edges");
            else
                for (int i = 0; i < listEdges.Count; ++i)
                    Console.WriteLine("Reversed edge [{0}] == ({1} , {2} );", i + 1, code.blocks.IndexOf(listEdges[i].valBegin) + 1,
                        code.blocks.IndexOf(listEdges[i].valEnd) + 1);

            Console.WriteLine("\nAdvancing Edges");
            listEdges = graphEdges.AdvancingEdges() as List<DomGraph.ValPair<Block>>;
            if (listEdges.Count == 0)
                Console.WriteLine("There are no advancing edges");
            else
                for (int i = 0; i < listEdges.Count; ++i)
                    Console.WriteLine("Advancing edge [{0}] == ({1} , {2} );", i + 1, code.blocks.IndexOf(listEdges[i].valBegin) + 1,
                        code.blocks.IndexOf(listEdges[i].valEnd) + 1);

            Console.WriteLine("\nRetreating Edges");
            listEdges = graphEdges.RetreatingEdges() as List<DomGraph.ValPair<Block>>;
            if (listEdges.Count == 0)
                Console.WriteLine("There are no retreating edges");
            else
                for (int i = 0; i < listEdges.Count; ++i)
                    Console.WriteLine("Retreating edge [{0}] == ({1} , {2} );", i + 1, code.blocks.IndexOf(listEdges[i].valBegin) + 1,
                        code.blocks.IndexOf(listEdges[i].valEnd) + 1);

            Console.WriteLine("\nCrossing Edges");
            listEdges = graphEdges.CrossingEdges() as List<DomGraph.ValPair<Block>>;
            if (listEdges.Count == 0)
                Console.WriteLine("There are no crossing edges");
            else
                for (int i = 0; i < listEdges.Count; ++i)
                    Console.WriteLine("Crossing edge [{0}] == ({1} , {2} );", i + 1, code.blocks.IndexOf(listEdges[i].valBegin) + 1,
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


        /// <summary>
        ///Выводит список ConstNACInfo для каждого блока. Каждую переменную в текущем блоке, помеченную как CONSTANT, 
        ///можно заменить (в НЁМ же) на ее значение
        /// </summary>
        public static void TestConstantsPropagation_sets(ProgramTree.BlockNode root)
        {
            Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
            codeGenerator.Visit(root);

            var code = codeGenerator.CreateCode();

            WriteBlocksToConsole(code);

            InOutData<ConstNACInfo> constsInfo = DataFlowAnalysis.buildConsts(code);

            foreach (Block block in code.blocks)
            {
                
                Console.WriteLine();
                Console.WriteLine("block: {0}", code.blocks.IndexOf(block) + 1);

                Console.WriteLine("in:");
                foreach (ConstNACInfo cInfo in constsInfo.In[block])
                {
                    Console.WriteLine(cInfo.ToString());
                    Console.WriteLine();
                }

                Console.WriteLine("\r\nout:");
                foreach (ConstNACInfo cInfo in constsInfo.Out[block])
                {
                    Console.WriteLine(cInfo.ToString());
                    Console.WriteLine();
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
            Dictionary<int, List<int>> testDictionary = new Dictionary<int, List<int>>();
            for (int i = 1; i <= 3; ++i)
                testDictionary[i] = new List<int>();

            testDictionary[1].Add(2);
            testDictionary[3].AddRange(new int[] { 1, 2 });

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
                Console.WriteLine("Test GraphInt 1 did not pass");
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
                Console.WriteLine("Test GraphInt 2 did not pass");
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
                Console.WriteLine("Test GraphInt 3 did not pass");
        }


        /// <summary>
        ///Выводит список ConstNACInfo для каждого блока. Каждую переменную в текущем блоке, помеченную как CONSTANT, 
        ///можно заменить (в НЁМ же) на ее значение
        /// </summary>
        public static void TestConstantsPropagation(ProgramTree.BlockNode root)
        {
            Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
            codeGenerator.Visit(root);

            var code = codeGenerator.CreateCode();

            Console.WriteLine("code: \r\n");
            Console.WriteLine(code.ToString());

            ConstantsPropagationOptimization opt = new ConstantsPropagationOptimization(code);
            opt.Optimize();
            Console.WriteLine("transformed code: \r\n");
            Console.WriteLine(code.ToString());
        }


        /// <summary>
        /// Протяжка констант + сворачивание констант
        /// </summary>
        public static void TestDraggingConstants(ProgramTree.BlockNode root)
        {
            Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
            codeGenerator.Visit(root);

            var code = codeGenerator.CreateCode();

            WriteBlocksToConsole(code);
            (new DraggingConstantsOptimization(code)).Optimize();
            (new ConstantFolding(code)).Optimize();

            WriteBlocksToConsole(code);
        }


        /// <summary>
        /// Провести тесты
        /// </summary>
        /// <param name="files">Список файлов</param>
        /// <param name="root_tests">Список тестов для корня синтаксического дерева</param>
        /// <param name="code_tests">Список тестов для трехадресного кода</param>
        public static void PerformTests(List<string> files, List<RootTest> root_tests = null, List<CodeTest> code_tests = null)
        {
            foreach (var file in files)
            {
                try
                {
                    var root = FileLoader.LoadFile(file, System.Text.Encoding.UTF8);
                    Console.WriteLine("Check " + file + "...\n---");
                    Console.WriteLine("Syntax tree is ready");

                    if (root_tests != null && root_tests.Count != 0)
                    {
                        Console.WriteLine("Performing Root tests");
                        for (int i = 0; i < root_tests.Count; ++i)
                        {
                            Console.WriteLine("Root Test " + (i + 1).ToString());
                            root_tests[i](root);
                        }
                    }

                    Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
                    codeGenerator.Visit(root);
                    var code = codeGenerator.CreateCode();
                    Console.WriteLine("ThreeAddrCode is ready");

                    if (code_tests != null && code_tests.Count != 0)
                    {
                        Console.WriteLine("Performing Code tests");

                        for (int i = 0; i < code_tests.Count; ++i)
                        {
                            Console.WriteLine("Code Test " + (i + 1).ToString());
                            code_tests[i](code);
                        }
                    }

                    Console.WriteLine();
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
            }
        }

        /// <summary>
        /// Провести все тесты
        /// </summary>
        /// <param name="root_tests">Список тестов для корня синтаксического дерева</param>
        /// <param name="code_tests">Список тестов для трехадресного кода</param>
        public static void PerformAllTests(List<RootTest> root_tests = null, List<CodeTest> code_tests = null)
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

            PerformTests(files, root_tests, code_tests);
        }
    }
}
