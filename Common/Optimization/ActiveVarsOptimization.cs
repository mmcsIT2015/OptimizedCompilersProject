using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    class ActiveVarsOptimization : DeadCodeElimination
    {
        private Dictionary<Block, IEnumerable<string>> OutData;
        public ActiveVarsOptimization(ThreeAddrCode code, int blockNumber = -1) : base(code, blockNumber)
        {
            OutData = DataFlowAnalysis.GetActiveVariables(code).Out;
        }

        public override void Optimize(params Object[] values)
        {
            base.Optimize(OutData);
        }
    }
}
