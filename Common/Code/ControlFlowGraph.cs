using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.Code
{
    /// <summary>
    /// Объект графа потока управления
    /// Использование:
    ///     - Инициализация
    ///     var graph = new ControlFlowGraph(threeAddrCode.blocks);
    ///     
    ///     - Получить список всех блоков, в которые есть вход из блока `source`
    ///     foreach (var block in graph.OutEdges(source)) {...}
    ///     
    ///     - Получить список всех блоков, из которых есть вход в блок `sink`
    ///     foreach (var block in graph.InEdges(sink)) {...}
    /// </summary>
    public class ControlFlowGraph : IGraph<Block>
    {
        private Block mEntryPoint;
        private Dictionary<Block, IList<Block>> mGraph;
        private Dictionary<Block, IList<Block>> mReversedGraph;

        private Dictionary<int, List<int>> getIndexedGraph(IList<Block> blocks)
        {
            mEntryPoint = blocks.First();

            var labelsToBlocksIndexes = new Dictionary<string, int>();
            for (int i = 0; i < blocks.Count(); i++)
            {
                if (blocks[i][0].HasLabel()) labelsToBlocksIndexes[blocks[i][0].label] = i;
            }

            var graph = new Dictionary<int, List<int>>();
            for (int i = 0; i < blocks.Count(); i++)
            {
                graph[i] = new List<int>(2);
            }

            for (int i = 0; i < blocks.Count(); i++)
            {
                var line = blocks[i].Last();
                if (line is Line.GoTo)
                {
                    if (line is Line.СonditionalJump)
                    {
                        if (i < blocks.Count() - 1) graph[i].Add(i + 1);
                    }

                    graph[i].Add(labelsToBlocksIndexes[(line as Line.GoTo).target]);
                }
                else if (i < blocks.Count() - 1)
                {
                    graph[i].Add(i + 1);
                }
            }

            return graph;
        }

        private Dictionary<int, List<int>> getReversedIndexedGraph(Dictionary<int, List<int>> graph)
        {
            bool[,] graphTable = new bool[graph.Count(), graph.Count()];
            graphTable.Initialize();
            for (int i = 0; i < graph.Count(); i++)
            {
                foreach (int j in graph[i])
                {
                    graphTable[i, j] = true;
                }
            }

            Dictionary<int, List<int>> reversedGraph = new Dictionary<int, List<int>>();
            for (int i = 0; i < graph.Count(); i++) reversedGraph[i] = new List<int>(2);
            for (int i = 0; i < graph.Count(); i++)
            {
                for (int j = 0; j < graph.Count(); j++)
                {
                    if (graphTable[i, j]) reversedGraph[j].Add(i);
                }
            }

            return reversedGraph;
        }

        public ControlFlowGraph(IList<Block> blocks)
        {
            mGraph = new Dictionary<Block, IList<Block>>();
            var indGr = getIndexedGraph(blocks);
            for (int i = 0; i < blocks.Count; i++)
            {
                mGraph[blocks[i]] = new List<Block>(2);
                foreach (Block b in indGr[i].Select(ind => blocks[ind]))
                {
                    mGraph[blocks[i]].Add(b);
                }
            }

            mReversedGraph = new Dictionary<Block, IList<Block>>();
            var indices = getReversedIndexedGraph(indGr);
            for (int i = 0; i < blocks.Count; i++)
            {
                mReversedGraph[blocks[i]] = new List<Block>();
                foreach (Block block in indices[i].Select(ind => blocks[ind]))
                {
                    mReversedGraph[blocks[i]].Add(block);
                }
            }
        }

        public Block EntryPoint()
        {
            return mEntryPoint;
        }

        public IEnumerable<Block> OutEdges(Block block)
        {
            return mGraph[block];
        }

        public IEnumerable<Block> InEdges(Block block)
        {
            return mReversedGraph[block];
        }
    }
}
