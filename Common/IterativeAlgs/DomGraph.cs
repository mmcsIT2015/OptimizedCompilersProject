﻿using System;
using System.Collections.Generic;

namespace iCompiler
{

    public static class DomGraph
    {
        public struct BlocksPair<T>
        {
            public T blockBegin;
            public T blockEnd;
            public BlocksPair(T begin, T end)
            {
                blockBegin = begin;
                blockEnd = end;
            }
        }
       
        public static Dictionary<Block, IEnumerable<Block>> GenerateDomOut(ThreeAddrCode code)
        {
            HashSet<Block> semilatticeTop = new HashSet<Block>(code.blocks);
            var semilattice = new IntersectSemilattice<Block>(semilatticeTop);

            var funcs = TransferFuncFactory.TransferFuncsForDom(code);

            var alg = new IterativeAlgo<Block, TransferFunction<Block>>(semilattice, funcs);
            alg.Run(code);

            return alg.Out;
        }

        public static Dictionary<Block, List<Block>> GenerateDomTree(ThreeAddrCode code)
        {
            Dictionary<Block, List<Block>> result = new Dictionary<Block, List<Block>>();

            Dictionary<Block, IEnumerable<Block>> blockDoms = DomGraph.GenerateDomOut(code);
            Dictionary<int, List<int>> sets = new Dictionary<int, List<int>>();
            foreach (Block block in blockDoms.Keys)
            {
                List<int> set = new List<int>();

                foreach (Block domBlock in blockDoms[block])
                    set.Insert(0, code.blocks.IndexOf(domBlock));

                sets.Add(code.blocks.IndexOf(block), set);
                result.Add(block, new List<Block>());
            }

            foreach(int i in sets.Keys)
            {
                int j_prev = 0;
                foreach(int j in sets[i])
                {
                    if (j_prev != j && !result[code.blocks[j_prev]].Contains(code.blocks[j]))
                        result[code.blocks[j_prev]].Add(code.blocks[j]);
                    j_prev = j;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all reversed edges of given CFG according to Dom-Graph
        /// </summary>
        /// <param name="Dom"></param>
        /// <param name="CFG"></param>
        /// <returns></returns>
        public static IEnumerable<BlocksPair<Block>> ReverseEdges(Dictionary<Block, IEnumerable<Block>> Dom, ControlFlowGraph CFG)
        {
            List<BlocksPair<Block>> listEdges = new List<BlocksPair<Block>>();
/*            List<Block> inputCFG = new List<Block>();

            foreach (Block block in Dom.Keys)
                inputCFG.Add(block);

            ControlFlowGraph CFG = new ControlFlowGraph(inputCFG);
*/
            foreach (Block a in Dom.Keys)
                foreach (Block b in Dom[a])
                    //if a -> b
                    if ((CFG.OutEdges(a) as List<Block>).Contains(b))
                        listEdges.Add(new BlocksPair<Block>(a, b));

            return listEdges;

        }
    }
}