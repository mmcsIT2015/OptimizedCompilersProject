using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
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
                if (obj == null) return false;
 	            if (obj.GetType() != this.GetType()) return false;

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

            BaseBlocksPartition.Partition(this);
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

        /// <summary>
        /// Функция возвращает список объектов GenKillInfo для каждого блока
        /// </summary>
        /// <returns>Список GenKillInfo</returns>
        public List<GenKillInfo> GetGenKillInfoData()
        {
            List<GenKillInfo> genKillInfoList = new List<GenKillInfo>();

            //мн-во Gen
            for (int i = 0; i < blocks.Count; i++)
            {
                genKillInfoList.Add(new GenKillInfo());
                for (int j = 0; j < blocks[i].Count; j++)
                {
                    var line = blocks[i][j];
                    if (line.IsEmpty()) continue;
                    if (line is Line.GoTo || line is Line.FunctionParam || line is Line.FunctionCall) continue;

                    var left = (line as Line.Operation).left;
                    Index currentInd = new Index(i, j, left);
                    if (genKillInfoList[i].Gen.Contains(currentInd, new Index.IndexVariableNameComparer()))
                    {
                        genKillInfoList[i].Gen.Remove(currentInd);
                    }
                    genKillInfoList[i].Gen.Add(new Index(i, j, left));
                }
            }

            // Мн-во Kill
            // Стандартное пересечение просит большее количество костылей в виде компараторов и проч.
            // Код специфический, поэтому лучше оставить как есть - все равно больше нигде не пригодится.
            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks.Count; j++)
                {
                    if (i == j) continue;
                    foreach (Index ind_i in genKillInfoList[i].Gen)
                    {
                        foreach (Index ind_j in genKillInfoList[j].Gen)
                        {
                            if (ind_i.mVariableName == ind_j.mVariableName)
                                genKillInfoList[i].Kill.Add(ind_j);
                        }
                    }

                }
            }

            return genKillInfoList;
        }

        /// <summary>
        /// Использование:
        /// ----
        /// var a = codeGenerator.Code.GetInOutInfoData();         
        /// for (int i = 0; i < a.Count; ++i)
        /// {
        ///     Console.WriteLine("Block: " + i);
        ///     
        ///     Console.WriteLine("In");
        ///     foreach (ThreeAddrCode.Index ind in a[i].In) {
        ///         Console.WriteLine(ind.ToString());
        ///     }
        ///         
        ///     Console.WriteLine("Out");
        ///     foreach (ThreeAddrCode.Index ind in a[i].Out) {
        ///         Console.WriteLine(ind.ToString());
        ///     }
        /// 
        ///     Console.WriteLine();
        /// }
        /// </summary>
        /// <returns></returns>
        public List<InOutInfo> GetInOutInfoData()
        {
            List<GenKillInfo> gen_kill = GetGenKillInfoData();
            var r_graph = GetReversedGraph();

            List<InOutInfo> result = new List<InOutInfo>(blocks.Count);
            for (int i = 0; i < blocks.Count; ++i) result.Add(new InOutInfo());

            bool changed = true;
            while (changed)
            {
                changed = false;

                for (int i = 0; i < blocks.Count; ++i)
                {
                    result[i].In.Clear();
                    foreach (int j in r_graph[i])
                        result[i].In.UnionWith(result[j].Out);

                    HashSet<Index> prev_b = null;
                    if (!changed)
                        prev_b = new HashSet<Index>(result[i].Out);

                    HashSet<Index> subs = new HashSet<Index>(result[i].In);
                    subs.ExceptWith(gen_kill[i].Kill);
                    result[i].Out = new HashSet<Index>(gen_kill[i].Gen);
                    result[i].Out.UnionWith(subs);

                    if (!changed)
                        changed = !prev_b.SetEquals(result[i].Out);
                }
            }

            return result;
        }

        /// <summary>
        /// Выполнение GetInOutInfoData() с другим порядком блоков
        /// </summary>
        /// <param name="ordering">Порядок блоков</param>
        /// <param name="iters">Количество итераций</param>
        /// <returns></returns>
        public List<InOutInfo> GetInOutInfoData(List<int> ordering, out int iters)
        {
            List<GenKillInfo> gen_kill = GetGenKillInfoData();
            var r_graph = GetReversedGraph();

            List<InOutInfo> result = new List<InOutInfo>(blocks.Count);
            for (int i = 0; i < blocks.Count; ++i) result.Add(new InOutInfo());

            iters = 0;
            bool changed = true;
            while (changed)
            {
                changed = false;

                foreach (int i in ordering)
                {
                    result[i].In.Clear();
                    foreach (int j in r_graph[i])
                        result[i].In.UnionWith(result[j].Out);

                    HashSet<Index> prev_b = null;
                    if (!changed)
                        prev_b = new HashSet<Index>(result[i].Out);

                    HashSet<Index> subs = new HashSet<Index>(result[i].In);
                    subs.ExceptWith(gen_kill[i].Kill);
                    result[i].Out = new HashSet<Index>(gen_kill[i].Gen);
                    result[i].Out.UnionWith(subs);

                    if (!changed)
                        changed = !prev_b.SetEquals(result[i].Out);
                }

                ++iters;
            }

            return result;
        }
    }
}
