﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class InOutData<AData>
    {
        public Dictionary<Block, IEnumerable<AData>> In;
        public Dictionary<Block, IEnumerable<AData>> Out;
    }

    static class DataFlowAnalysis
    {
        // Получить достигающие определения для блоков
        public InOutData<ThreeAddrCode.Index> GetReachDefs(ThreeAddrCode code)
        {
            var semilattice = new ReachDefSemilattice(code);
            var funcs = TransferFuncFactory.TransferFuncsForReachDef(code);
            var alg = new IterativeAlgo<ThreeAddrCode.Index, TransferFunction<ThreeAddrCode.Index>>(semilattice, funcs);

            alg.Run(code);

            InOutData<ThreeAddrCode.Index> dst = new InOutData<ThreeAddrCode.Index>();
            dst.In = alg.In;
            dst.Out = alg.Out;
            return dst;
        }
    }
}
