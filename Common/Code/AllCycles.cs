using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    /// <summary>
    /// Интерфейс для дерева доминатора
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    public interface IDominatorRelation<T>
    {
        /// <summary>
        /// Отношение доминирования первой вершины над второй
        /// </summary>
        /// <param name="a">Первая вершина</param>
        /// <param name="b">Вторая вершина</param>
        /// <returns></returns>
        bool FirstDomSeccond(T a, T b);
    }

    public abstract class AbstractTestDominatorTree: IDominatorRelation<int>
    {
        protected Dictionary<int, List<int>> data;
        public bool FirstDomSeccond(int a, int b)
        {
            return data[b].Any(x => x == a);
        }
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

    /// <summary>
    /// Пример2: граф доминатора из лекции
    /// </summary>
    public class TestDominatorTree1 : AbstractTestDominatorTree
    {
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

    /// <summary>
    /// Тест алгоритма определения всех естественных циклов на 2-х премерах из лекций
    /// </summary>
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
            int[] blocks1 = { 1, 2, 3, 4 };
            List<DomGraph.BlocksPair<int>> reverseEdges1 = new List<DomGraph.BlocksPair<int>>();
            reverseEdges1.Add(new DomGraph.BlocksPair<int>(3, 1));
            reverseEdges1.Add(new DomGraph.BlocksPair<int>(4, 1));
            CalcAndPrint(blocks1, graph1, reverseEdges1, domTree1);
        }

        private static void CalcAndPrint(IEnumerable<int> blocks, IGraph<int> graph, List<DomGraph.BlocksPair<int>> reverseEdges, IDominatorRelation<int> domTree)
        {
            Console.WriteLine("New example");
            AllCycles<int> allCycles = new AllCycles<int>(blocks, graph, reverseEdges, domTree);
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

    /// <summary>
    /// Класс цикла, содержащий вход в цикл (n) 
    /// и список всех вершин, пренадлежащих циклу
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class Cycle<T>
    {
        public Cycle(T n, List<T> data)
        {
            this.n = n;
            this.data = data;
        }

        public T n { get; set; }

        public List<T> data { get; set; }
    }

    /// <summary>
    /// Обределяет и хранит все циклы CFG, в том числе вложенные
    /// Использование
    ///     Инициализация
    ///     AllCycles<Block> allCycles = new AllCycles<Block>(code.blocks, code.graph, reverseEdges, domTree);
    ///     
    ///     Получить все циклы
    ///     List<Cycle<Block>> cycles = allCycles.cycles;
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class AllCycles<T> where T : IComparable<T>
    {
        /// <summary>
        /// Все циклы
        /// </summary>
        public List<Cycle<T>> cycles { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocks">Список блоков</param>
        /// <param name="graph">CFG</param>
        /// <param name="reverseEdges">Список обратных дуг</param>
        /// <param name="domTree">Дерево доминирования</param>
        public AllCycles(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.BlocksPair<T>> reverseEdges, IDominatorRelation<T> domTree)
        {
            cycles = new List<Cycle<T>>();
            foreach (T n in blocks)
            {
                foreach (T d in reverseEdges.FindAll(pair => pair.blockEnd.CompareTo(n) == 0).Select(pair => pair.blockBegin))
                {
                    Dictionary<T, bool> mark = new Dictionary<T, bool>();
                    foreach (T bl in blocks)
                        mark[bl] = false;
                    cycles.Add(new Cycle<T>(n, Find(n, d, d, mark, graph, domTree)));
                }
            }
        }

        private static List<T> Find(T n, T d, T x, Dictionary<T, bool> mark, IGraph<T> graph, IDominatorRelation<T> domTree)
        {
            List<T> verts = new List<T>();
            if (mark[x])
            {
                return verts;
            }
            else if (n.CompareTo(x) == 0)
            {
                mark[n] = true;
                verts.Add(n);
                return verts;
            }
            mark[x] = true;
            verts.Add(x);
            foreach (T prev in graph.InEdges(x).Where(e => !domTree.FirstDomSeccond(d, e)))
            {
                verts.AddRange(Find(n, d, prev, mark, graph, domTree));
            }
            return verts;
        }
    }
}