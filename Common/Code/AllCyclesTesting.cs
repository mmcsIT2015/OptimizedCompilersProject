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
        /// Абстрактый тестовый граф
        /// </summary>
        public abstract class AbstarctTestGraph : IGraph<int>
        {
            protected Dictionary<int, IList<int>> Data;
            protected Dictionary<int, IList<int>> ReversedData;
            public IEnumerable<int> OutEdges(int block)
            {
                return Data[block];
            }

            public IEnumerable<int> InEdges(int block)
            {
                return ReversedData[block];
            }

            protected static Dictionary<int, IList<int>> getReversedIndexedGraph(Dictionary<int, IList<int>> graph)
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

                Dictionary<int, IList<int>> reversedGraph = new Dictionary<int, IList<int>>();
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
        /// Абстрактное тестовое дерево доминаторов
        /// </summary>
        public abstract class AbstractTestDominatorTree : IDominatorRelation<int>
        {
            protected Dictionary<int, List<int>> data;
            public bool FirstDomSeccond(int a, int b)
            {
                return data[b].Any(x => x == a);
            }

            public IEnumerable<int> UpperDominators(int a)
            {
                return data[a];
            }

            public IEnumerable<int> DownDominators(int a)
            {
                return data.Where(e => e.Value.Any(x => x == a)).Select(e => e.Key).Aggregate(new List<int>(), (l, e) => { l.Add(e); return l; });
            }
        }
        /// <summary>
        /// Фабрика для примеров
        /// </summary>
        public abstract class AllCyclesTestExampleAbstractFactory
        {
            public abstract int[] GetBlocks();
            public abstract AbstarctTestGraph GetGraph();
            public abstract List<DomGraph.BlocksPair<int>> GetReverseArcs();
            public abstract AbstractTestDominatorTree GetDomTree();
            public void TestAllCycles()
            {
                AllCycles<int> allCycles = new AllCycles<int>(GetBlocks(), GetGraph(), GetReverseArcs(), GetDomTree());
                _cycles(allCycles);
            }
            public void TestAllCyclesWithSpecialCase()
            {
                AllCycles<int> allCycles = new AllCyclesWithSpecialCase<int>(GetBlocks(), GetGraph(), GetReverseArcs(), GetDomTree());
                _cycles(allCycles);
            }
            private void _cycles(AllCycles<int> allCycles)
            {
                foreach (var cycle in allCycles.cycles)
                {
                    Console.Write("N:" + cycle.N + " ");
                    if (cycle is CycleUsual<int>)
                    {
                        CycleUsual<int> c = cycle as CycleUsual<int>;
                        Console.Write("D:" + c.D);
                    }
                    else if (cycle is CycleSpecialCase<int>)
                    {
                        CycleSpecialCase<int> c = cycle as CycleSpecialCase<int>;
                        Console.Write("D1:" + c.D1 + " D2:" + c.D2 + " DOM:" + c.DOM);
                    }
                    Console.Write(" DATA:");
                    foreach (var i in cycle.DATA)
                        Console.Write(i + " ");
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            private void _spanningTree(SpanningTree<int> t)
            {
                for (int i = GetBlocks().First(); i <= GetBlocks().Last(); i++)
                {
                    Console.Write(i + "(" + t.Numbers[i] + "): ");
                    foreach (int v in t.Data[i])
                        Console.Write(v + " ");
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            public void TestSpanningTree()
            {
                SpanningTree<int> t = new SpanningTree<int>(GetBlocks(), GetGraph());
                _spanningTree(t);
            }
            public void TestSpanningTreeWithoutRecursive()
            {
                SpanningTree<int> t = new SpanningTreeWithoutRecursive<int>(GetBlocks(), GetGraph());
                _spanningTree(t);
            }
        }
        /// <summary>
        /// Пример 1 из лекции
        /// </summary>
        public class AllCyclesTestExample : AllCyclesTestExampleAbstractFactory
        {
            /// <summary>
            /// Управляющий граф из лекции
            /// </summary>
            public class TestGraph : AbstarctTestGraph
            {
                public TestGraph()
                {
                    Data = new Dictionary<int, IList<int>>();
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
                    ReversedData = getReversedIndexedGraph(Data);
                }
            }

            public override int[] GetBlocks()
            {
                return new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            }

            public override AbstarctTestGraph GetGraph()
            {
                return new TestGraph();
            }

            public override List<DomGraph.BlocksPair<int>> GetReverseArcs()
            {
                List<DomGraph.BlocksPair<int>> reverseEdges = new List<DomGraph.BlocksPair<int>>();
                reverseEdges.Add(new DomGraph.BlocksPair<int>(9, 1));
                reverseEdges.Add(new DomGraph.BlocksPair<int>(4, 3));
                reverseEdges.Add(new DomGraph.BlocksPair<int>(8, 3));
                reverseEdges.Add(new DomGraph.BlocksPair<int>(7, 4));
                reverseEdges.Add(new DomGraph.BlocksPair<int>(10, 7));
                return reverseEdges;
            }

            public override AbstractTestDominatorTree GetDomTree()
            {
                return new TestDominatorTree();
            }

            /// <summary>
            /// Граф доминатора из лекции
            /// </summary>
            public class TestDominatorTree : AbstractTestDominatorTree
            {
                public TestDominatorTree()
                {
                    data = new Dictionary<int, List<int>>();
                    for (int i = 1; i <= 10; i++)
                        data[i] = new List<int>();
                    data[1].AddRange(new int[] { 1 });
                    data[2].AddRange(new int[] { 1, 2 });
                    data[3].AddRange(new int[] { 1, 3 });
                    data[4].AddRange(new int[] { 1, 3, 4 });
                    data[5].AddRange(new int[] { 1, 3, 4, 5 });
                    data[6].AddRange(new int[] { 1, 3, 4, 6 });
                    data[7].AddRange(new int[] { 1, 3, 4, 7 });
                    data[8].AddRange(new int[] { 1, 3, 4, 7, 8 });
                    data[9].AddRange(new int[] { 1, 3, 4, 7, 8, 9 });
                    data[10].AddRange(new int[] { 1, 3, 4, 7, 8, 10 });
                }
            }

            public new void TestAllCycles()
            {
                Console.WriteLine("Example1");
                base.TestAllCycles();
            }
            public new void TestAllCyclesWithSpecialCase()
            {
                Console.WriteLine("Example1");
                base.TestAllCyclesWithSpecialCase();
            }
            public new void TestSpanningTree()
            {
                Console.WriteLine("Example1");
                base.TestSpanningTree();
            }
            public new void TestSpanningTreeWithoutRecursive()
            {
                Console.WriteLine("Example1");
                base.TestSpanningTreeWithoutRecursive();
            }
        }
        /// <summary>
        /// Пример 2 из лекции
        /// </summary>
        public class AllCyclesTestExample1 : AllCyclesTestExampleAbstractFactory
        {
            /// <summary>
            /// Пример2: граф доминатора из лекции
            /// </summary>
            public class TestDominatorTree1 : AbstractTestDominatorTree
            {
                public TestDominatorTree1()
                {
                    data = new Dictionary<int, List<int>>();
                    for (int i = 1; i <= 4; i++)
                        data[i] = new List<int>();
                    data[1].AddRange(new int[] { 1 });
                    data[2].AddRange(new int[] { 1, 2 });
                    data[3].AddRange(new int[] { 1, 2, 3 });
                    data[4].AddRange(new int[] { 1, 2, 4 });
                }
            }

            /// <summary>
            /// Пример2: CFG граф из лекции
            /// </summary>
            public class TestGraph1 : AbstarctTestGraph
            {
                public TestGraph1()
                {
                    Data = new Dictionary<int, IList<int>>();
                    for (int i = 1; i <= 4; i++)
                        Data[i] = new List<int>();
                    Data[1].Add(2);
                    Data[2].Add(3);
                    Data[2].Add(4);
                    Data[3].Add(1);
                    Data[4].Add(1);
                    ReversedData = getReversedIndexedGraph(Data);
                }
            }

            public override int[] GetBlocks()
            {
                return new int[] { 1, 2, 3, 4 };
            }

            public override AbstarctTestGraph GetGraph()
            {
                return new TestGraph1();
            }

            public override List<DomGraph.BlocksPair<int>> GetReverseArcs()
            {
                List<DomGraph.BlocksPair<int>> reverseEdges1 = new List<DomGraph.BlocksPair<int>>();
                reverseEdges1.Add(new DomGraph.BlocksPair<int>(3, 1));
                reverseEdges1.Add(new DomGraph.BlocksPair<int>(4, 1));
                return reverseEdges1;
            }

            public override AbstractTestDominatorTree GetDomTree()
            {
                return new TestDominatorTree1();
            }
            public new void TestAllCycles()
            {
                Console.WriteLine("Example2");
                base.TestAllCycles();
            }
            public new void TestAllCyclesWithSpecialCase()
            {
                Console.WriteLine("Example2");
                base.TestAllCyclesWithSpecialCase();
            }
            public new void TestSpanningTree()
            {
                Console.WriteLine("Example2");
                base.TestSpanningTree();
            }
            public new void TestSpanningTreeWithoutRecursive()
            {
                Console.WriteLine("Example2");
                base.TestSpanningTreeWithoutRecursive();
            }
        }
        /// <summary>
        /// Тесты для классов AllCycles
        /// </summary>
        public static void TestAllCycles()
        {
            AllCyclesTestExample ex = new AllCyclesTestExample();
            AllCyclesTestExample1 ex1 = new AllCyclesTestExample1();
            ex.TestAllCycles();
            ex1.TestAllCycles();
            ex.TestAllCyclesWithSpecialCase();
            ex1.TestAllCyclesWithSpecialCase();
        }
        /// <summary>
        /// Тесты для классов SpanningTree
        /// </summary>
        public static void TestSpanningTree()
        {
            AllCyclesTestExample ex = new AllCyclesTestExample();
            ex.TestSpanningTree();
            ex.TestSpanningTreeWithoutRecursive();
        }
    }
}
