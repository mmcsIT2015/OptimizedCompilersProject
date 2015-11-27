using System;
using System.Collections.Generic;

namespace Compiler
{

    static class DomGraph
    {
        public struct BlocksPair
        {
            public Block blockBegin;
            public Block blockEnd;
            public BlocksPair(Block begin, Block end)
            {
                blockBegin = begin;
                blockEnd = end;
            }
        }
       
        public static Dictionary<Block, IEnumerable<Block>> generateDomOut(ThreeAddrCode code)
        {
            HashSet<Block> semilatticeTop = new HashSet<Block>(code.blocks);
            var semilattice = new IntersectSemilattice<Block>(semilatticeTop);

            var funcs = TransferFuncFactory.TransferFuncsForDom(code);

            var alg = new IterativeAlgo<Block, TransferFunction<Block>>(semilattice, funcs);
            alg.Run(code);

            return alg.Out;
        }

        public static IEnumerable<BlocksPair> ReverseEdges(Dictionary<Block, IEnumerable<Block>> Dom)
        {
            List<BlocksPair> listEdges = new List<BlocksPair>();
            List<Block> inputCFG = new List<Block>();

            foreach (Block block in Dom.Keys)
                inputCFG.Add(block);

            ControlFlowGraph CFG = new ControlFlowGraph(inputCFG);

            foreach (Block a in Dom.Keys)
                foreach (Block b in Dom[a])
                    if ((CFG.OutEdges(a) as List<Block>).Contains(b))
                        listEdges.Add(new BlocksPair(a, b));

            return listEdges;

        }
    }
}