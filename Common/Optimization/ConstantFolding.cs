using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public ConstantFolding(ThreeAddrCode tac = null)
        {
            Code = tac;
        }

        private static HashSet<string> ops = new HashSet<string> { "+", "-", "*", "/" };

        public override void Optimize(params Object[] values)
        {
            Debug.Assert(Code != null);
            NumberOfChanges = 0;

            FoldConstants();
            ApplyAlgebraicEqualities();
        }

        private void FoldConstants()
        {
            foreach (Block block in Code.blocks)
            {
                for (int i = 0; i < block.Count; ++i)
                {
                    if (block[i].IsNot<Line.BinaryExpr>()) continue;

                    var line = block[i] as Line.BinaryExpr;
                    if (!line.IsArithmExpr()) continue;
                    if (!line.FirstParamIsNumber() || !line.SecondParamIsNumber()) continue;

                    double x = double.Parse(line.first);
                    double y = double.Parse(line.second);
                    switch (line.operation)
                    {
                        case Operator.Minus:
                            NumberOfChanges += 1;
                            block.ReplaceLines(line, new Line.Identity(line.left, (x - y).ToString()), true);
                            break;
                        case Operator.Plus:
                            NumberOfChanges += 1;
                            block.ReplaceLines(line, new Line.Identity(line.left, (x + y).ToString()), true);
                            break;
                        case Operator.Mult:
                            NumberOfChanges += 1;
                            block.ReplaceLines(line, new Line.Identity(line.left, (x * y).ToString()), true);
                            break;
                        case Operator.Div:
                            NumberOfChanges += 1;
                            block.ReplaceLines(line, new Line.Identity(line.left, (x / y).ToString()), true);
                            break;
                    }
                }
            }
        }

        private void ApplyAlgebraicEqualities()
        {
            foreach (Block block in Code.blocks)
            {
                for (int i = 0; i < block.Count; ++i)
                {
                    if (block[i].IsNot<Line.BinaryExpr>()) continue;

                    var line = block[i] as Line.BinaryExpr;
                    if (!line.IsArithmExpr()) continue;

                    double first, second;
                    bool firstIsNumber = double.TryParse(line.first, out first);
                    bool secondIsNumber = double.TryParse(line.second, out second);
                    if (firstIsNumber && !secondIsNumber)
                    {
                        if (first == 0) //мы сравниваем double
                        {
                            if (line.operation == Operator.Plus)
                            {
                                NumberOfChanges += 1;
                                block.ReplaceLines(line, new Line.Identity(line.left, line.second), true);
                            }
                            else if (line.operation == Operator.Minus)
                            {
                                NumberOfChanges += 1;
                                block.ReplaceLines(line, new Line.Identity(line.left, "-" + second), true);
                            }
                            else if (line.operation == Operator.Mult)
                            {
                                NumberOfChanges += 1;
                                block.ReplaceLines(line, new Line.Identity(line.left, "0"), true);
                            }
                            else if (line.operation == Operator.Div)
                            {
                                NumberOfChanges += 1;
                                block.ReplaceLines(line, new Line.Identity(line.left, "0"), true);
                            }
                        }
                        else if (first == 1)
                        {
                            if (line.operation == Operator.Mult)
                            {
                                NumberOfChanges += 1;
                                block.ReplaceLines(line, new Line.Identity(line.left, line.second), true);
                            }
                        }
                    }
                    else if (!firstIsNumber && secondIsNumber)
                    {
                        if (second == 0)
                        {
                            if (line.operation == Operator.Mult)
                            {
                                NumberOfChanges += 1;
                                block.ReplaceLines(line, new Line.Identity(line.left, "0"), true);
                            }
                            else if (line.operation == Operator.Plus || line.operation == Operator.Minus)
                            {
                                NumberOfChanges += 1;
                                block.ReplaceLines(line, new Line.Identity(line.left, line.first), true);
                            }
                        }
                        else if (second == 1)
                        {
                            if (line.operation == Operator.Mult || line.operation == Operator.Div)
                            {
                                NumberOfChanges += 1;
                                block.ReplaceLines(line, new Line.Identity(line.left, line.first), true);
                            }
                        }
                    }
                }
            }
        }
    }
}
