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

        /// <summary>
        /// Все вершины, которые доминируют над текущей
        /// </summary>
        /// <param name="a">Вершина</param>
        /// <returns></returns>
        IEnumerable<T> UpperDominators(T a);

        /// <summary>
        /// Все вершины, над которыми доминирует текущая
        /// </summary>
        /// <param name="a">Вершина</param>
        /// <returns></returns>
        IEnumerable<T> DownDominators(T a);
    }

    public abstract class AbstractTestDominatorTree: IDominatorRelation<int>
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
            
            TestGraph1 graph1 = new TestGraph1();
            TestDominatorTree1 domTree1 = new TestDominatorTree1();
            int[] blocks1 = { 1, 2, 3, 4 };
            List<DomGraph.BlocksPair<int>> reverseEdges1 = new List<DomGraph.BlocksPair<int>>();
            reverseEdges1.Add(new DomGraph.BlocksPair<int>(3, 1));
            reverseEdges1.Add(new DomGraph.BlocksPair<int>(4, 1));

            Console.WriteLine("Without special case");
            AllCycles<int> allCycles = new AllCyclesWithSpecialCase<int>(blocks, graph, reverseEdges, domTree);
            Print(allCycles);
            AllCycles<int> allCycles1 = new AllCycles<int>(blocks1, graph1, reverseEdges1, domTree1);
            Print(allCycles1);

            Console.WriteLine("With special case");
            AllCycles<int> allCycles3 = new AllCyclesWithSpecialCase<int>(blocks, graph, reverseEdges, domTree);
            Print(allCycles3);
            AllCycles<int> allCycles4 = new AllCyclesWithSpecialCase<int>(blocks1, graph1, reverseEdges1, domTree1);
            Print(allCycles4);
        }

        private static void Print(AllCycles<int> allCycles)
        {
            Console.WriteLine("Example");
            foreach (var cycle in allCycles.cycles)
            {
                Console.Write("n:" + cycle.n + " d:");
                foreach (var d in cycle.ds)
                    Console.Write(d + " ");
                Console.Write("verts:");
                foreach (var l in cycle.data)
                    Console.Write(l + " ");
                if (cycle.ds.Count == 2)
                    Console.Write(" leastDomine: " + cycle.leastOuterDominator);
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Класс цикла в CFG
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class Cycle<T> where T: new()
    {
        public Cycle(T n, List<T> ds, List<T> data)
        {
            this.n = n;
            this.data = data;
            this.ds = ds;
            this.leastOuterDominator = new T();
        }

        public Cycle(T n, List<T> ds, List<T> data, T leastOuterDominator)
        {
            this.n = n;
            this.data = data;
            this.ds = ds;
            this.leastOuterDominator = leastOuterDominator;
        }

        /// <summary>
        /// Вход в цикл
        /// </summary>
        public T n { get; set; }

        /// <summary>
        /// Все вершины, принадлежащие цику
        /// </summary>
        public List<T> data { get; set; }

        /// <summary>
        /// Все выходы из цикла
        /// </summary>
        public List<T> ds { get; set; }

        /// <summary>
        /// Ближайший общий доминатор для 2-х выходов из цикла (опционально)
        /// </summary>
        public T leastOuterDominator { get; set; }
    }

    /// <summary>
    /// Обределяет и хранит все циклы CFG, в том числе вложенные.
    /// Считает граф, подаваемый на вход, приводимым!
    /// Использование
    ///     Инициализация
    ///     AllCycles < Block > allCycles = new AllCyclesWithSpecialCase<Block>(code.blocks, code.graph, reverseEdges, domTree);
    ///     
    ///     Получить все циклы
    ///     List < Cycle < Block > > cycles = allCycles.cycles;
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class AllCycles<T> where T : IComparable<T>, new()
    {
        /// <summary>
        /// Все циклы
        /// </summary>
        public List<Cycle<T>> cycles { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocks">Список блоков</param>
        /// <param name="graph">CFG</param>
        /// <param name="reverseEdges">Список обратных дуг</param>
        /// <param name="domTree">Дерево доминирования</param>
        public AllCycles(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.BlocksPair<T>> reverseEdges, IDominatorRelation<T> domTree)
        {
            InitCycles(blocks, graph, reverseEdges, domTree);
        }

        protected virtual void InitCycles(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.BlocksPair<T>> reverseEdges, IDominatorRelation<T> domTree)
        {
            cycles = new List<Cycle<T>>();
            foreach (T n in blocks)
            {
                foreach (T d in reverseEdges.FindAll(pair => pair.blockEnd.CompareTo(n) == 0).Select(pair => pair.blockBegin))
                {
                    Dictionary<T, bool> mark = new Dictionary<T, bool>();
                    foreach (T bl in blocks)
                        mark[bl] = false;
                    List<T> ds = new List<T>();
                    ds.Add(d);
                    cycles.Add(new Cycle<T>(n, ds, Find(n, d, d, mark, graph, domTree)));
                }
            }
        }

        protected static List<T> Find(T n, T d, T x, Dictionary<T, bool> mark, IGraph<T> graph, IDominatorRelation<T> domTree)
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

    /// <summary>
    /// !ОТЛИЧИЕ:
    ///     Обрабатывает "особый случай" с двумя обратными дугами (см. лекцию) 
    /// Обределяет и хранит все циклы CFG, в том числе вложенные.
    /// Считает граф, подаваемый на вход, приводимым!
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class AllCyclesWithSpecialCase<T> : AllCycles<T> where T: IComparable<T>, new()
    {
        public AllCyclesWithSpecialCase(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.BlocksPair<T>> reverseEdges, IDominatorRelation<T> domTree)
            : base(blocks, graph, reverseEdges, domTree) { }
        protected override void InitCycles(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.BlocksPair<T>> reverseEdges, IDominatorRelation<T> domTree)
        {
            cycles = new List<Cycle<T>>();
            foreach (T n in blocks)
            {
                List<T> Ds = reverseEdges.FindAll(pair => pair.blockEnd.CompareTo(n) == 0).Select(pair => pair.blockBegin).ToList();
                List<KeyValuePair<List<T>, T>> DsGroupedInCycles = GroupingInCycles(n, Ds, domTree);
                foreach(KeyValuePair<List<T>, T> group in DsGroupedInCycles)
                {
                    System.Diagnostics.Debug.Assert(group.Key.Count() <= 2);
                    Dictionary<T, bool> mark = new Dictionary<T, bool>();
                    foreach (T bl in blocks)
                        mark[bl] = false;
                    T leastBothDominator = n;
                    List<T> ds = new List<T>();
                    List<T> finded = null;
                    if (group.Key.Count() == 1)
                    {
                        ds.Add(group.Key.First());
                        finded = Find(n, group.Key.First(), group.Key.First(), mark, graph, domTree);
                        cycles.Add(new Cycle<T>(n, ds, finded));
                    }
                    else if (group.Key.Count() == 2)
                    {
                        ds.Add(group.Key.First());
                        ds.Add(group.Key[1]);
                        finded = Find(n, group.Key.First(), group.Key.First(), mark, graph, domTree);
                        finded.AddRange(Find(n, group.Key[1], group.Key[1], mark, graph, domTree));
                        cycles.Add(new Cycle<T>(n, ds, finded, group.Value));
                    }
                    else
                        System.Diagnostics.Debug.Assert(false);
                    
                }
            }
        }

        private List<KeyValuePair<List<T>, T>> GroupingInCycles(T n, List<T> Ds, IDominatorRelation<T> domTree)
        {
            IComparer<T> domComparer = new DomComparor<T>(domTree);
            bool[] added = new bool[Ds.Count()];
            List<KeyValuePair<List<T>, T>> groups = new List<KeyValuePair<List<T>, T>>();
            for (int i = 0; i < Ds.Count(); i++)
            {
                if (added[i])
                    continue;
                T leastBothDominator = new T();
                List<T> l = new List<T>(2);
                l.Add(Ds[i]);
                for (int j = i; j < Ds.Count(); j++)
                {
                    if (!domTree.FirstDomSeccond(Ds[i], Ds[j]) && !domTree.FirstDomSeccond(Ds[j], Ds[i]))
                    {
                        IEnumerable<T> bothDomsUnderN = domTree.UpperDominators(Ds[i]).Intersect(domTree.UpperDominators(Ds[j])).Intersect(domTree.DownDominators(n));
                        if (bothDomsUnderN.Count() > 0)
                        {
                            leastBothDominator = bothDomsUnderN.OrderBy(e => e, domComparer).Last();
                            l.Add(Ds[j]);
                            added[j] = true;
                            break;
                        }
                    }
                }
                groups.Add(new KeyValuePair<List<T>, T>(l, leastBothDominator));
            }
            return groups;
        }
        class DomComparor<T1>: IComparer<T1> where T1: IComparable<T1>
        {
            public DomComparor(IDominatorRelation<T1> domTree)
            {
                this.domTree = domTree;
            }
            int IComparer<T1>.Compare(T1 x, T1 y)
            {
                return x.CompareTo(y) == 0 ? 0 : domTree.FirstDomSeccond(x, y) ? -1 : 1;
            }

            public IDominatorRelation<T1> domTree { get; private set; }
        }
    }
}