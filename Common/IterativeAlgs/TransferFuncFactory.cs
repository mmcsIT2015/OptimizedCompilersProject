﻿using System;
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
                int id = code.GetBlockId(block);
                var info = source[id];

                var gen = new HashSet<ThreeAddrCode.Index>(info.Gen);
                var kill = new HashSet<ThreeAddrCode.Index>(info.Kill);

                funcs[block] = new TransferFunction<ThreeAddrCode.Index>(gen, kill);
            }

            return funcs;
        }
    }
}
