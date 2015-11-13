﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace Compiler
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
                    if (l.IsNot<Line.Operation>()) continue;

                    var line = l as Line.Operation;
                    if (!line.IsArithmExpr()) continue;
                    if (!line.FirstParamIsNumber() || !line.SecondParamIsNumber()) continue;

                    double x = double.Parse(line.first);
                    double y = double.Parse(line.second);
                    switch (line.operation)
                    {
                        case BinaryOperation.Minus:
                            line.ToIdentity((x - y).ToString());
                            break;
                        case BinaryOperation.Plus:
                            line.ToIdentity((x + y).ToString());
                            break;
                        case BinaryOperation.Mult:
                            line.ToIdentity((x * y).ToString());
                            break;
                        case BinaryOperation.Div:
                            line.ToIdentity((x / y).ToString());
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
                    if (l.IsNot<Line.Operation>()) continue;

                    var line = l as Line.Operation;
                    if (!line.IsArithmExpr()) continue;

                    double first, second;
                    bool firstIsNumber = double.TryParse(line.first, out first);
                    bool secondIsNumber = double.TryParse(line.second, out second);
                    if (firstIsNumber && !secondIsNumber)
                    {
                        if (first == 0) //мы сравниваем double
                        {
                            if (line.operation == BinaryOperation.Plus) line.ToIdentity(line.second);
                            else if (line.operation == BinaryOperation.Minus) line.ToIdentity(("-" + second));
                            else if (line.operation == BinaryOperation.Mult) line.ToIdentity("0");
                            else if (line.operation == BinaryOperation.Div) 
                                line.ToIdentity("0");
                        }
                        else if (first == 1) 
                        {
                            if (line.operation == BinaryOperation.Mult)
                                line.ToIdentity(line.second);
                        }
                    }
                    else if (!firstIsNumber && secondIsNumber)
                    {
                        if (second == 0) 
                        {
                            if (line.operation == BinaryOperation.Mult) line.ToIdentity("0");
                            else if (line.operation == BinaryOperation.Plus || line.operation == BinaryOperation.Minus)
                            {
                                line.ToIdentity(line.first);
                            }
                        }
                        else if (second == 1) 
                            if (line.operation == BinaryOperation.Mult || line.operation == BinaryOperation.Div)
                                line.ToIdentity(line.first);
                    }
                }
            }
        }
    }
}
