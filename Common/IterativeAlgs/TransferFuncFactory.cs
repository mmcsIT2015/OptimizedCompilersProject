using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
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

        public static Dictionary<Block, TransferFunction<String>> TransferFuncsForActiveVariables(ThreeAddrCode code)
        {
            var funcs = new Dictionary<Block, TransferFunction<String>>();
            var source = code.GetDefUseInfo();
            foreach (var block in code.blocks)
            {
                int id = code.GetBlockId(block);
                var info = source[id];

                var def = new HashSet<String>(info.Def);
                var use = new HashSet<String>(info.Use);

                funcs[block] = new TransferFunction<String>(use, def);
            }

            return funcs;
        }

        public static Dictionary<Block, TransferFunction<Block>> TransferFuncsForDom(ThreeAddrCode code)
        {
            var funcs = new Dictionary<Block, TransferFunction<Block>>();

            var emptyKill = new HashSet<Block>();

            foreach (var block in code.blocks)
            {
                var blockFuncGen = new HashSet<Block>() { block };
                funcs[block] = new TransferFunction<Block>(blockFuncGen, emptyKill);
            }

            return funcs;
        }

        public static Dictionary<Block, TransferFunctionForDraggingConstants> TransferFuncsForDraggingConst(ThreeAddrCode code)
        {
            var funcs = new Dictionary<Block, TransferFunctionForDraggingConstants>();
            var source = code.GetConstInfo();
            for (int i = 0; i < code.blocks.Count; ++i)
                funcs[code.blocks[i]] = new TransferFunctionForDraggingConstants(source[i]);
            return funcs;
        }
    }
}
