using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    /// <summary>
    /// Набор тестов для класса AllCycles
    /// </summary>
    public static class AllCyclesTesting
    {
        /// <summary>
        /// Тестовый граф
        /// </summary>
        public class TestGraph : IGraph<int>
        {
            public TestGraph(Dictionary<int, List<int>> data)
            {
                this.Data = data;
                ReversedData = TestGraph.getReversedIndexedGraph(Data);
            }

            private Dictionary<int, List<int>> Data;
            private Dictionary<int, List<int>> ReversedData;
            public IEnumerable<int> OutEdges(int block)
            {
                return Data[block];
            }

            public IEnumerable<int> InEdges(int block)
            {
                return ReversedData[block];
            }

            private static Dictionary<int, List<int>> getReversedIndexedGraph(Dictionary<int, List<int>> graph)
            {
                bool[,] graphTable = new bool[graph.Count() + 1, graph.Count() + 1];
                graphTable.Initialize();
                for (int i = 1; i <= graph.Count(); i++)
                {
                    foreach (int j in graph[i])
                    {
                        graphTable[i, j] = true;
                    }
                }

                Dictionary<int, List<int>> reversedGraph = new Dictionary<int, List<int>>();
                for (int i = 1; i <= graph.Count(); i++) reversedGraph[i] = new List<int>(2);
                for (int i = 1; i <= graph.Count(); i++)
                {
                    for (int j = 1; j <= graph.Count(); j++)
                    {
                        if (graphTable[i, j]) reversedGraph[j].Add(i);
                    }
                }

                return reversedGraph;
            }
        }
        /// <summary>
        /// Тестовое дерево доминаторов
        /// </summary>
        public class TestDominatorTree : IDominatorRelation<int>
        {
            public TestDominatorTree(Dictionary<int, List<int>> data)
            {
                this.data = data;
            }
            private Dictionary<int, List<int>> data;
            public bool FirstDomSeccond(int a, int b)
            {
                return data[b].Any(x => x == a);
            }
            public IEnumerable<int> UpperDominators(int a)
            {
                return data[a];
            }
        }
        /// <summary>
        /// Тестовое остовное дерево
        /// </summary>
        public class TestSpanningTree: SpanningTree<int>
        {
            public TestSpanningTree(Dictionary<int, List<int>> Data, Dictionary<int, int> Numbers)
                :base()
            {
                this.Data = Data;
                this.Numbers = Numbers;
            }
        }
        /// <summary>
        /// Фабрика для примеров
        /// </summary>
        public abstract class AllCyclesTestExampleAbstract
        {
            protected string exampleTitle;
            protected int[] blocks;
            protected TestGraph graph;
            protected List<DomGraph.ValPair<int>> reverseEdges;
            protected TestDominatorTree domTree;
            public void TestAllCycles()
            {
                AllCycles<int> allCycles = new AllCycles<int>(blocks, graph, reverseEdges, domTree);
                Console.WriteLine("AllCycles");
                Console.WriteLine(exampleTitle);
                foreach (Cycle<int> c in allCycles.cycles)
                    _print_cycle(c);
                Console.WriteLine();
            }
            public void TestAllCyclesWithSpecialCase()
            {
                AllCycles<int> allCycles = new AllCyclesWithSpecialCase<int>(blocks, graph, reverseEdges, domTree);
                Console.WriteLine("AllCyclesWithSpecialCase");
                Console.WriteLine(exampleTitle);
                foreach(Cycle<int> c in allCycles.cycles)
                    _print_cycle(c);
                Console.WriteLine();
            }
            public void TestCyclesHierarchy()
            {
                //находим все циклы
                AllCycles<int> allCycles = new AllCyclesWithSpecialCase<int>(blocks, graph, reverseEdges, domTree);
                Console.WriteLine("CyclesHierarchy");
                Console.WriteLine(exampleTitle);
                //здесь определяем вложенность циклов (например)
                //...
                Dictionary<Cycle<int>, List<Cycle<int>>> hierarchy = null;
                //...
                //и выводим
                foreach(Cycle<int> c in allCycles.cycles)
                {
                    //...
                    _print_cycle(c);
                    //...
                    foreach(Cycle<int> c1 in hierarchy[c])
                    {
                        //...
                        _print_cycle(c1);
                        //...
                    }
                    //...
                }
                Console.WriteLine();
            }
            private void _print_cycle(Cycle<int> cycle)
            {
                Console.Write("IN:" + cycle.N + " VERTS:");
                foreach (var i in cycle.DATA)
                    Console.Write(i + " ");
                Console.Write("OUTS:");
                foreach (DomGraph.ValPair<int> p in cycle.OUTS)
                    Console.Write("(" + p.valBegin + ">" + p.valEnd + ") ");
                if (cycle is CycleUsual<int>)
                {
                    CycleUsual<int> c = cycle as CycleUsual<int>;
                    Console.Write("D:" + c.D);
                }
                else if (cycle is CycleSpecialCase<int>)
                {
                    CycleSpecialCase<int> c = cycle as CycleSpecialCase<int>;
                    Console.Write("D1:" + c.D1 + " D2:" + c.D2);
                }
                Console.WriteLine();
            }
            public void TestSpanningTree()
            {
                SpanningTree<int> t = new SpanningTreeWithoutRecursive<int>(blocks, graph);
                Console.WriteLine("SpanningTree");
                Console.WriteLine(exampleTitle);
                for (int i = blocks.First(); i <= blocks.Last(); i++)
                {
                    Console.Write(i + "(" + t.Numbers[i] + "): ");
                    foreach (int v in t.Data[i])
                        Console.Write(v + " ");
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Пример 1 из лекции
        /// </summary>
        public class AllCyclesTestExample1 : AllCyclesTestExampleAbstract
        {
            public AllCyclesTestExample1() 
            { 
                exampleTitle = "Example1";
                blocks = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                Dictionary<int, List<int>>  Data = new Dictionary<int, List<int>>();
                for (int i = 1; i <= 10; i++)
                    Data[i] = new List<int>();
                Data[1].Add(2);
                Data[1].Add(3);
                Data[2].Add(3);
                Data[3].Add(4);
                Data[4].Add(5);
                Data[4].Add(6);
                Data[4].Add(3);
                Data[5].Add(7);
                Data[6].Add(7);
                Data[7].Add(8);
                Data[7].Add(4);
                Data[8].Add(9);
                Data[8].Add(10);
                Data[8].Add(3);
                Data[9].Add(1);
                Data[10].Add(7);
                graph = new TestGraph(Data);
                reverseEdges = new List<DomGraph.ValPair<int>>();
                reverseEdges.Add(new DomGraph.ValPair<int>(9, 1));
                reverseEdges.Add(new DomGraph.ValPair<int>(4, 3));
                reverseEdges.Add(new DomGraph.ValPair<int>(8, 3));
                reverseEdges.Add(new DomGraph.ValPair<int>(7, 4));
                reverseEdges.Add(new DomGraph.ValPair<int>(10, 7));
                Dictionary<int, List<int>> DataDom = new Dictionary<int, List<int>>();
                for (int i = 1; i <= 10; i++)
                    DataDom[i] = new List<int>();
                DataDom[1].AddRange(new int[] { 1 });
                DataDom[2].AddRange(new int[] { 1, 2 });
                DataDom[3].AddRange(new int[] { 1, 3 });
                DataDom[4].AddRange(new int[] { 1, 3, 4 });
                DataDom[5].AddRange(new int[] { 1, 3, 4, 5 });
                DataDom[6].AddRange(new int[] { 1, 3, 4, 6 });
                DataDom[7].AddRange(new int[] { 1, 3, 4, 7 });
                DataDom[8].AddRange(new int[] { 1, 3, 4, 7, 8 });
                DataDom[9].AddRange(new int[] { 1, 3, 4, 7, 8, 9 });
                DataDom[10].AddRange(new int[] { 1, 3, 4, 7, 8, 10 });
                domTree = new TestDominatorTree(DataDom);
            }
        }
        /// <summary>
        /// Пример 2 из лекции
        /// </summary>
        public class AllCyclesTestExample2 : AllCyclesTestExampleAbstract
        {
            public AllCyclesTestExample2() 
            { 
                exampleTitle = "Example2";
                blocks = new int[] { 1, 2, 3, 4 };
                Dictionary<int, List<int>> Data = new Dictionary<int, List<int>>();
                for (int i = 1; i <= 4; i++)
                    Data[i] = new List<int>();
                Data[1].Add(2);
                Data[2].Add(3);
                Data[2].Add(4);
                Data[3].Add(1);
                Data[4].Add(1);
                graph = new TestGraph(Data);
                reverseEdges = new List<DomGraph.ValPair<int>>();
                reverseEdges.Add(new DomGraph.ValPair<int>(3, 1));
                reverseEdges.Add(new DomGraph.ValPair<int>(4, 1));
                Dictionary<int, List<int>> DataDom = new Dictionary<int, List<int>>();
                for (int i = 1; i <= 4; i++)
                    DataDom[i] = new List<int>();
                DataDom[1].AddRange(new int[] { 1 });
                DataDom[2].AddRange(new int[] { 1, 2 });
                DataDom[3].AddRange(new int[] { 1, 2, 3 });
                DataDom[4].AddRange(new int[] { 1, 2, 4 });
                domTree = new TestDominatorTree(DataDom);
            }
        }
        /// <summary>
        /// Тесты для классов AllCycles
        /// </summary>
        public static void TestingAllCycles()
        {
            AllCyclesTestExample1 ex1 = new AllCyclesTestExample1();
            AllCyclesTestExample2 ex2 = new AllCyclesTestExample2();
            ex1.TestAllCycles();
            ex2.TestAllCycles();
            ex1.TestAllCyclesWithSpecialCase();
            ex2.TestAllCyclesWithSpecialCase();
        }
        /// <summary>
        /// Тесты для классов SpanningTree
        /// </summary>
        public static void TestingSpanningTree()
        {
            AllCyclesTestExample1 ex1 = new AllCyclesTestExample1();
            AllCyclesTestExample2 ex2 = new AllCyclesTestExample2();
            ex1.TestSpanningTree();
            ex2.TestSpanningTree();
        }
        /// <summary>
        /// Тесты для определения вложенности циклов
        /// </summary>
        public static void TestingCyclesHierarchy()
        {
            AllCyclesTestExample1 ex1 = new AllCyclesTestExample1();
            AllCyclesTestExample2 ex2 = new AllCyclesTestExample2();
            ex1.TestCyclesHierarchy();
            ex2.TestCyclesHierarchy();
        }
    }
}
