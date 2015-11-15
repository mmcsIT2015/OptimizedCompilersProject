using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class TransferFuncFactory
    {
        public static Dictionary<Block, TransferFunction<ThreeAddrCode.Index>> TransferFuncsForReachDef(ThreeAddrCode code) 
        {
            var funcs = new Dictionary<Block, TransferFunction<ThreeAddrCode.Index>>();
            var source = code.GetGenKillInfoData();
            foreach (var block in code.blocks)
            {
                var gen = new HashSet<ThreeAddrCode.Index>();
                var kill = new HashSet<ThreeAddrCode.Index>();
                foreach (var e in source)
                {
                    foreach (var e1 in e.GetGenForBlock(code.GetBlockId(block))) gen.Add(e1);
                    foreach (var e1 in e.GetKillForBlock(code.GetBlockId(block))) kill.Add(e1);
                }

                funcs[block] = new TransferFunction<ThreeAddrCode.Index>(gen, kill);
            }

            return funcs;
        }
    }
}
