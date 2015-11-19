﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Compiler;

namespace Compiler
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

        class UnaryOperation : Value
        {
            public Value value = null;
            public ProgramTree.UnaryOperation op_type = ProgramTree.UnaryOperation.Minus;

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

        private void Iteration(Dictionary<string, Value> dict, Line.BinaryExpr op)
        {
            if (dict.ContainsKey(op.left))
                dict[op.left].ids.Remove(op.left);

            if (op.IsIdentity())
            {
                Value v = GetValue(dict, op.first);
                dict[op.left] = v;
                v.ids.Add(op.left);

                op.first = v.ids[0];
            }
            else
            {
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

        private void Iteration(Dictionary<string, Value> dict, Block block, int ind)
        {
            Line.UnaryExpr op = block[ind] as Line.UnaryExpr;

            if (dict.ContainsKey(op.left))
                dict[op.left].ids.Remove(op.left);

            UnaryOperation opp = new UnaryOperation();
            opp.ids.Add(op.left);
            opp.value = GetValue(dict, op.argument);
            opp.op_type = op.operation;

            bool b = false;
            foreach (Value v2 in dict.Values)
            {
                UnaryOperation opp2 = v2 as UnaryOperation;
                if (opp2 != null && opp2.Equals(opp))
                {
                    dict[op.left] = opp2;
                    opp2.ids.Add(op.left);

                    block[ind] = new Line.BinaryExpr(op.left, opp2.ids[0]);
                    b = true;
                    break;
                }
            }

            if (!b)
            {
                dict[op.left] = opp;
                if (opp.value.ids.Count != 0)
                    op.argument = opp.value.ids[0];
            }
        }

        public override void Optimize(params Object[] values)
        {
            foreach (Block block in Code.blocks)
            {
                Dictionary<string, Value> dict = new Dictionary<string, Value>();
                for (int i = 0; i < block.Count; ++i)
                    if (block[i].Is<Line.BinaryExpr>())
                        Iteration(dict, block[i] as Line.BinaryExpr);
                    else if (block[i].Is<Line.UnaryExpr>())
                        Iteration(dict, block, i);
            }
        }
    }
}
