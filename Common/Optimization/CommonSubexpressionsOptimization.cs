using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using SimpleLang;

namespace SimpleLang
{
    /// <summary>
    /// Оптимизация: Устранение общих подвыражений
    /// Примечание: Использовать после разбиения на внутренние блоки
    /// Пример использования:
    /// ====
    ///     CommonSubexpressionsOptimization opt = new CommonSubexpressionsOptimization(codeGenerator.Code);
    ///     opt.Optimize();
    ///     
    ///     Console.WriteLine(codeGenerator.Code);
    ///
    /// Пример оптимизации: 
    ///	    a = b + c;
    ///	    b = a - d;
    ///	    c = b + c;
    ///	    k = 4 + 10;
    ///	    d = a - d;
    ///	    l = a - d;
    ///	    m = a - d;
    ///	=>
    ///	   	a = b + c;
    ///     b = a - d;
    ///     c = b + c;
    ///     k = 4 + 10;
    ///     d = b;
    ///     l = a - d;
    ///     m = l;
    /// </summary>
    public class CommonSubexpressionsOptimization : IOptimizer
    {
        class Value
        {
            public List<string> ids = new List<string>();
        }

        class Operation : Value
        {
            public Value value1 = null, value2 = null;
            public ProgramTree.BinaryOperation op_type = ProgramTree.BinaryOperation.None;

            public override bool Equals(object obj)
            {
                Operation op = obj as Operation;
                if (op == null)
                    return false;

                return value1 == op.value1 && value2 == op.value2 && op_type == op.op_type;
            }

            public override int GetHashCode()
            {
                return value1.GetHashCode() * 19 + value2.GetHashCode() * 17 + op_type.GetHashCode() * 13;
            }
        }

        /*
        class RightExpr
        {
            public string left, right;
            public ProgramTree.BinaryOperation command;

            public RightExpr(Line.Operation line)
            {
                left = line.first;
                right = line.second;
                command = line.operation;
            }

            public override bool Equals(object obj)
            {
                RightExpr re = obj as RightExpr;
                return re.left == left && re.right == right && re.command == command;
            }

            public override int GetHashCode()
            {
                return (left + right + command).GetHashCode();
            }

            public override string ToString()
            {
                return left + " " + command + " " + right;
            }
        }
        */

        public CommonSubexpressionsOptimization(ThreeAddrCode code)
        {
            Code = code;
        }

        /*
        private Dictionary<RightExpr, List<int>> GetListOfCommonExprs(Block block)
        {
            var expressions = new Dictionary<RightExpr, List<int>>();
            for (int i = 0; i < block.Count; ++i)
            {
                if (block[i].IsNot<Line.Operation>()) continue;

                RightExpr re = new RightExpr(block[i] as Line.Operation);
                if (!expressions.ContainsKey(re))
                {
                    expressions.Add(re, new List<int>());
                }

                expressions[re].Add(i);
            }

            // Список с действительно общими подвыражениями (которые содержатся несколько раз и т.п.
            var targets = new Dictionary<RightExpr, List<int>>();
            foreach (RightExpr re in expressions.Keys)
            {
                Debug.Assert(re.left != "" || re.right != "");
                if (expressions[re].Count > 1)
                {
                    targets.Add(re, expressions[re]);
                }
            }

            return targets;
        }
        */

        private Value GetValue(Dictionary<string, Value> dict, string id)
        {
            Value v;
            if (!dict.ContainsKey(id))
            {
                v = new Value();
                v.ids.Add(id);
                dict[id] = v;
            }
            else
                v = dict[id];
            return v;
        }

        public override void Optimize(params Object[] values)
        {
            Dictionary<string, Value> dict = new Dictionary<string, Value>();
            foreach (Block block in Code.blocks)
            {
                foreach (Line.Line line in block)
                {
                    if (line.Is<Line.Operation>())
                    {
                        Line.Operation op = line as Line.Operation;
                        if (op.IsIdentity())
                        {
                            Value v = GetValue(dict, op.first);                            

                            if (dict.ContainsKey(op.left))
                            {
                                dict[op.left].ids.Remove(op.left);
                            }
                            dict[op.left] = v;
                            v.ids.Add(op.left);
                            op.first = v.ids[0];
                        }
                        else
                        {
                            if (dict.ContainsKey(op.left))
                            {
                                dict[op.left].ids.Remove(op.left);
                            }
                            Operation opp = new Operation();
                            opp.ids.Add(op.left);
                            opp.value1 = GetValue(dict, op.first);
                            opp.value2 = GetValue(dict, op.second);
                            opp.op_type = op.operation;

                            bool b = false;
                            foreach (Value v2 in dict.Values)
                            {
                                Operation opp2 = v2 as Operation;
                                if (opp2 != null && opp2.Equals(opp))
                                {
                                    dict[op.left] = opp2;
                                    opp2.ids.Add(op.left);

                                    op.first = opp2.ids[0];
                                    op.operation = ProgramTree.BinaryOperation.None;
                                    op.second = "";
                                    b = true;
                                    break;
                                }
                            }
                            if (!b)
                            { 
                                dict[op.left] = opp;
                                if (opp.value1.ids.Count != 0)
                                    op.first = opp.value1.ids[0];
                                if (opp.value2.ids.Count != 0)
                                    op.second = opp.value2.ids[0];
                            }
                        }

                        

                    }
                }
            }

            /*
            bool wo = true;

            while (wo)
            {
                wo = false;

                // TODO Сделать вложенность циклов поменьше...
                foreach (Block block in Code.blocks)
                {
                    var targets = GetListOfCommonExprs(block);
                    foreach (RightExpr re in targets.Keys)
                    {
                        List<int> lines = targets[re];
                        for (int i = 0; i < lines.Count - 1; ++i)
                        {
                            bool cbo = true;
                            for (int j = lines[i]; j < lines[i + 1]; ++j)
                            {
                                if (block[j].IsNot<Line.Operation>()) continue;

                                var line = block[j] as Line.Operation;
                                if (line.left == re.left || line.left == re.right)
                                {
                                    cbo = false;
                                    break;
                                }
                            }

                            if (cbo)
                            {
                                wo = true;

                                var replace = (block[lines[i]] as Line.Operation).left;
                                var line = block[lines[i + 1]] as Line.Operation;
                                block[lines[i + 1]] = new Line.Operation(line.left, replace);

                                lines.RemoveAt(i + 1);
                                --i;
                            }
                        }
                    }
                }
            }
            */
        }
    }
}
