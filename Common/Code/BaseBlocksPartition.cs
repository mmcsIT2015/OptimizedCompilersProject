using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    /// <summary>
    /// Класс изменяет входящий threeAddrCode - на выходе этот код будет разбит на блоки и будет построен граф переходов между блоками
    /// (!)
    /// Вызывать вручную разбиение на блоки не требуется - оно происходит при конструировании трехадресного кода из List<Line.Line>.
    /// </summary>
    static class BaseBlocksPartition
    {
        public static void Partition(ThreeAddrCode threeAddrCode)
        {
            if (threeAddrCode.blocks.Count() != 1) return;

            List<Block> blocks = new List<Block>();
            var currentBlock = new Block();
            foreach (var line in threeAddrCode.blocks[0])
            {
                if (line.HasLabel() && currentBlock.Count() > 0)
                {
                    blocks.Add(currentBlock);
                    currentBlock = new Block();
                }
                currentBlock.Add(line);
                if (line is Line.GoTo) // Line.ConditionalJump является GoTo
                {
                    blocks.Add(currentBlock);
                    currentBlock = new Block();
                }
            }

            if (currentBlock.Count() > 0) blocks.Add(currentBlock);

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

            threeAddrCode.blocks = blocks;
            threeAddrCode.graph = graph;
        }
    }
}
