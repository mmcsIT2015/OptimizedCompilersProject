using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;
using iCompiler.Line;

namespace iCompiler
{
    using Label = KeyValuePair<int, int>; // хранит номер блока и номер строки в этом блоке

    public class ThreeAddrCode
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
                    return obj1.VariableName == obj2.VariableName;
                }

                public int GetHashCode(Index obj)
                {
                    return obj.VariableName.GetHashCode();
                }
            }

            public int BlockIndex { get; set; }
            public int InternalIndex { get; set; }
            public string VariableName { get; set; }

            public Index (int blockInd, int internalInd, string variableName)
            {
                BlockIndex = blockInd;
                InternalIndex = internalInd;
                VariableName = variableName;
            }

            public override int GetHashCode()
            {
                return BlockIndex * 65536 + InternalIndex;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
 	            if (obj.GetType() != this.GetType()) return false;

                Index o = obj as Index;
                return (this.InternalIndex == o.InternalIndex) && (this.BlockIndex == o.BlockIndex);
            }

            public override string ToString()
            {
                //return "Name: " + this.VariableName + "; Block: " + this.BlockIndex + "; Line: " + this.InternalIndex;
                return "[" + this.VariableName + "] b: " + this.BlockIndex + ", l: " + this.InternalIndex;
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

        public class InOutInfo<T>
        {
            public HashSet<T> In = new HashSet<T>();
            public HashSet<T> Out = new HashSet<T>();
        }

        public class DefUseInfo
        {
            public HashSet<String> Def = new HashSet<String>();
            public HashSet<String> Use = new HashSet<String>();
        }

        public Dictionary<string, Label> labels; // содержит список меток и адресом этих меток в blocks
        public List<Block> blocks; // содержит массив с блоками

        public ControlFlowGraph graph; // граф потока управления
        public Dictionary<string, SimpleVarType> tableOfNames; // таблица с типами переменных

        public ThreeAddrCode()
        {
            blocks = new List<Block>() { new Block() };
            labels = new Dictionary<string, Label>();
            
            graph = null;
            tableOfNames = null;
        }

        public ThreeAddrCode(Block lines)
        {
            blocks = new List<Block>();
            blocks.Add(lines);

            labels = new Dictionary<string, Label>();

            BaseBlocksPartition.Partition(this);
            graph = new ControlFlowGraph(this.blocks);
            tableOfNames = null;
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
            foreach (var block in blocks)
            {
                //builder.Append("block " + GetBlockId(block) + ":\n");
                builder.Append(block.ToString());
                builder.Append("\n");
            }
            return builder.ToString();
        }

        public string TableOfNamesToString() {
            var builder = new StringBuilder();
            foreach (var record in tableOfNames)
            {
                builder.Append(record.Value);
                builder.Append('\t', 1);
                builder.Append(record.Key);
                builder.Append("\n");
            }

            return builder.ToString();
        }

        public int GetBlockId(Block block)
        {
            return blocks.IndexOf(block);
        }

        public int GetLineId(Line.Line line)
        {
            int counter = 0;
            foreach (var block in blocks)
            {
                for (int j = 0; j < block.Count(); ++j)
                {
                    if (line == block[j]) return counter;
                    ++counter;
                }
            }

            return counter;
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

                    var left = (line as Line.Expr).left;
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
                            if (ind_i.VariableName == ind_j.VariableName)
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
        public List<InOutInfo<Index>> GetInOutInfoData()
        {
            List<GenKillInfo> gen_kill = GetGenKillInfoData();

            List<InOutInfo<Index>> result = new List<InOutInfo<Index>>(blocks.Count);
            for (int i = 0; i < blocks.Count; ++i) result.Add(new InOutInfo<Index>());

            bool changed = true;
            while (changed)
            {
                changed = false;

                for (int i = 0; i < blocks.Count; ++i)
                {
                    result[i].In.Clear();
                    foreach (var block in graph.OutEdges(blocks[i])) // для обратного графа это входные дуги
                    {
                        result[i].In.UnionWith(result[GetBlockId(block)].Out);
                    }

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
        public List<InOutInfo<Index>> GetInOutInfoData(List<int> ordering, out int iters)
        {
            List<GenKillInfo> gen_kill = GetGenKillInfoData();

            List<InOutInfo<Index>> result = new List<InOutInfo<Index>>(blocks.Count);
            for (int i = 0; i < blocks.Count; ++i) result.Add(new InOutInfo<Index>());

            iters = 0;
            bool changed = true;
            while (changed)
            {
                changed = false;

                foreach (int i in ordering)
                {
                    result[i].In.Clear();
                    foreach (var block in graph.OutEdges(blocks[i])) // для обратного графа это входные дуги
                    {
                        result[i].In.UnionWith(result[GetBlockId(block)].Out);
                    }

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

        /// <summary>
        /// Функция возвращает список объектов DefUseInfo для каждого блока
        /// </summary>
        /// <returns>Список DefUseInfo</returns>
        public List<DefUseInfo> GetDefUseInfo()
        {
            List<DefUseInfo> defUseInfoList = new List<DefUseInfo>();

            for (int i = 0; i < blocks.Count; i++)
            {
                defUseInfoList.Add(new DefUseInfo());
                for (int j = 0; j < blocks[i].Count; j++)
                {

                    //DEF
                    var line = blocks[i][j];
                    if (line.IsEmpty()) continue;
                    if (line is Line.GoTo || line is Line.FunctionParam || line is Line.FunctionCall) continue;

                    String currentIndDef = (line as Line.Expr).left;

                    if (defUseInfoList[i].Def.Contains(currentIndDef))
                        defUseInfoList[i].Def.Remove(currentIndDef);

                    defUseInfoList[i].Def.Add(currentIndDef);

                    //USE
                    String usedVar = "";
                    if (line is Line.BinaryExpr && !(line as Line.BinaryExpr).FirstParamIsNumber())
                    {
                        usedVar = (line as Line.BinaryExpr).first;
                    }
                    else if (line is Line.UnaryExpr && !(line as Line.UnaryExpr).ArgIsNumber())
                    {
                        usedVar = (line as Line.UnaryExpr).argument;
                    }
                    else if (line is Line.Identity && !(line as Line.Identity).RightIsNumber())
                    {
                        usedVar = (line as Line.Identity).right;
                    }

                    if (usedVar.Length > 0)
                    {
                        if (defUseInfoList[i].Use.Contains(usedVar))
                        {
                            defUseInfoList[i].Use.Remove(usedVar);
                        }

                        defUseInfoList[i].Use.Add(usedVar);
                    }

                    if (line is Line.BinaryExpr) // второй операнд есть только у BinaryExpr
                    {
                        var expr = (line as Line.BinaryExpr);
                        if (!expr.SecondParamIsNumber() && !"".Equals(expr.second))
                        {
                            usedVar = expr.second;

                            if (defUseInfoList[i].Use.Contains(usedVar))
                                defUseInfoList[i].Use.Remove(usedVar);

                            defUseInfoList[i].Use.Add(usedVar);
                        }
                    }

                }
            }

            return defUseInfoList;
        }

        public List<InOutInfo<String>> GetAliveVariablesIterationAlgo()
        {

            List<DefUseInfo> def_use = GetDefUseInfo();

            List<InOutInfo<String>> result = new List<InOutInfo<String>>(blocks.Count);
            for (int i = 0; i < blocks.Count; ++i) result.Add(new InOutInfo<String>());

            bool changed = true;
            while (changed)
            {
                changed = false;

                for (int i = 0; i < blocks.Count; ++i)
                {
                    result[i].In.Clear();
                    foreach (var block in graph.OutEdges(blocks[i])) // для обратного графа это входные дуги
                    {
                        result[i].In.UnionWith(result[GetBlockId(block)].Out);
                    }

                    HashSet<String> prev_b = null;
                    if (!changed)
                        prev_b = new HashSet<String>(result[i].Out);

                    HashSet<String> subs = new HashSet<String>(result[i].In);
                    subs.ExceptWith(def_use[i].Def);
                    result[i].Out = new HashSet<String>(def_use[i].Use);
                    result[i].Out.UnionWith(subs);

                    if (!changed)
                        changed = !prev_b.SetEquals(result[i].Out);
                }
            }

            return result;
        }


        //Использование
        //var code = codeGenerator.CreateCode();
        //Dictionary<String, String> exprs = code.RollExpressionsToNormalForm();
        
        /// <summary>
        /// Функция возвращает словарь, в котором каждой встреченной переменной ставится в соответствие её развернутый вид
        /// </summary>
        /// <returns>Словарь развернутых переменных</returns>
        public Dictionary<String,String> RollExpressionsToNormalForm()
        {
            Dictionary<String, String> expressions = new Dictionary<String, String>();

            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks[i].Count; j++)
                {
                    var line = blocks[i][j];
                    if (line.IsEmpty()) continue;
                    if (line is Line.GoTo || line is Line.FunctionCall) continue;

                    String defOfExpr = (line as Line.Expr).left;

                    String firstParamOfRightPart = "";
                    String secondParamOfRightPart = "";
                    String rightPartOfExpr = "";

                    if (line is Line.BinaryExpr)
                    {
                        firstParamOfRightPart = expressions.ContainsKey((line as Line.BinaryExpr).first) ?
                                                       expressions[(line as Line.BinaryExpr).first] :
                                                       (line as Line.BinaryExpr).first;

                        secondParamOfRightPart = expressions.ContainsKey((line as Line.BinaryExpr).second) ?
                                                        expressions[(line as Line.BinaryExpr).second] :
                                                        (line as Line.BinaryExpr).second;

                        Operator op = (line as Line.BinaryExpr).operation;

                        rightPartOfExpr = firstParamOfRightPart +
                                          (line as Line.BinaryExpr).operation +
                                          secondParamOfRightPart;


                        if (op.Equals(Operator.Minus) || op.Equals(Operator.Plus))
                            rightPartOfExpr = "(" + rightPartOfExpr + ")";
                    }
                    else if (line is Line.Identity)
                    {
                        rightPartOfExpr = expressions.ContainsKey((line as Line.Identity).right) ?
                                                       expressions[(line as Line.Identity).right] :
                                                       (line as Line.Identity).right;
                    }
                    else if (line is Line.UnaryExpr)
                    {
                        rightPartOfExpr = expressions.ContainsKey((line as Line.UnaryExpr).argument) ?
                                                       expressions[(line as Line.UnaryExpr).argument] :
                                                       (line as Line.UnaryExpr).argument;

                        rightPartOfExpr = "(" + (line as Line.UnaryExpr).operation + rightPartOfExpr + ")";
                        
                    }
                 
                    if (expressions.ContainsKey(defOfExpr))                 
                        expressions.Remove(defOfExpr);

                    expressions.Add(defOfExpr, rightPartOfExpr);
                                        
                }
            }

            return expressions;
        }
    }
}

