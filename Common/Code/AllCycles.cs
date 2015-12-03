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

    /// <summary>
    /// Абстрактный класс цикла
    /// (2 вида циклов)
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public abstract class CycleAbstract<T>
    {
        /// <summary>
        /// Вход в цикл
        /// </summary>
        public T N { get; set; }

        /// <summary>
        /// Все вершины, принадлежащие циклу
        /// </summary>
        public List<T> DATA { get; set; }
    }

    /// <summary>
    /// Цикл с одним выходом
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class CycleUsual<T>: CycleAbstract<T>
    {
        public CycleUsual(T n, T d, List<T> data)
        {
            this.N = n;
            this.DATA = data;
            this.D = d;
        }

        /// <summary>
        /// Выход из цикла
        /// </summary>
        public T D { get; set; }
    }

    /// <summary>
    /// Цикл с двумя выходами
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class CycleSpecialCase<T> : CycleAbstract<T>
    {
        public CycleSpecialCase(T n, T d1, T d2, List<T> data, T dom)
        {
            this.N = n;
            this.DATA = data;
            this.D1 = d1;
            this.D2 = d2;
            this.DOM = dom;
        }

        /// <summary>
        /// Первый выход
        /// </summary>
        public T D1 { get; set; }

        /// <summary>
        /// Второй выход
        /// </summary>
        public T D2 { get; set; }

        /// <summary>
        /// Ближайший общий доминатор для 2-х выходов из цикла
        /// </summary>
        public T DOM { get; set; }
    }

    /// <summary>
    /// Обределяет и хранит все циклы CFG, в том числе вложенные.
    /// Считает граф, подаваемый на вход, приводимым!
    /// Использование
    ///     Инициализация
    ///     AllCycles < Block > allCycles = new AllCyclesWithSpecialCase<Block>(code.blocks, code.graph, reverseEdges, domTree);
    ///     
    ///     Получить все циклы
    ///     List < CycleAbstract < Block > > cycles = allCycles.cycles;
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class AllCycles<T> where T : IComparable<T>
    {
        /// <summary>
        /// Все циклы
        /// </summary>
        public List<CycleAbstract<T>> cycles { get; protected set; }

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
            cycles = new List<CycleAbstract<T>>();
            foreach (T n in blocks)
            {
                foreach (T d in reverseEdges.FindAll(pair => pair.blockEnd.CompareTo(n) == 0).Select(pair => pair.blockBegin))
                {
                    Dictionary<T, bool> mark = new Dictionary<T, bool>();
                    foreach (T bl in blocks)
                        mark[bl] = false;
                    cycles.Add(new CycleUsual<T>(n, d, Find(n, d, d, mark, graph, domTree)));
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
    public class AllCyclesWithSpecialCase<T> : AllCycles<T> where T: IComparable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocks">Список блоков</param>
        /// <param name="graph">CFG</param>
        /// <param name="reverseEdges">Список обратных дуг</param>
        /// <param name="domTree">Дерево доминирования</param>
        public AllCyclesWithSpecialCase(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.BlocksPair<T>> reverseEdges, IDominatorRelation<T> domTree)
            : base(blocks, graph, reverseEdges, domTree) { }
        protected override void InitCycles(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.BlocksPair<T>> reverseEdges, IDominatorRelation<T> domTree)
        {
            cycles = new List<CycleAbstract<T>>();
            foreach (T n in blocks)
            {
                List<T> Ds = reverseEdges.FindAll(pair => pair.blockEnd.CompareTo(n) == 0).Select(pair => pair.blockBegin).ToList();
                List<CycleAbstract<T>> grouped = GroupingInCycles(n, Ds, domTree);
                for (int i = 0; i < grouped.Count; i++ )
                {
                    Dictionary<T, bool> mark = new Dictionary<T, bool>();
                    foreach (T bl in blocks)
                        mark[bl] = false;
                    if (grouped[i] is CycleUsual<T>)
                    {
                        CycleUsual<T> c = grouped[i] as CycleUsual<T>;
                        c.DATA = Find(n, c.D, c.D, mark, graph, domTree);
                    }
                    else if (grouped[i] is CycleSpecialCase<T>)
                    {
                        CycleSpecialCase<T> c = grouped[i] as CycleSpecialCase<T>;
                        List<T> finded = Find(n, c.D1, c.D1, mark, graph, domTree);
                        finded.AddRange(Find(n, c.D2, c.D2, mark, graph, domTree));
                        c.DATA = finded;
                    }
                }
                cycles.AddRange(grouped);
            }
        }

        private List<CycleAbstract<T>> GroupingInCycles(T n, List<T> Ds, IDominatorRelation<T> domTree)
        {
            bool[] added = new bool[Ds.Count()];
            List<CycleAbstract<T>> groups = new List<CycleAbstract<T>>();
            for (int i = 0; i < Ds.Count(); i++)
            {
                if (added[i])
                    continue;
                bool ususalCycle = true;
                T D1 = Ds[i];
                for (int j = i; j < Ds.Count(); j++)
                {
                    if (!domTree.FirstDomSeccond(Ds[i], Ds[j]) && !domTree.FirstDomSeccond(Ds[j], Ds[i]))
                    {
                        T leastDom = domTree.UpperDominators(Ds[i]).Intersect(domTree.UpperDominators(Ds[j])).Intersect(domTree.DownDominators(n)).OrderBy(e => e, new DomComparor<T>(domTree)).Last();
                        T D2 = Ds[j];
                        added[j] = true;
                        ususalCycle = false;
                        groups.Add(new CycleSpecialCase<T>(n, D1, D2, null, leastDom));
                        break;
                    }
                }
                if (ususalCycle)
                    groups.Add(new CycleUsual<T>(n, D1, null));
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