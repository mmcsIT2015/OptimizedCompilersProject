using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using iCompiler;

namespace iCompiler
{
    /// <summary>
    /// Оптимизация: Устранение общих подвыражений
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
        protected class Value
        {
            public List<string> ids = new List<string>();

            public Value() { }
            public Value(Line.Expr expr)
            {
                ids.Add(expr.left);
            }
        }

        protected class Operation : Value
        {
            public Value value1 = null, value2 = null;
            public ProgramTree.Operator op_type = ProgramTree.Operator.None;

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

        protected class UnaryOperation : Value
        {
            public Value value = null;
            public ProgramTree.Operator op_type = ProgramTree.Operator.Minus;

            public override bool Equals(object obj)
            {
                UnaryOperation op = obj as UnaryOperation;
                if (op == null)
                    return false;

                return value == op.value && op_type == op.op_type;
            }

            public override int GetHashCode()
            {
                return value.GetHashCode() * 23 + op_type.GetHashCode() * 29;
            }
        }

        public CommonSubexpressionsOptimization(ThreeAddrCode code)
        {
            Code = code;
        }

        // TODO чертовски неинформативное название. И вводящее в заблуждение.
        // Ну так метод protected, не для того, чтобы давать какую-то информацию
        protected Value GetValue(Dictionary<string, Value> dict, string id)
        {
            Value v;
            if (!dict.ContainsKey(id))
            {
                v = new Value();
                v.ids.Add(id);
                dict[id] = v;
            }
            else
            {
                v = dict[id];
            }

            return v;
        }

        protected void IterationUnaryExpr(Dictionary<string, Value> dict, Block block, int ind)
        {
            Line.UnaryExpr op = block[ind] as Line.UnaryExpr;

            if (dict.ContainsKey(op.left))
                dict[op.left].ids.Remove(op.left);

            UnaryOperation opp = new UnaryOperation();
            opp.ids.Add(op.left);
            opp.value = GetValue(dict, op.argument);
            opp.op_type = op.operation;

            bool found = false;
            foreach (Value v2 in dict.Values)
            {
                UnaryOperation opp2 = v2 as UnaryOperation;
                if (opp2 != null && opp2.Equals(opp))
                {
                    dict[op.left] = opp2;
                    opp2.ids.Add(op.left);

                    var label = block[ind].label;
                    block[ind] = new Line.Identity(op.left, opp2.ids[0]);
                    block[ind].label = label;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                dict[op.left] = opp;
                if (opp.value.ids.Count != 0) op.argument = opp.value.ids[0];
            }
        }

        protected void IterationBinaryExpr(Dictionary<string, Value> dict, Block block, int ind)
        {
            Line.BinaryExpr op = block[ind] as Line.BinaryExpr;

            if (dict.ContainsKey(op.left))
            {
                dict[op.left].ids.Remove(op.left);
            }

            Operation opp = new Operation();
            opp.ids.Add(op.left);
            opp.value1 = GetValue(dict, op.first);
            opp.value2 = GetValue(dict, op.second);
            opp.op_type = op.operation;

            bool found = false;
            foreach (Value v2 in dict.Values)
            {
                Operation opp2 = v2 as Operation;
                if (opp2 != null && opp2.Equals(opp))
                {
                    dict[op.left] = opp2;
                    opp2.ids.Add(op.left);

                    var label = block[ind].label;
                    block[ind] = new Line.Identity(op.left, opp2.ids[0]);
                    block[ind].label = label;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                dict[op.left] = opp;
                if (opp.value1.ids.Count != 0) op.first = opp.value1.ids[0];
                if (opp.value2.ids.Count != 0) op.second = opp.value2.ids[0];
            }
        }

        protected void IterationIdentity(Dictionary<string, Value> dict, Block block, int ind)
        {
            Line.Identity op = block[ind] as Line.Identity;

            if (dict.ContainsKey(op.left))
            {
                dict[op.left].ids.Remove(op.left);
            }

            Value v = GetValue(dict, op.right);
            dict[op.left] = v;
            v.ids.Add(op.left);

            op.right = v.ids[0];
        }

        protected virtual Dictionary<string, Value> PrepareDictionary(Block block)
        {
            Dictionary<string, Value> dict = new Dictionary<string, Value>();
            return dict;
        }

        public override void Optimize(params Object[] values)
        {
            foreach (Block block in Code.blocks)
            {
                var dict = PrepareDictionary(block);
                for (int i = 0; i < block.Count; ++i)
                    if (block[i].Is<Line.Identity>())
                        IterationIdentity(dict, block, i);
                    else if (block[i].Is<Line.BinaryExpr>())
                        IterationBinaryExpr(dict, block, i);
                    else if (block[i].Is<Line.UnaryExpr>())
                        IterationUnaryExpr(dict, block, i);
            }
        }
    }
}
