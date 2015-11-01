using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.New
{
    using Label = KeyValuePair<int, int>; // хранит номер блока и номер строки в этом блоке

    class ThreeAddrCode
    {
        /// <summary>
        /// Класс идентифицирует строку трехадресного кода тройкой: (номер блока, номер в блоке, имя определяемой переменной)
        /// </summary>
        public class Index
        {
            public class IndexVariableNameComparer : IEqualityComparer<Index>
            {
                public bool Equals(Index obj1, Index obj2)
                {
                    return obj1.mVariableName == obj2.mVariableName;
                }

                public int GetHashCode(Index obj)
                {
                    return obj.mVariableName.GetHashCode();
                }
            }

            public int mBlockInd { get; set; }
            public int mInternalInd { get; set; }
            public string mVariableName { get; set; }

            public Index (int blockInd, int internalInd, string variableName)
            {
                mBlockInd = blockInd;
                mInternalInd = internalInd;
                mVariableName = variableName;
            }

            public override int GetHashCode()
            {
 	            return mBlockInd * 65536 + mInternalInd;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
 	            if (obj.GetType() != this.GetType())
                    return false;
                Index o = obj as Index;
                return (this.mInternalInd == o.mInternalInd) && (this.mBlockInd == o.mBlockInd);
            }

            public override string ToString()
            {
                return "Name: " + this.mVariableName + "; Block: " + this.mBlockInd + "; Line: " + this.mInternalInd;
            }
        }

        /// <summary>
        /// Класс связан с конкретным блоком и содержит множества Gen и Kill
        /// для этого блока в виде набора объектов класса Index
        /// </summary>
        public class GenKillInfo
        {
            public HashSet<Index> Gen = new HashSet<Index>();
            public HashSet<Index> Kill = new HashSet<Index>();
        }

        public class InOutInfo
        {
            public HashSet<Index> In = new HashSet<Index>();
            public HashSet<Index> Out = new HashSet<Index>();
        }

        public Dictionary<string, Label> labels; // содержит список меток и адресом этих меток в blocks
        public List<Block> blocks; // содержит массив с блоками

        public bool Verbose { get; set; }

        //граф переходов между базовыми блоками (по индексам в массиве)
        //Пример: получить список всех дуг (переходов) из 5-ого блока
        //  List<int> d = graph[5];
        public Dictionary<int, List<int>> graph; 

        //получить обратный граф переходов между базовыми блоками
        //Пример: получить список всех дуг (переходов) в 5-ый блок
        //  List<int> d = graph[5];
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

        public ThreeAddrCode()
        {
            Verbose = false;

            blocks = new List<Block>() { new Block() };
            labels = new Dictionary<string, Label>();
            graph = null;
        }

        public ThreeAddrCode(Block lines)
        {
            Verbose = false;

            blocks = new List<Block>();
            blocks.Add(lines);

            labels = new Dictionary<string, Label>();
            graph = null;
        }

        public void NewBlock()
        {
            // смысла в пустых блоках нет - поэтому, если мы пытаемся добавить еще один очередной пустой, ничего не делаем
            if (blocks.Last().Count > 0)
            {
                blocks.Add(new Block());
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (graph == null)
            {
                foreach (var block in blocks)
                {
                    builder.Append(block.ToString());
                    builder.Append("\n\n");
                }
            }
            else
            {
                for (int i = 0; i < blocks.Count(); i++)
                {
                    if (Verbose) builder.Append("BLOCK " + i + "\n");

                    builder.Append(blocks[i].ToString());

                    if (Verbose)
                    {
                        builder.Append("TRANSITION TO BLOCKS: ");
                        foreach (int index in graph[i])
                        {
                            builder.Append(" " + index);
                        }
                        builder.Append("\n\n");
                    }
                }
            }
            return builder.ToString();
        }
    }
}
