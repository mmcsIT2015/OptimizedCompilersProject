using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    /// <summary>
    /// Тестирует остовое дерево управляющего графа из лекции
    /// </summary>
    public static class SpaningTreeTesting
    {
        public static void Test()
        {
            TestGraph graph = new TestGraph();
            int[] blocks = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Console.WriteLine("RECUR METHOD");
            SpanningTree<int> t = new SpanningTree<int>(blocks, graph);
            for (int i = 1; i <= 10; i++)
            {
                Console.Write(i + "(" + t.Numbers[i] + "): ");
                foreach (int v in t.Data[i])
                    Console.Write(v + " ");
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.WriteLine("NOT RECUR METHOD");
            t = new SpanningTreeWithoutRecursive<int>(blocks, graph);
            for (int i = 1; i <= 10; i++)
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
    /// Управляющий граф из лекции
    /// </summary>
    public class TestGraph : IGraph<int>
    {
        private Dictionary<int, IList<int>> Data;
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
        }

        public IEnumerable<int> OutEdges(int block)
        {
            return Data[block];
        }

        public IEnumerable<int> InEdges(int block)
        {
            throw new Exception();
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
    public class SpanningTree<T>
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

        public Dictionary<T, int> Numbers { get; private set; }
        public Dictionary<T, List<T>> Data { get; private set; }

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
                if (enumers.Count() == 0)
                    break;
                e = enumers.Pop();
                root = parents.Pop();
            }
        }
    }
}