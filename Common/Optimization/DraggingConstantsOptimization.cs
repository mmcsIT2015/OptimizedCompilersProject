using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace SimpleLang.Optimization
{
    //Пример применения:
    //SimpleLang.Optimization.DraggingConstantsOptimization dco = new SimpleLang.Optimization.DraggingConstantsOptimization(codeGenerator.Code);
    //dco.Optimize();
    //Console.WriteLine("Optimization:\n" + codeGenerator.Code);

    /// <summary>
    /// Оптимизация: Протяжка констант
    /// Примечание: Использовать после разбиения на внутренние блоки
    /// </summary>
    class DraggingConstantsOptimization : IOptimizer
    {
        private ThreeAddrCode code;

        public DraggingConstantsOptimization(ThreeAddrCode code)
        {
            this.code = code;
        }

        public void Optimize(params Object[] values)
        {
            foreach(Block b in code.blocks)
            {
                b.CalculateDefUseData();
                for(int i = 1; i < b.Count; ++i)
                {
                    foreach (string variable in b.GetAliveVariables(i))
                    {
                        for(int j = i - 1; j >= 0; --j)
                        {
                            if(!b.IsVariableAlive(variable, j))
                            {
                                if (b[j].command.Equals("") && b[j].first.Equals(""))
                                {
                                    string const_value = "";
                                    if (b[j].left.Equals(variable) && !b.GetAliveVariables(j).Contains(b[j].second))
                                        const_value = b[j].second;
                                    else
                                        const_value = variable;

                                    if (variable.Equals(b[i].first))
                                        b[i].first = const_value;
                                    else if (variable.Equals(b[i].second))
                                        b[i].second = const_value;
                                    j = -1;
                                }
                            }
                        }
                    }
                }

            }
        }

        public ThreeAddrCode Code { get { return code; } }
    }
}
