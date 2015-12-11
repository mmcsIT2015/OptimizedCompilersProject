using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    public class AllCyclesHierarchy<T>
    {
        private Dictionary<Cycle<T>, List<Cycle<T>>> hierarchy = new Dictionary<Cycle<T>, List<Cycle<T>>>();
        private List<Cycle<T>> mCycles = new List<Cycle<T>>();

        public AllCyclesHierarchy(List<Cycle<T>> cycles)
        {
            mCycles = cycles;
        }

        public void HierarchyAlgo()
        {
            for (int i = 0; i < mCycles.Count; ++i)
                if (mCycles[i] is CycleSpecialCase<T>)
                {
                    CycleSpecialCase<T> cycleSpec = mCycles[i] as CycleSpecialCase<T>;

                    List<T> vertices = new List<T>();
                    vertices = cycleSpec.DATA;
                    vertices.Remove(cycleSpec.D2);
                    vertices.Remove(cycleSpec.D1);

                    List<DomGraph.ValPair<T>> outs1 = new List<DomGraph.ValPair<T>>();
                    List<DomGraph.ValPair<T>> outs2 = new List<DomGraph.ValPair<T>>();

                    outs1.Add(new DomGraph.ValPair<T>(cycleSpec.D1, mCycles[i].N));
                    outs2.Add(new DomGraph.ValPair<T>(cycleSpec.D2, mCycles[i].N));

                    List<T> vertices1 = new List<T>();
                    vertices1.AddRange(vertices);
                    vertices1.Add(cycleSpec.D1);

                    List<T> vertices2 = new List<T>();
                    vertices2.AddRange(vertices);
                    vertices2.Add(cycleSpec.D2);

                    CycleUsual<T> c1 = new CycleUsual<T>(mCycles[i].N, vertices1,
                        outs1, cycleSpec.D1);

                    CycleUsual<T> c2 = new CycleUsual<T>(mCycles[i].N, vertices2,
                        outs2, cycleSpec.D2);

                    List<Cycle<T>> cycles = new List<Cycle<T>>();
                    cycles.Add(c1);
                    cycles.Add(c2);

                    hierarchy[cycleSpec] = cycles;
                }
        }

        public void PrintHierarchy(List<Cycle<T>> root)
        {
            if (root.Count == 0)
                return;
            if (root.Count == 1)
            {
                if (root[0] is CycleUsual<T>)
                {
                    CycleUsual<T> c = root[0] as CycleUsual<T>;
                    Console.WriteLine("Вход в цикл: {0}", c.N.ToString());
                    Console.WriteLine("Выход из цикла: {0}", c.D.ToString());
                    Console.WriteLine("Вершины цикла:");
                    for (int i = 0; i < c.DATA.Count; ++i)
                        Console.WriteLine(c.DATA[i].ToString() + " ");
                }
                if (root[0] is CycleSpecialCase<T>)
                {
                    CycleSpecialCase<T> c = root[0] as CycleSpecialCase<T>;
                    Console.WriteLine("Вход в цикл: {0}", c.N.ToString());
                    Console.WriteLine("Выходы из цикла: {0} , {1} ", c.D1.ToString(), c.D2.ToString());
                    Console.WriteLine("Вершины цикла:");
                    for (int i = 0; i < c.DATA.Count; ++i)
                        Console.WriteLine(c.DATA[i].ToString() + " ");
                }

                if (hierarchy.ContainsKey(root[0]))
                {
                    Console.WriteLine("Вложенные циклы для этого цикла:");
                    PrintHierarchy(hierarchy[root[0]]);
                }
                else
                {
                    Console.WriteLine("Нет вложенных циклов на этом уровне");
                }
            }
            if (root.Count == 2)
            {
                if (root[0] is CycleUsual<T>)
                {
                    CycleUsual<T> c = root[0] as CycleUsual<T>;
                    Console.WriteLine("Вход в цикл: {0}", c.N.ToString());
                    Console.WriteLine("Выход из цикла: {0}", c.D.ToString());
                    Console.WriteLine("Вершины цикла:");
                    for (int i = 0; i < c.DATA.Count; ++i)
                        Console.WriteLine(c.DATA[i].ToString() + " ");
                }
                if (root[0] is CycleSpecialCase<T>)
                {
                    CycleSpecialCase<T> c = root[0] as CycleSpecialCase<T>;
                    Console.WriteLine("Вход в цикл: {0}", c.N.ToString());
                    Console.WriteLine("Выходы из цикла: {0} , {1} ", c.D1.ToString(), c.D2.ToString());
                    Console.WriteLine("Вершины цикла:");
                    for (int i = 0; i < c.DATA.Count; ++i)
                        Console.WriteLine(c.DATA[i].ToString() + " ");
                }

                if (hierarchy.ContainsKey(root[0]))
                {
                    Console.WriteLine("Вложенные циклы для этого цикла:");
                    PrintHierarchy(hierarchy[root[0]]);
                }
                else
                {
                    Console.WriteLine("Нет вложенных циклов на этом уровне");
                }

                if (root[1] is CycleUsual<T>)
                {
                    CycleUsual<T> c = root[1] as CycleUsual<T>;
                    Console.WriteLine("Вход в цикл: {0}", c.N.ToString());
                    Console.WriteLine("Выход из цикла: {0}", c.D.ToString());
                    Console.WriteLine("Вершины цикла:");
                    for (int i = 0; i < c.DATA.Count; ++i)
                        Console.WriteLine(c.DATA[i].ToString() + " ");
                }
                if (root[1] is CycleSpecialCase<T>)
                {
                    CycleSpecialCase<T> c = root[1] as CycleSpecialCase<T>;
                    Console.WriteLine("Вход в цикл: {0}", c.N.ToString());
                    Console.WriteLine("Выходы из цикла: {0} , {1} ", c.D1.ToString(), c.D2.ToString());
                    Console.WriteLine("Вершины цикла:");
                    for (int i = 0; i < c.DATA.Count; ++i)
                        Console.WriteLine(c.DATA[i].ToString() + " ");
                }

                if (hierarchy.ContainsKey(root[1]))
                {
                    Console.WriteLine("Вложенные циклы для этого цикла:");
                    PrintHierarchy(hierarchy[root[1]]);
                }
                else
                {
                    Console.WriteLine("Нет вложенных циклов на этом уровне");
                }

            }


        }
    }
}
