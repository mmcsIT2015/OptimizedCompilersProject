using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    using ExprWrapper = ReachableExprsGenerator.ExpressionWrapper;

    public class InOutData<AData>
    {
        public Dictionary<Block, IEnumerable<AData>> In;
        public Dictionary<Block, IEnumerable<AData>> Out;

        public InOutData()
        {
            In = new Dictionary<Block, IEnumerable<AData>>();
            Out = new Dictionary<Block, IEnumerable<AData>>();
        }
            
    }

    static class DataFlowAnalysis
    {
        // Получить достигающие определения для блоков
        public static InOutData<ThreeAddrCode.Index> GetReachDefs(ThreeAddrCode code)
        {
            var semilattice = new ReachableDefSemilattice(code);
            var funcs = TransferFuncFactory.TransferFuncsForReachDef(code);
            var alg = new IterativeAlgo<ThreeAddrCode.Index, TransferFunction<ThreeAddrCode.Index>>(semilattice, funcs);

            alg.Run(code);

            InOutData<ThreeAddrCode.Index> dst = new InOutData<ThreeAddrCode.Index>();
            dst.In = alg.In;
            dst.Out = alg.Out;
            return dst;
        }

        // Анализ активных переменных
        public static InOutData<String> GetActiveVariables(ThreeAddrCode code)
        {
            var semilattice = new ActiveVariablesSemilattice(code);
            var funcs = TransferFuncFactory.TransferFuncsForActiveVariables(code);
            var alg = new IterativeAlgo<String, TransferFunction<String>>(semilattice, funcs);

            alg.RunOnReverseGraph(code);

            InOutData<String> dst = new InOutData<String>();
            dst.In = alg.In;
            dst.Out = alg.Out;
            return dst;
        }

        public static InOutData<Line.Expr> BuildReachableExpressions(ThreeAddrCode code)
        {
            var semilattice = new ReachableExprSemilattice(code);
            var funcs = ReachableExprsGenerator.BuildTransferFuncsForReachableExprs(code);
            var alg = new IterativeAlgo<ExprWrapper, TransferFunction<ExprWrapper>>(semilattice, funcs);

            alg.Run(code);

            var result = new InOutData<Line.Expr>();
            foreach (Block block in code.blocks)
            {
                result.In[block] = alg.In[block].Select(e => e.GetExpression());
                result.Out[block] = alg.Out[block].Select(e => e.GetExpression());
            }

            return result;
        }

        /// <summary>
        /// Протяжка констант
        /// </summary>
        public static InOutData<ConstNACInfo> buildConsts(ThreeAddrCode code)
        {
            var semilattice = new SemilatticeForDragingConsts();
            var funcs = TransferFuncFactory.TransferFuncsForDraggingConst(code);
            var alg = new IterativeAlgo<ConstNACInfo, TransferFunctionForDraggingConstants>(semilattice, funcs);
            alg.Run(code);

            var result = new InOutData<ConstNACInfo>();
            foreach (Block block in code.blocks)
            {
                result.In[block] = alg.In[block];
                result.Out[block] = alg.Out[block];
            }


            return result;
        }
    }
}
