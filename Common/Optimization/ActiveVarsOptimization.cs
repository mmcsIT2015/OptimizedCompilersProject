using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iCompiler
{
    public class ActiveVarsOptimization : DeadCodeElimination
    {
        private Dictionary<Block, IEnumerable<string>> InData;
        private Dictionary<Block, IEnumerable<string>> OutData;

        public ActiveVarsOptimization(ThreeAddrCode code = null)
            : base(code)
        {
            if (code != null)
            {
                var InOut = DataFlowAnalysis.GetActiveVariables(code);
                InData = InOut.In;
                OutData = InOut.Out;
            }
        }

        protected override Dictionary<string, bool> PrepareVitalityVars(Block block)
        {
            var dict = new Dictionary<string, bool>();
            foreach (var e in block)
            {
                if (e.IsNot<Line.Expr>()) continue;
                var expr = e as Line.Expr;
                if (!OutData[block].Contains(expr.left))
                {
                    dict[expr.left] = false;
                }
                // else dict[expr.left] = true; // Это нужно? Хз...
            }

            return dict;
        }

        public override void Assign(ThreeAddrCode code)
        {
            Debug.Assert(code != null);

            Code = code;
            var InOut = DataFlowAnalysis.GetActiveVariables(code);
            InData = InOut.In;
            OutData = InOut.Out;
        }
    }
}
