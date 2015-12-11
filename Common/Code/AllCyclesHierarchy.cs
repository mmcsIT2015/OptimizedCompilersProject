using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    /// <summary>
    /// Находит все вложенные циклы
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AllCyclesHierarchy<T>
    {
        public AllCyclesHierarchy(List<Cycle<T>> list)
        {
            data = new Dictionary<Cycle<T>, List<Cycle<T>>>();
            Dictionary<Cycle<T>, bool> referenced = new Dictionary<Cycle<T>, bool>();
            foreach (Cycle<T> cycle in list)
            {
                data[cycle] = new List<Cycle<T>>();
                referenced[cycle] = false;
            }
            //сортировка от самого общего цикла к самому меньшему
            list = list.OrderBy(e => e, new CycleComparer()).ToList();
            for (int i = list.Count-2; i >= 0; i--)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    //list[j] подмножество list[i]? && list[j] не состоит в дереве
                    if (!list[j].DATA.Except(list[i].DATA).Any() && !referenced[list[j]])
                    {
                        data[list[i]].Add(list[j]);
                        referenced[list[j]] = true;
                    }
                }
            }
            root = referenced.Where(e => !e.Value).Select(e => e.Key).ToList();
        }
        public class CycleComparer : IComparer<Cycle<T>>
        {
            int IComparer<Cycle<T>>.Compare(Cycle<T> x, Cycle<T> y)
            {
                return x.Equals(y) ? 0 : !x.DATA.Except(y.DATA).Any() ? 1 : !y.DATA.Except(x.DATA).Any() ? -1 : 0;
            }
        }
        /// <summary>
        /// Корень дерева
        /// </summary>
        public List<Cycle<T>> root { get; private set; }
        /// <summary>
        /// Дерево
        /// </summary>
        public Dictionary<Cycle<T>, List<Cycle<T>>> data { get; private set; }
    }
}
