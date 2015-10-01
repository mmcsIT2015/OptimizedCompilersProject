using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    using Block = List<ThreeAddrCode.Line>;

    class BaseBlocksPartition
    {
        readonly List<Block> blocks; //список базовых блоков

        readonly Dictionary<int, List<int>> graph; //граф переходов между безовыми блоками (по индексам в массиве)

        public BaseBlocksPartition(ThreeAddrCode threeAddrCode)
        {
            graph = new Dictionary<int, List<int>>();
            blocks = new List<Block>();
            Partition(threeAddrCode);
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
                    //if (i < blocks.Count() - 1)
                        graph[i].Add(i + 1);
                    graph[i].Add(labelsToBlocksIndexes[l.first]);
                }
                else //if (i < blocks.Count() - 1)
                {
                    graph[i].Add(i + 1);
                }
            }
        }

        public override string ToString()
        {
            const int indent = 1; // количество отступов
            var builder = new StringBuilder();
            for (int i = 0; i < blocks.Count(); i++)
            {
                builder.Append("BLOCK " + i + "\n");
                foreach (var line in blocks[i])
                {
                    if (line.label.Length > 0) builder.Append(line.label + ":");
                    builder.Append('\t', indent);

                    if (line.command == "if")
                    {
                        builder.Append("if " + line.left + " goto " + line.first + "\n");
                    }
                    else if (line.command == "goto")
                    {
                        builder.Append("goto " + line.left + "\n");
                    }
                    else if (line.command == "param")
                    {
                        builder.Append("param " + line.left + "\n");
                    }
                    else if (line.command == "call")
                    {
                        builder.Append("call " + line.left + ", " + line.second + "\n");
                    }
                    else
                    {
                        if (line.IsEmpty())
                        {
                            builder.Append("<empty statement>\n");
                        }
                        else
                        {
                            builder.Append(line.left + " = " + line.first + " ");
                            builder.Append((line.command == "" ? "" : line.command + " ") + line.second + "\n");
                        }
                    }
                }
                builder.Append("TRANSITION TO BLOCKS: ");
                foreach (int index in graph[i])
                    builder.Append(" " + index);
                builder.Append("\n\n");

                builder.Replace("  ", " ");
            }
            return builder.ToString();
        }
    }
}
