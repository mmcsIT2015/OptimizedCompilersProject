using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using iCompiler.Line;

namespace iCompiler
{
    class ConstantsPropagationOptimization : IOptimizer
    {
        public ConstantsPropagationOptimization(ThreeAddrCode code = null)
        {
            Code = code;
        }

        /// <summary>
        /// Выполняет протяжку и сворачивание констант внутри каждого блока
        /// </summary>
        private bool DragAndFoldConstants()
        {
            bool hasChanges = false;
            do
            {
                DraggingConstantsOptimization opt1 = new DraggingConstantsOptimization(Code);
                ConstantFolding opt2 = new ConstantFolding(Code);
                hasChanges = Code.makePreoptimization(opt1, opt2);
            } while (hasChanges);
            return hasChanges;
        }

        public override void Optimize(params Object[] values)
        {
            Debug.Assert(Code != null);

            //Выполняем, пока есть изменения
            bool hasChanges = DragAndFoldConstants();
            do
            {
                InOutData<ConstNACInfo> constsInfo = DataFlowAnalysis.buildConsts(Code);

                Dictionary<int, int> insertsCount = new Dictionary<int, int>();
                for (int i = 0; i < Code.blocks.Count; ++i)
                {

                    foreach (var constsList in constsInfo.In)
                    {
                        int ind = Code.GetBlockId(constsList.Key);
                        if (!insertsCount.ContainsKey(ind))
                            insertsCount.Add(ind, 0);
                        
                        //добавляем известные константы в начало блока, чтобы протянуть внутрь него.
                        foreach (ConstNACInfo cInfo in constsList.Value)
                        {
                            if (cInfo.mType == VariableConstType.CONSTANT)
                            {
                                Code.blocks[ind].Insert(0, new Identity(cInfo.VarName, cInfo.ConstVal));
                                ++insertsCount[ind];
                            }
                        }
                    }
                    
                }

                //протягиваем и сворачиваем константы, если возможно
                hasChanges |= DragAndFoldConstants();
                
                //удаляем ранее добавленные строки
                for (int i = 0; i < Code.blocks.Count; ++i)
                    Code.blocks[i].RemoveRange(0, insertsCount[i]);

            } while (hasChanges);
        }
    }
}
