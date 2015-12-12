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
    }

    /// <summary>
    /// Класс цикла
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public abstract class Cycle<T>
    {
        /// <summary>
        /// Вход в цикл
        /// </summary>
        public T N { get; set; }

        /// <summary>
        /// Все вершины, принадлежащие циклу
        /// </summary>
        public List<T> DATA { get; set; }

        /// <summary>
        /// Ребра - выходы из цикла
        /// </summary>
        public List<DomGraph.ValPair<T>> OUTS { get; set; }
    }

    /// <summary>
    /// Цикл с одной обратной дугой
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class CycleUsual<T>: Cycle<T>
    {
        public CycleUsual(T n, List<T> data, List<DomGraph.ValPair<T>> outs, T d)
        {
            this.N = n;
            this.DATA = data;
            this.OUTS = outs;
            this.D = d;
        }

        /// <summary>
        /// Вершина из обратного ребра
        /// </summary>
        public T D { get; set; }
    }

    /// <summary>
    /// Цикл с двумя обратными дугами
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class CycleSpecialCase<T> : Cycle<T>
    {
        public CycleSpecialCase(T n, List<T> data, List<DomGraph.ValPair<T>> outs, T d1, T d2)
        {
            this.N = n;
            this.DATA = data;
            this.OUTS = outs;
            this.D1 = d1;
            this.D2 = d2;
        }

        /// <summary>
        /// Вершина из первого обратного ребра
        /// </summary>
        public T D1 { get; set; }

        /// <summary>
        /// Вершина из второго обратного ребра
        /// </summary>
        public T D2 { get; set; }
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
    public class AllCycles<T>
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
        public AllCycles(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.ValPair<T>> reverseEdges, IDominatorRelation<T> domTree)
        {
            InitCycles(blocks, graph, reverseEdges, domTree);
        }

        protected virtual void InitCycles(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.ValPair<T>> reverseEdges, IDominatorRelation<T> domTree)
        {
            cycles = new List<Cycle<T>>();
            foreach (T n in blocks)
            {
                foreach (T d in reverseEdges.FindAll(pair => pair.valEnd.Equals(n)).Select(pair => pair.valBegin))
                {
                    Dictionary<T, bool> mark = new Dictionary<T, bool>();
                    foreach (T bl in blocks)
                        mark[bl] = false;
                    List<T> verts = FindVerts(n, d, d, mark, graph, domTree);
                    List<DomGraph.ValPair<T>> outs = FindOuts(n, graph, domTree, verts);
                    cycles.Add(new CycleUsual<T>(n, verts, outs, d));
                }
            }
        }

        protected static List<T> FindVerts(T n, T d, T x, Dictionary<T, bool> mark, IGraph<T> graph, IDominatorRelation<T> domTree)
        {
            List<T> verts = new List<T>();
            if (mark[x])
            {
                return verts;
            }
            else if (n.Equals(x))
            {
                mark[n] = true;
                verts.Add(n);
                return verts;
            }
            mark[x] = true;
            verts.Add(x);
            foreach (T prev in graph.InEdges(x).Where(e => !domTree.FirstDomSeccond(d, e)))
            {
                verts.AddRange(FindVerts(n, d, prev, mark, graph, domTree));
            }
            return verts;
        }

        protected static List<DomGraph.ValPair<T>> FindOuts(T n, IGraph<T> graph, IDominatorRelation<T> domTree, List<T> verts)
        {
            return verts.Select(v => graph.OutEdges(v).Select(v1 => new DomGraph.ValPair<T>(v, v1)))
                .Aggregate(new List<DomGraph.ValPair<T>>(), (l, e) => { l.AddRange(e); return l; })
                .Where(e => verts.All(u => !e.valEnd.Equals(u)) && domTree.UpperDominators(n).All(u => !e.valEnd.Equals(u)))
                .ToList();
        }
    }

    /// <summary>
    /// !ОТЛИЧИЕ:
    ///     Обрабатывает "особый случай" с двумя обратными дугами (см. лекцию) 
    /// Обределяет и хранит все циклы CFG, в том числе вложенные.
    /// Считает граф, подаваемый на вход, приводимым!
    /// </summary>
    /// <typeparam name="T">Тип вершин</typeparam>
    public class AllCyclesWithSpecialCase<T> : AllCycles<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocks">Список блоков</param>
        /// <param name="graph">CFG</param>
        /// <param name="reverseEdges">Список обратных дуг</param>
        /// <param name="domTree">Дерево доминирования</param>
        public AllCyclesWithSpecialCase(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.ValPair<T>> reverseEdges, IDominatorRelation<T> domTree)
            : base(blocks, graph, reverseEdges, domTree) { }
        protected override void InitCycles(IEnumerable<T> blocks, IGraph<T> graph, List<DomGraph.ValPair<T>> reverseEdges, IDominatorRelation<T> domTree)
        {
            cycles = new List<Cycle<T>>();
            foreach (T n in blocks)
            {
                List<T> Ds = reverseEdges.FindAll(pair => pair.valEnd.Equals(n)).Select(pair => pair.valBegin).ToList();
                List<Cycle<T>> grouped = GroupingInCycles(n, Ds, domTree);
                for (int i = 0; i < grouped.Count; i++ )
                {
                    Dictionary<T, bool> mark = new Dictionary<T, bool>();
                    foreach (T bl in blocks)
                        mark[bl] = false;
                    List<T> verts = null;
                    if (grouped[i] is CycleUsual<T>)
                    {
                        CycleUsual<T> c = grouped[i] as CycleUsual<T>;
                        verts = FindVerts(n, c.D, c.D, mark, graph, domTree);
                    }
                    else if (grouped[i] is CycleSpecialCase<T>)
                    {
                        CycleSpecialCase<T> c = grouped[i] as CycleSpecialCase<T>;
                        verts = FindVerts(n, c.D1, c.D1, mark, graph, domTree);
                        verts.AddRange(FindVerts(n, c.D2, c.D2, mark, graph, domTree));
                    }
                    grouped[i].DATA = verts;
                    grouped[i].OUTS = FindOuts(n, graph, domTree, verts);
                }
                cycles.AddRange(grouped);
            }
        }

        private List<Cycle<T>> GroupingInCycles(T n, List<T> Ds, IDominatorRelation<T> domTree)
        {
            bool[] added = new bool[Ds.Count()];
            List<Cycle<T>> groups = new List<Cycle<T>>();
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
                        T D2 = Ds[j];
                        added[j] = true;
                        ususalCycle = false;
                        groups.Add(new CycleSpecialCase<T>(n, null, null, D1, D2));
                        break;
                    }
                }
                if (ususalCycle)
                    groups.Add(new CycleUsual<T>(n, null, null, D1));
            }
            return groups;
        }
    }
}