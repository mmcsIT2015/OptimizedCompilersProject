using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    /// <summary>
    /// Класс изменяет входящий threeAddrCode - на выходе этот код будет разбит на блоки
    /// Example:
    /// ====
    ///     Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
    ///     codeGenerator.Visit(parser.root);
    ///     
    ///     BaseBlocksPartition baseBlocksPartition = new BaseBlocksPartition(codeGenerator.Code);
    ///     Console.WriteLine(codeGenerator.Code);
    ///     
    /// </summary>
    class BaseBlocksPartition
    {
        private List<Block> blocks; //список базовых блоков

        readonly Dictionary<int, List<int>> graph; //граф переходов между базовыми блоками (по индексам в массиве)

        //обратный граф переходов между базовыми блоками
        public Dictionary<int, List<int>> GetReversedGraph()
        {
            bool[,] graphTable = new bool[blocks.Count(), blocks.Count()];
            graphTable.Initialize();
            for (int i = 0; i < blocks.Count(); i++)
            {
                foreach (int j in graph[i])
                    graphTable[i, j] = true;
            }
            Dictionary<int, List<int>> reversedGraph = new Dictionary<int, List<int>>();
            for (int i = 0; i < blocks.Count(); i++)
                reversedGraph[i] = new List<int>(2);
            for (int i = 0; i < blocks.Count(); i++)
                for (int j = 0; j < blocks.Count(); j++)
                    if (graphTable[i, j])
                        reversedGraph[j].Add(i);
            return reversedGraph;
        }

        public BaseBlocksPartition(ThreeAddrCode threeAddrCode)
        {
            graph = new Dictionary<int, List<int>>();
            blocks = new List<Block>();
            Partition(threeAddrCode);

            threeAddrCode.blocks = blocks;
        }

        private void Partition(ThreeAddrCode threeAddrCode)
        {
            if (threeAddrCode.blocks.Count() != 1)
                return;
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
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < blocks.Count(); i++)
            {
                builder.Append("BLOCK " + i + "\n");

                builder.Append(blocks[i].ToString());

                builder.Append("TRANSITION TO BLOCKS: ");
                foreach (int index in graph[i])
                {
                    builder.Append(" " + index);
                }
                builder.Append("\n\n");
            }

            return builder.ToString();
        }
    }
}
