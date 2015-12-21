using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    /// <summary>
    /// Тестирует остовое дерево управляющего графа из лекции
    /// </summary>
    public static class SpaningTreeTesting
    {
        public static void Test()
        {
            AllCyclesTesting.TestingSpanningTree();
        }
    }

    /// <summary>
    /// Строит и хранит остовное дерево по графу потока управления
    /// Использование:
    ///     Инициализация
    ///     SpanningTree<Block> tree = new SpanningTreeWithoutRecursive<Block>(code.blocks, code.graph);
    ///     
    ///     Получить номер блока b (от 0 до n-1)
    ///     int i = tree.Numbers[b];
    ///     
    ///     Получить всех потомков блока b в остовном дереве tree
    ///     List<Block> blocks = tree.Data[b];
    /// </summary>
    /// <typeparam name="T">Тип данных вершин дерева</typeparam>
    public abstract class SpanningTree<T>
    {
        public SpanningTree(IEnumerable<T> blocks, IGraph<T> graph)
        {
            Numbers = new Dictionary<T, int>();
            Data = new Dictionary<T, List<T>>();
            foreach (T b in blocks)
                Data[b] = new List<T>();
            int index = 0;
            FindSpanningTree(blocks.First(), ref index, graph);
        }

        protected SpanningTree() { }

        public Dictionary<T, int> Numbers { get; protected set; }
        public Dictionary<T, List<T>> Data { get; protected set; }

        protected virtual void FindSpanningTree(T root, ref int index, IGraph<T> graph)
        {
            Numbers[root] = index;
            index++;
            foreach (var b in graph.OutEdges(root))
                if (!Numbers.ContainsKey(b))
                {
                    Data[root].Add(b);
                    FindSpanningTree(b, ref index, graph);
                }
        }
    }

    /// <summary>
    /// Реализация без использования рекурсивных вызовов
    /// Может быть работает быстрее, чем SpanningTree
    /// </summary>
    /// <typeparam name="T">Тип данных вершин дерева</typeparam>
    public class SpanningTreeWithoutRecursive<T> : SpanningTree<T>
    {
        public SpanningTreeWithoutRecursive(IEnumerable<T> blocks, IGraph<T> graph)
            : base(blocks, graph) { }

        protected override void FindSpanningTree(T root, ref int index, IGraph<T> graph)
        {
            Stack<IEnumerator<T>> enumers = new Stack<IEnumerator<T>>();
            Stack<T> parents = new Stack<T>();

            Numbers[root] = index;
            index++;
            var e = graph.OutEdges(root).GetEnumerator();
            while (true)
            {
                while (e.MoveNext())
                {
                    if (!Numbers.ContainsKey(e.Current))
                    {
                        Data[root].Add(e.Current);
                        parents.Push(root);
                        root = e.Current;
                        enumers.Push(e);
                        e = graph.OutEdges(root).GetEnumerator();
                        Numbers[root] = index;
                        index++;
                    }
                }

                if (enumers.Count() == 0) break;

                e = enumers.Pop();
                root = parents.Pop();
            }
        }
    }
}