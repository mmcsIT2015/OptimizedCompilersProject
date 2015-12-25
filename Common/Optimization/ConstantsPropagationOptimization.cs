using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using iCompiler.Line;

namespace iCompiler
{
    public class ConstantsPropagationOptimization : IOptimizer
    {
        public ConstantsPropagationOptimization(ThreeAddrCode code = null)
        {
            Code = code;
        }

        /// <summary>
        /// Выполняет протяжку и сворачивание констант внутри каждого блока
        /// </summary>
        private int DragAndFoldConstants()
        {
            var drag = new DraggingConstantsOptimization(Code);
            drag.Optimize();

            var fold = new ConstantFolding(Code);
            fold.Optimize();

            return drag.NumberOfChanges + fold.NumberOfChanges;
        }

        public override void Optimize(params Object[] values)
        {
            Debug.Assert(Code != null);

            //Выполняем, пока есть изменения
            do
            {
                NumberOfChanges = 0;
                InOutData<ConstNACInfo> constsInfo = DataFlowAnalysis.buildConsts(Code);

                Dictionary<int, int> insertsCount = new Dictionary<int, int>();
                foreach (var constsList in constsInfo.In)
                {
                    int ind = Code.GetBlockId(constsList.Key);
                    if (!insertsCount.ContainsKey(ind))
                    {
                        insertsCount.Add(ind, 0);
                    }

                    //добавляем известные константы в начало блока, чтобы протянуть внутрь него.
                    foreach (ConstNACInfo cInfo in constsList.Value)
                    {
                        if (cInfo.VarType == VariableConstType.CONSTANT)
                        {
                            Code.blocks[ind].Insert(0, new Identity(cInfo.VarName, cInfo.ConstVal));
                            ++insertsCount[ind];
                        }
                    }
                }

                //протягиваем и сворачиваем константы, если возможно
                NumberOfChanges = DragAndFoldConstants();

                //удаляем ранее добавленные строки
                for (int i = 0; i < Code.blocks.Count; ++i)
                {
                    Code.blocks[i].RemoveRange(0, insertsCount[i]);
                }
            } while (NumberOfChanges > 0);
        }
    }
}
