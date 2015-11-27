using System;
using System.Collections.Generic;

namespace Compiler
{

    static class DomGraph
    {
        public static Dictionary<Block, IEnumerable<Block>> generateDomOut(ThreeAddrCode code)
        {
            HashSet<Block> semilatticeTop = new HashSet<Block>(code.blocks);
            var semilattice = new IntersectSemilattice<Block>(semilatticeTop);

            var funcs = TransferFuncFactory.TransferFuncsForDom(code);

            var alg = new IterativeAlgo<Block, TransferFunction<Block>>(semilattice, funcs);
            alg.Run(code);

            return alg.Out;
        }
    }
}