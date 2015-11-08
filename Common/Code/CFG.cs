using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.Code
{
    /// <summary>
    /// Объект графа потока управления
    /// Использование:
    ///     Инициализация
    ///     CFG g = new CFG(threeAddrCode.blocks);
    ///     
    ///     Получить список всех блоков, следующих за блоком b
    ///     foreach(Block b in g.GetOutBlocks(b)) {...}
    ///     
    ///     Получить список всех блоков, предшествующих блоку b
    ///     foreach(Block b in g.GetInBlocks(b)) {...}
    /// </summary>
    public class CFG
    {
        private Dictionary<Block, IList<Block>> graph;
        private Dictionary<Block, IList<Block>> reversedGraph;

        private Dictionary<int, List<int>> getIndexedGraph(IList<Block> blocks)
        {
            Dictionary<string, int> labelsToBlocksIndexes = new Dictionary<string, int>();
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
                    graphTable[i, j] = true;
            }
            Dictionary<int, List<int>> reversedGraph = new Dictionary<int, List<int>>();
            for (int i = 0; i < graph.Count(); i++)
                reversedGraph[i] = new List<int>(2);
            for (int i = 0; i < graph.Count(); i++)
                for (int j = 0; j < graph.Count(); j++)
                    if (graphTable[i, j])
                        reversedGraph[j].Add(i);
            return reversedGraph;
        }

        public CFG(IList<Block> blocks)
        {
            graph = new Dictionary<Block, IList<Block>>();
            var indGr = getIndexedGraph(blocks);
            for (int i = 0; i < blocks.Count; i++)
            {
                graph[blocks[i]] = new List<Block>(2);
                foreach (Block b in indGr[i].Select(ind => blocks[ind]))
                    graph[blocks[i]].Add(b);
            }

            reversedGraph = new Dictionary<Block, IList<Block>>();
            var revIndGr = getReversedIndexedGraph(indGr);
            for (int i = 0; i < blocks.Count; i++)
            {
                reversedGraph[blocks[i]] = new List<Block>();
                foreach (Block b in revIndGr[i].Select(ind => blocks[ind]))
                    reversedGraph[blocks[i]].Add(b);
            }
        }

        public IEnumerable<Block> GetOutBlocks(Block block)
        {
            return graph[block];
        }

        public IEnumerable<Block> GetInBlocks(Block block)
        {
            return reversedGraph[block];
        }
    }
}
