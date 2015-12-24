using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iCompiler
{
    /// <summary>
    /// Оптимизация: Протяжка констант
    /// 
    /// Пример применения:
    /// iCompiler.DraggingConstantsOptimization dco = new iCompiler.DraggingConstantsOptimization(codeGenerator.Code);
    // / dco.Optimize();
    /// Console.WriteLine("Optimization:\n" + codeGenerator.Code);
    /// 
    /// </summary>
    public class DraggingConstantsOptimization : IOptimizer
    {
        public DraggingConstantsOptimization(ThreeAddrCode code = null)
        {
            Code = code;
        }

        public void CheckDragging(Block block, int targetLine)
        {
            foreach (string variable in block.GetAliveVariables(targetLine))
                for (int j = targetLine - 1; j >= 0; --j)
                {
                    if (block.IsVariableAlive(variable, j)) continue;
                    if (block[j].IsNot<Line.Identity>()) continue;

                    var jLine = block[j] as Line.Identity;

                    string const_value = "";
                    if (jLine.left.Equals(variable) && !block.GetAliveVariables(j).Contains(jLine.right))
                        const_value = jLine.right;
                    else
                        continue;

                    if (block[targetLine].Is<Line.BinaryExpr>())
                    {
                        var iLine = block[targetLine] as Line.BinaryExpr;
                        if (variable.Equals(iLine.first))
                        {
                            NumberOfChanges += 1;
                            iLine.first = const_value;
                        }
                        else if (variable.Equals(iLine.second))
                        {
                            NumberOfChanges += 1;
                            iLine.second = const_value;
                        }
                        j = -1;
                    }
                    else if (block[targetLine].Is<Line.Identity>())
                    {
                        var iLine = block[targetLine] as Line.Identity;
                        if (variable.Equals(iLine.right))
                        {
                            NumberOfChanges += 1;
                            iLine.right = const_value; 
                        }
                        j = -1;
                    }
                }
        }

        public override void Optimize(params Object[] values)
        {
            Debug.Assert(Code != null);
            NumberOfChanges = 0;

            foreach (Block block in Code.blocks)
            {
                block.CalculateDefUseData();
                for (int i = 0; i < block.Count; ++i)
                     CheckDragging(block, i);
            }
        }
    }
}
