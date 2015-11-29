using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public interface IDominatorRelation<T>
    {
        bool FirstDomSeccond(T a, T b);
    }

    public class TestDominatorTree: IDominatorRelation<int>
    {
        private Dictionary<int, List<int>> data;
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

        public bool FirstDomSeccond(int a, int b)
        {
            return data[b].Any(x => x == a);
        }
    }

    public class TestDominatorTree1 : IDominatorRelation<int>
    {
        private Dictionary<int, List<int>> data;
        public TestDominatorTree1()
        {
            data = new Dictionary<int, List<int>>();
            for (int i = 1; i <= 10; i++)
                data[i] = new List<int>();
            data[1].AddRange(new int[] { 1 });
            data[2].AddRange(new int[] { 1, 2 });
            data[3].AddRange(new int[] { 1, 2, 3 });
            data[4].AddRange(new int[] { 1, 3, 4 });
        }

        public bool FirstDomSeccond(int a, int b)
        {
            return data[b].Any(x => x == a);
        }
    }

    public class TestGraph1 : IGraph<int>
    {
        private Dictionary<int, IList<int>> Data;
        private Dictionary<int, IList<int>> ReversedData;
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

        public IEnumerable<int> OutEdges(int block)
        {
            return Data[block];
        }

        public IEnumerable<int> InEdges(int block)
        {
            return ReversedData[block];
        }

        private static Dictionary<int, IList<int>> getReversedIndexedGraph(Dictionary<int, IList<int>> graph)
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

    public static class TestAllCycles
    {
        public static void Test()
        {
            TestGraph graph = new TestGraph();
            TestDominatorTree domTree = new TestDominatorTree();
            int[] blocks = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<DomGraph.BlocksPair<int>> reverseEdges = new List<DomGraph.BlocksPair<int>>();
            reverseEdges.Add(new DomGraph.BlocksPair<int>(9, 1));
            reverseEdges.Add(new DomGraph.BlocksPair<int>(4, 3));
            reverseEdges.Add(new DomGraph.BlocksPair<int>(8, 3));
            reverseEdges.Add(new DomGraph.BlocksPair<int>(7, 4));
            reverseEdges.Add(new DomGraph.BlocksPair<int>(10, 7));
            CalcAndPrint(blocks, graph, reverseEdges, domTree);

            TestGraph1 graph1 = new TestGraph1();
            TestDominatorTree1 domTree1 = new TestDominatorTree1();
            int[] blocks1 = { 1, 2, 3, 4};
            List<DomGraph.BlocksPair<int>> reverseEdges1 = new List<DomGraph.BlocksPair<int>>();
            reverseEdges1.Add(new DomGraph.BlocksPair<int>(3, 1));
            reverseEdges1.Add(new DomGraph.BlocksPair<int>(4, 1));
            CalcAndPrint(blocks1, graph1, reverseEdges1, domTree1);
        }

        private static void CalcAndPrint(IEnumerable<int> blocks, IGraph<int> graph, List<DomGraph.BlocksPair<int>> reverseEdges, IDominatorRelation<int> domTree)
        {
            Console.WriteLine("New example");
            AllCycles allCycles = new AllCycles(blocks, graph, reverseEdges, domTree);
            foreach (var cycle in allCycles.cycles)
            {
                Console.Write("n:" + cycle.n + "  ");
                foreach (var l in cycle.data)
                    Console.Write(l + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    public class Cycle
    {
        public Cycle(int n, List<int> data)
        {
            this.n = n;
            this.data = data;
        }

        public int n { get; set; }

        public List<int> data { get; set; }
    }

    public class AllCycles
    {
        public List<Cycle> cycles { get; private set; }

        public AllCycles(IEnumerable<int> blocks, IGraph<int> graph, List<DomGraph.BlocksPair<int>> reverseEdges, IDominatorRelation<int> domTree)
        {
            cycles = new List<Cycle>();
            foreach(int n in blocks)
            {
                foreach(int d in reverseEdges.FindAll(pair => pair.blockEnd == n).Select(pair => pair.blockBegin))
                {
                    Dictionary<int, bool> mark = new Dictionary<int, bool>();
                    foreach (int bl in blocks)
                        mark[bl] = false;
                    cycles.Add(new Cycle(n, Find(n, d, d, mark, graph, domTree)));
                }
            }
        }

        private static List<int> Find(int n, int d, int x, Dictionary<int, bool> mark, IGraph<int> graph, IDominatorRelation<int> domTree)
        {
            List<int> verts = new List<int>();
            if (mark[x])
            {
                return verts;
            }
            else if (n == x)
            {
                mark[n] = true;
                verts.Add(n);
                return verts;
            }
            mark[x] = true;
            verts.Add(x);
            foreach(int prev in graph.InEdges(x).Where(e => !domTree.FirstDomSeccond(d, e)))
            {
                verts.AddRange(Find(n, d, prev, mark, graph, domTree));
            }
            return verts;
        }
    }
}
