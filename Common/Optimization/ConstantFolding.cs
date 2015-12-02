using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace iCompiler
{

    /// <summary>
    /// Оптимизация: Свертка констант и применение алгебраических тождеств
    ///                  
    /// Пример: 
    /// ConstantFolding cf = new ConstantFolding(codeGenerator.Code);
    /// cf.Optimize();
    ///                    
    /// Console.WriteLine(codeGenerator.Code);
    ///
    /// </summary>
    public class ConstantFolding : IOptimizer
    {
        public ConstantFolding(ThreeAddrCode tac)
        {
            Code = tac;
        }

        private static HashSet<string> ops = new HashSet<string> { "+", "-", "*", "/" };

        public override void Optimize(params Object[] values)
        {
            FoldConstants();
            ApplyAlgebraicEqualities();
        }

        private void FoldConstants()
        {
            foreach (Block block in Code.blocks)
            {
                foreach (var l in block)
                {
                    if (l.IsNot<Line.BinaryExpr>()) continue;

                    var line = l as Line.BinaryExpr;
                    if (!line.IsArithmExpr()) continue;
                    if (!line.FirstParamIsNumber() || !line.SecondParamIsNumber()) continue;

                    double x = double.Parse(line.first);
                    double y = double.Parse(line.second);
                    switch (line.operation)
                    {
                        case BinaryOperation.Minus:
                            block.ReplaceLines(line, new Line.Identity(line.left, (x - y).ToString()));
                            break;
                        case BinaryOperation.Plus:
                            block.ReplaceLines(line, new Line.Identity(line.left, (x + y).ToString()));
                            break;
                        case BinaryOperation.Mult:
                            block.ReplaceLines(line, new Line.Identity(line.left, (x * y).ToString()));
                            break;
                        case BinaryOperation.Div:
                            block.ReplaceLines(line, new Line.Identity(line.left, (x / y).ToString()));
                            break;
                    }
                }
            }
        }

        private void ApplyAlgebraicEqualities()
        {
            foreach (Block block in Code.blocks)
            {
                foreach (var l in block)
                {
                    if (l.IsNot<Line.BinaryExpr>()) continue;

                    var line = l as Line.BinaryExpr;
                    if (!line.IsArithmExpr()) continue;

                    double first, second;
                    bool firstIsNumber = double.TryParse(line.first, out first);
                    bool secondIsNumber = double.TryParse(line.second, out second);
                    if (firstIsNumber && !secondIsNumber)
                    {
                        if (first == 0) //мы сравниваем double
                        {
                            if (line.operation == BinaryOperation.Plus)
                            {
                                block.ReplaceLines(line, new Line.Identity(line.left, line.second));
                            }
                            else if (line.operation == BinaryOperation.Minus)
                            {
                                block.ReplaceLines(line, new Line.Identity(line.left, "-" + second));
                            }
                            else if (line.operation == BinaryOperation.Mult)
                            {
                                block.ReplaceLines(line, new Line.Identity(line.left, "0"));
                            }
                            else if (line.operation == BinaryOperation.Div)
                                block.ReplaceLines(line, new Line.Identity(line.left, "0"));
                        }
                        else if (first == 1) 
                        {
                            if (line.operation == BinaryOperation.Mult)
                                block.ReplaceLines(line, new Line.Identity(line.left, line.second));
                        }
                    }
                    else if (!firstIsNumber && secondIsNumber)
                    {
                        if (second == 0) 
                        {
                            if (line.operation == BinaryOperation.Mult)
                            {
                                block.ReplaceLines(line, new Line.Identity(line.left, "0"));
                            }
                            else if (line.operation == BinaryOperation.Plus || line.operation == BinaryOperation.Minus)
                            {
                                block.ReplaceLines(line, new Line.Identity(line.left, line.first));
                            }
                        }
                        else if (second == 1)
                        {
                            if (line.operation == BinaryOperation.Mult || line.operation == BinaryOperation.Div)
                            {
                                block.ReplaceLines(line, new Line.Identity(line.left, line.first));
                            }
                        }
                    }
                }
            }
        }
    }
}
