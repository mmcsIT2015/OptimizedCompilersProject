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

        public class GenKill<T>
        {
            public HashSet<T> Gen = new HashSet<T>();
            public HashSet<T> Kill = new HashSet<T>();
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
            for (int i = 0; i<blocks.Count(); ++i) {
                if (block == blocks[i]) return i;
            }

            Debug.Assert(false);
            return -1;
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

        private class ExpressionsGenKillComparer : IEqualityComparer<Line.Line>
        {
            public bool Equals(Line.Line obj1, Line.Line obj2)
            {
                if (obj1.Is<Line.BinaryExpr>() && obj2.Is<Line.BinaryExpr>())
                {
                    Line.BinaryExpr expr1 = obj1 as Line.BinaryExpr;
                    Line.BinaryExpr expr2 = obj2 as Line.BinaryExpr;

                    return expr1.first == expr2.first && expr1.operation == expr2.operation
                        && expr1.second == expr2.second;
                }
                else if (obj1.Is<Line.UnaryExpr>() && obj2.Is<Line.UnaryExpr>())
                {
                    Line.UnaryExpr expr1 = obj1 as Line.UnaryExpr;
                    Line.UnaryExpr expr2 = obj2 as Line.UnaryExpr;

                    return expr1.argument == expr2.argument && expr1.operation == expr2.operation;
                }
                else if (obj1.Is<Line.Identity>() && obj2.Is<Line.Identity>())
                {
                    Line.Identity expr1 = obj1 as Line.Identity;
                    Line.Identity expr2 = obj2 as Line.Identity;

                    return expr1.right == expr2.right;
                }

                return false;
            }

            public int GetHashCode(Line.Line obj)
            {
                return obj.GetHashCode();
            }
        }

        public List<GenKill<Line.Line>> buildExprGenKillInfo()
        {
            ExpressionsGenKillComparer regeneratedExpressionFilter = new ExpressionsGenKillComparer();

            List<GenKill<Line.Line>> exprGenKillList = new List<GenKill<Line.Line>>();

            for (int i = 0; i < blocks.Count; ++i)
            {
                GenKill<Line.Line> blockGenKill = new GenKill<Line.Line>();
                exprGenKillList.Add(blockGenKill);

                HashSet<String> redifined = new HashSet<String>();

                for (int j = blocks[i].Count - 1; j >= 0; --j)
                {
                    if (blocks[i][j].Is<Line.BinaryExpr>())
                    {
                        Line.BinaryExpr expr = blocks[i][j] as Line.BinaryExpr;

                        redifined.Add(expr.left);

                        if (!redifined.Contains(expr.first) && !redifined.Contains(expr.second))
                        {
                            blockGenKill.Gen.Add(blocks[i][j]);
                        }
                    }
                    else if (blocks[i][j].Is<Line.UnaryExpr>())
                    {
                        Line.UnaryExpr expr = blocks[i][j] as Line.UnaryExpr;

                        redifined.Add(expr.left);

                        if (!redifined.Contains(expr.argument))
                        {
                            blockGenKill.Gen.Add(blocks[i][j]);
                        }
                    }
                    else if (blocks[i][j].Is<Line.Identity>())
                    {
                        Line.Identity expr = blocks[i][j] as Line.Identity;

                        redifined.Add(expr.left);

                        if (!redifined.Contains(expr.right))
                        {
                            blockGenKill.Gen.Add(blocks[i][j]);
                        }
                    }
                }

                for (int k = 0; k < blocks.Count; ++k)
                {
                    if (k == i)
                    {
                        continue;
                    }

                    for (int j = 0; j < blocks[k].Count; ++j)
                    {
                        if (blocks[k][j].Is<Line.BinaryExpr>())
                        {
                            Line.BinaryExpr expr = blocks[k][j] as Line.BinaryExpr;

                            if ((redifined.Contains(expr.first) || redifined.Contains(expr.second))
                                && !blockGenKill.Gen.Contains(expr, regeneratedExpressionFilter))
                            {
                                blockGenKill.Kill.Add(expr);
                            }
                        }
                        else if (blocks[k][j].Is<Line.UnaryExpr>())
                        {
                            Line.UnaryExpr expr = blocks[k][j] as Line.UnaryExpr;

                            if (redifined.Contains(expr.argument)
                                && !blockGenKill.Gen.Contains(expr, regeneratedExpressionFilter))
                            {
                                blockGenKill.Kill.Add(expr);
                            }
                        }
                        else if (blocks[k][j].Is<Line.Identity>())
                        {
                            Line.Identity expr = blocks[k][j] as Line.Identity;

                            if (redifined.Contains(expr.right)
                                && !blockGenKill.Gen.Contains(expr, regeneratedExpressionFilter))
                            {
                                blockGenKill.Kill.Add(expr);
                            }
                        }
                    }
                }
            }
            
            return exprGenKillList;
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
                    else if (line is Line.UnaryExpr && !(line as Line.UnaryExpr).ParamIsNumber())
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
    }
}

