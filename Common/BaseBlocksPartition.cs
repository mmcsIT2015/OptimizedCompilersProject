using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    /// <summary>
    /// Класс изменяет входящий threeAddrCode - на выходе этот код будет разбит на блоки и будет построен граф переходов между блоками
    /// Example:
    /// ====
    ///     Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
    ///     codeGenerator.Visit(parser.root);
    ///     
    ///     BaseBlocksPartition.Partition(codeGenerator.Code);
    ///     
    ///     Console.WriteLine(codeGenerator.Code);
    ///     
    /// </summary>
    static class BaseBlocksPartition
    {
        public static void Partition(ThreeAddrCode threeAddrCode)
        {
            if (threeAddrCode.blocks.Count() != 1)
                return;
            List<Block> blocks = new List<Block>();
            Block currentBlock = new Block();
            foreach (ThreeAddrCode.Line line in threeAddrCode.blocks[0])
            {
                if (line.label != "" && currentBlock.Count() > 0)
                {
                    blocks.Add(currentBlock);
                    currentBlock = new Block();
                }
                currentBlock.Add(line);
                if (line.command == "goto" || line.command == "if")
                {
                    blocks.Add(currentBlock);
                    currentBlock = new Block();
                }
            }
            if (currentBlock.Count() > 0)
                blocks.Add(currentBlock);

            Dictionary<string, int> labelsToBlocksIndexes = new Dictionary<string, int>();
            for (int i = 0; i < blocks.Count(); i++)
                if (blocks[i][0].label != "")
                    labelsToBlocksIndexes[blocks[i][0].label] = i;

            Dictionary<int, List<int>> graph = new Dictionary<int,List<int>>();
            for (int i = 0; i < blocks.Count();i++ )
                graph[i] = new List<int>(2);
            for (int i = 0; i < blocks.Count(); i++)
            {
                ThreeAddrCode.Line l = blocks[i].Last();
                if (l.command == "goto")
                    graph[i].Add(labelsToBlocksIndexes[l.left]);
                else if (l.command == "if")
                {
                    if (i < blocks.Count() - 1)
                        graph[i].Add(i + 1);
                    graph[i].Add(labelsToBlocksIndexes[l.first]);
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
