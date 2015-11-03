using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using SimpleLang;

namespace SimpleLang
{
    /// <summary>
    /// Оптимизация: Устранение общих подвыражений
    /// Примечание: Использовать после разбиения на внутренние блоки
    /// Пример использования:
    /// ====
    ///     CommonSubexpressionsOptimization opt = new CommonSubexpressionsOptimization(codeGenerator.Code);
    ///     opt.Optimize();
    ///     
    ///     Console.WriteLine(codeGenerator.Code);
    ///
    /// Пример оптимизации: 
    ///	    a = b + c;
    ///	    b = a - d;
    ///	    c = b + c;
    ///	    k = 4 + 10;
    ///	    d = a - d;
    ///	    l = a - d;
    ///	    m = a - d;
    ///	=>
    ///	   	a = b + c;
    ///     b = a - d;
    ///     c = b + c;
    ///     k = 4 + 10;
    ///     d = b;
    ///     l = a - d;
    ///     m = l;
    /// </summary>
    class CommonSubexpressionsOptimization : IOptimizer
    {
        class RightExpr
        {
            public string left, right;
            public ProgramTree.BinaryOperation command;

            public RightExpr(Line.Operation line)
            {
                left = line.first;
                right = line.second;
                command = line.operation;
            }

            public override bool Equals(object obj)
            {
                RightExpr re = obj as RightExpr;
                return re.left == left && re.right == right && re.command == command;
            }

            public override int GetHashCode()
            {
                return (left + right + command).GetHashCode();
            }

            public override string ToString()
            {
                return left + " " + command + " " + right;
            }
        }

        public CommonSubexpressionsOptimization(ThreeAddrCode code)
        {
            Code = code;
        }

        private Dictionary<RightExpr, List<int>> GetListOfCommonExprs(Block block)
        {
            var expressions = new Dictionary<RightExpr, List<int>>();
            for (int i = 0; i < block.Count; ++i)
            {
                if (block[i].Is<Line.Operation>()) continue;

                RightExpr re = new RightExpr(block[i] as Line.Operation);
                if (!expressions.ContainsKey(re))
                {
                    expressions.Add(re, new List<int>());
                }

                expressions[re].Add(i);
            }

            // Список с действительно общими подвыражениями (которые содержатся несколько раз и т.п.
            var targets = new Dictionary<RightExpr, List<int>>();
            foreach (RightExpr re in expressions.Keys)
            {
                Debug.Assert(re.left != "" || re.right != "");
                if (expressions[re].Count > 1)
                {
                    targets.Add(re, expressions[re]);
                }
            }

            return targets;
        }

        public override void Optimize(params Object[] values)
        {
            bool wo = true;

            while (wo)
            {
                wo = false;

                // TODO Сделать вложенность циклов поменьше...
                foreach (Block block in Code.blocks)
                {
                    var targets = GetListOfCommonExprs(block);
                    foreach (RightExpr re in targets.Keys)
                    {
                        List<int> lines = targets[re];
                        for (int i = 0; i < lines.Count - 1; ++i)
                        {
                            bool cbo = true;
                            for (int j = lines[i]; j < lines[i + 1]; ++j)
                            {
                                var line = block[j] as Line.Operation;
                                if (line.left == re.left || line.left == re.right)
                                {
                                    cbo = false;
                                    break;
                                }
                            }

                            if (cbo)
                            {
                                wo = true;

                                var replace = (block[lines[i]] as Line.Operation).left;
                                var line = block[lines[i + 1]] as Line.Operation;
                                block[lines[i + 1]] = new Line.Operation(line.left, replace);

                                lines.RemoveAt(i + 1);
                                --i;
                            }
                        }
                    }
                }
            }
        }
    }
}
