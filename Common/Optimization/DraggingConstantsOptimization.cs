using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Compiler
{

    /// <summary>
    /// Оптимизация: Протяжка констант
    /// Примечание: Использовать после разбиения на внутренние блоки
    /// 
    /// Пример применения:
    /// Compiler.DraggingConstantsOptimization dco = new Compiler.DraggingConstantsOptimization(codeGenerator.Code);
    // / dco.Optimize();
    /// Console.WriteLine("Optimization:\n" + codeGenerator.Code);
    /// 
    /// </summary>
    public class DraggingConstantsOptimization : IOptimizer
    {
        public DraggingConstantsOptimization(ThreeAddrCode code)
        {
            Code = code;
        }

        public void CheckDragging(Block block, int targetLine)
        {
            foreach (string variable in block.GetAliveVariables(targetLine))
                for (int j = targetLine - 1; j >= 0; --j)
                {
                    if (block.IsVariableAlive(variable, j)) continue;
                    if (block[j].IsNot<Line.BinaryExpr>()) continue;

                    var jLine = block[j] as Line.BinaryExpr;
                    if (jLine.IsIdentity()) // тождество, `a = b;`; b <- line.first
                    {
                        string const_value = "";
                        if (jLine.left.Equals(variable) && !block.GetAliveVariables(j).Contains(jLine.first))
                            const_value = jLine.first;
                        else
                            const_value = variable;

                        var iLine = block[targetLine] as Line.BinaryExpr;
                        if (variable.Equals(iLine.first)) iLine.first = const_value;
                        else if (variable.Equals(iLine.second)) iLine.second = const_value;
                        j = -1;
                    }
                }
        }

        public override void Optimize(params Object[] values)
        {
            foreach (Block block in Code.blocks)
            {
                block.CalculateDefUseData();
                for (int i = 0; i < block.Count; ++i)
                    if (block[i].Is<Line.BinaryExpr>())
                        CheckDragging(block, i);
            }
        }
    }
}
