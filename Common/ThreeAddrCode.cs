using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    using Label = KeyValuePair<int, int>; // хранит номер блока и номер строки в этом блоке

    class Block: List<ThreeAddrCode.Line> {
        
        private List<HashSet<string>> defUseData;        

        public override string ToString()
        {
            var builder = new StringBuilder();            

            foreach (var line in this)
            {
                if (line.label.Length > 0) builder.Append(line.label + ":");
                builder.Append('\t', 1);

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

            builder.Replace("  ", " ");

            return builder.ToString();
        }

        /// <summary>
        /// Должна быть вызвана перед любыми вызовами IsVariableAlive, GetAliveVariables.
        /// Если блок был изменен, нужно вызвать еще раз, в противном случае возвращаемые данные бессмысленны.
        /// </summary>
        public void CalculateDefUseData()
        {
            defUseData = new List<HashSet<string>>();
            HashSet<string> currentlyAlive = new HashSet<string>();

            for (int i = this.Count - 1; i >= 0; --i)
            {
                
                switch(this[i].command)
                {
                    case "goto":                        
                            break;
                    case "call":
                            break;
                    case "param":
                        {
                            if (this[i].left == "")
                                throw new ArgumentException("Critical error! Empty LEFT operand for PARAM command in " + i + " line.");
                            if (!Char.IsDigit(this[i].left[0]))
                            {
                                if (!currentlyAlive.Contains(this[i].left))
                                    currentlyAlive.Add(this[i].left);
                            }
                            break;
                        }
                                            
                    case "if":
                        {
                            if (this[i].left == "")
                                throw new ArgumentException("Critical error! Empty LEFT operand for IF command in " + i + " line.");
                            if (!Char.IsDigit(this[i].left[0]))
                            {
                                if (!currentlyAlive.Contains(this[i].left))
                                    currentlyAlive.Add(this[i].left);
                            }
                            break;
                        }
                    default:
                        {
                            if (this[i].left != "")
                            {
                                if (currentlyAlive.Contains(this[i].left))
                                    currentlyAlive.Remove(this[i].left);
                            }

                            if (this[i].first != "")
                            {
                                if (!Char.IsDigit(this[i].first[0]))
                                {
                                    if (!currentlyAlive.Contains(this[i].first))
                                        currentlyAlive.Add(this[i].first);
                                }
                            }

                            if (this[i].second != "")
                            {
                                if (!Char.IsDigit(this[i].second[0]))
                                {
                                    if (!currentlyAlive.Contains(this[i].second))
                                        currentlyAlive.Add(this[i].second);
                                }
                            }
                            break;
                        }                
                }                                                

                defUseData.Add(new HashSet<string>(currentlyAlive.Clone()));
            }

            defUseData.Reverse();
        }
        /// <summary>
        /// Проверяет, жива ли переменная на некотором шаге
        /// </summary>
        /// <param name="variable">Имя переменной</param>
        /// <param name="step">Номер шага</param>
        /// <returns>Истина, если жива</returns>
        public bool IsVariableAlive(string variable, int step)
        {
            return defUseData[step].Contains(variable);
        }

        /// <summary>
        /// Возвращает множество живых переменных для заданного шага
        /// </summary>
        /// <param name="step">Номер шага</param>
        /// <returns>HashSet, содержащий все живые переменные в строковом виде</string></returns>
        public HashSet<string> GetAliveVariables(int step)
        {
            return defUseData[step];
        }
    };

    static class Extensions
    {
        /// <summary>
        /// Deep copy for IEnumerable containers, which is not implemented in the standard library by default
        /// </summary>
        /// <typeparam name="T">Type of data within container</typeparam>
        /// <param name="containerToClone">Container to copy</param>
        /// <returns>New container, fully copied</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> containerToClone) where T : ICloneable
        {
            return containerToClone.Select(item => (T)item.Clone());
        }

    }

    class ThreeAddrCode
    {
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

        public class Line
        {
            public string label;
            public string left; // операнд из левой части выражения (присваивания)
            public string command; // команда
            public string first; // первый операнд из  правой части выражения
            public string second; // второй операнд из  правой части выражения

            // Конструктор для строк вида x =  y `op` z
            public Line(string dst, string lhs, string cmd, string rhs) 
            {
                label = "";
                left = dst;
                first = lhs;
                command = cmd;
                second = rhs;
            }

            // Конструктор для строк вида x = `op` y, т.е. левый операнд - отсутствует
            public Line(string dst, string cmd, string rhs)
            {
                label = "";
                left = dst;
                first = "";
                command = cmd;
                second = rhs;
            }

            public bool IsEmpty()
            {
                return left == "" && first == "" && command == "" && second == "";
            }

            public static Line CreateEmpty()
            {
                return new Line("", "", "");
            }
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

        public void NewBlock()
        {
            // смысла в пустых блоках нет - поэтому, если мы пытаемся добавить еще один очередной пустой, ничего не делаем
            if (blocks.Last().Count > 0)
            {
                blocks.Add(new Block());
            }
        }

        public Line AddLine(Line line)
        {
            blocks.Last().Add(line);
            return line;
        }

        public Label GetLastPosition()
        {
            return new Label(blocks.Count() - 1, blocks.Last().Count() - 1);
        }

        public Line GetLine(Label label)
        {
            return blocks[label.Key][label.Value];
        }

        public Line GetLine(int block, int line)
        {
            return blocks[block][line];
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

        public List<GenKillInfo> GetGenKillInfoData()
        {
            List<GenKillInfo> genKillInfoList = new List<GenKillInfo>();
            for(int i = 0; i < blocks.Count; i++)
            {
                genKillInfoList.Add(new GenKillInfo());
                for(int j = 0; j < blocks[i].Count; j++)
                {
                    Line line = blocks[i][j];
                    if (line.IsEmpty())
                        continue;
                    if (line.command == "param" || line.command == "goto" || line.command == "if" || line.command == "call")
                        continue;
                    Index currentInd = new Index(i, j, line.left);
                    if (genKillInfoList[i].Gen.Contains(currentInd, new Index.IndexVariableNameComparer()))
                        genKillInfoList[i].Gen.Remove(currentInd);
                    genKillInfoList[i].Gen.Add(new Index(i, j, line.left));                     
                }
            }

            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks.Count; j++)
                {
                    if (i == j)
                        continue;
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

        public List<InOutInfo> GetInOutInfoData()
        {
            List<GenKillInfo> gen_kill = GetGenKillInfoData();
            Dictionary<int, List<int>> r_graph = GetReversedGraph();

            List<InOutInfo> result = new List<InOutInfo>(blocks.Count);

            for (int i = 0; i < blocks.Count; ++i)
                result.Add(new InOutInfo());

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
            Dictionary<int, List<int>> r_graph = GetReversedGraph();

            List<InOutInfo> result = new List<InOutInfo>(blocks.Count);
            iters = 0;

            for (int i = 0; i < blocks.Count; ++i)
                result.Add(new InOutInfo());

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
