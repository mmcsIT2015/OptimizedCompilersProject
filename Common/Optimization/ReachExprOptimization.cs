using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iCompiler
{
    /// <summary>
    /// Пример оптимизации
    /// Вход.
    /// ---
    ///     a = b + c
    ///     if 1 goto @l0
    ///
    ///     goto @l1
    ///
    ///@l0: d = b + c
    ///     e = b + c
    ///     h = b + c
    /// ---
    /// 1. CommonSubexpressionsOptimization:
    ///     a = b + c
    ///     if 1 goto @l0
    ///
    ///     goto @l1
    ///
    ///@l0: d = b + c
    ///     e = d
    ///     h = d
    /// ---
    /// 2. ReachExprOptimization
    ///     a = b + c
    ///     if 1 goto @l0
    ///
    ///     goto @l1
    ///
    ///@l0: d = a
    ///     e = a
    ///     h = a
    /// </summary>

    public class ReachExprOptimization : CommonSubexpressionsOptimization
    {
        protected InOutData<Line.Expr> InOut;

        public ReachExprOptimization(ThreeAddrCode code = null) :
            base(code)
        {
            Code = code;
            if (code != null)
            {
                InOut = DataFlowAnalysis.BuildReachableExpressions(Code);
            }
        }

        protected void CreateValue(Dictionary<string, Value> dict, Line.Expr expr) {
            if (expr.Is<Line.BinaryExpr>())
            {
                var t1 = GetValue(dict, (expr as Line.BinaryExpr).first);
                var t2 = GetValue(dict, (expr as Line.BinaryExpr).second);

                var op = new Operation();
                op.op_type = (expr as Line.BinaryExpr).operation;
                op.ids.Add(expr.left);
                op.value1 = t1;
                op.value2 = t2;
                dict[expr.left] = op;
            }
            else if (expr.Is<Line.UnaryExpr>())
            {
                var t = GetValue(dict, (expr as Line.UnaryExpr).argument);

                var op = new UnaryOperation();
                op.op_type = (expr as Line.UnaryExpr).operation;
                op.ids.Add(expr.left);
                op.value = t;
                dict[expr.left] = op;
            }
            else // expr.Is<Line.Identity>()
            {
                var op = GetValue(dict, (expr as Line.Identity).right);
                dict[expr.left] = op;
            }
        }

        protected override Dictionary<string, Value> PrepareDictionary(Block block)
        {
            var dict = new Dictionary<string, Value>();
            /* заполним словать данными из In */
            foreach (var e in InOut.In[block])
            {
                CreateValue(dict, e);
            }

            return dict;
        }

        public override void Assign(ThreeAddrCode code)
        {
            Debug.Assert(code != null);

            Code = code;
            InOut = DataFlowAnalysis.BuildReachableExpressions(Code);
        }
    }
}
