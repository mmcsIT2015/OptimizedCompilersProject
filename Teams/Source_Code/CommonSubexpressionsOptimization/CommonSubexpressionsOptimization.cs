using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SimpleLang;

namespace SimpleLang.Optimization
{
    /// <summary>
    /// Оптимизация: Устранение общих подвыражений
    /// Примечание: Использовать после разбиения на внутренние блоки
    /// </summary>
    class CommonSubexpressionsOptimization : IOptimizator
    {
        class RightExpr
        {
            public string left, right, command;

            public RightExpr(string l, string r, string com)
            {
                left = l;
                right = r;
                command = com;
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

        private ThreeAddrCode code;

        public CommonSubexpressionsOptimization(ThreeAddrCode code)
        {
            this.code = code;
        }

        public ThreeAddrCode Code { get { return code; } }

        public void Optimize()
        {
            bool wo = true;

            while (wo)
            {
                wo = false;

                foreach (Block b in code.blocks)
                {
                    Dictionary<RightExpr, List<int>> dict = new Dictionary<RightExpr, List<int>>(),
                        dict_a = new Dictionary<RightExpr, List<int>>();

                    for (int i = 0; i < b.Count; ++i)
                    {
                        RightExpr re = new RightExpr(b[i].first, b[i].second, b[i].command);
                        if (!dict.ContainsKey(re))
                            dict.Add(re, new List<int>());
                        dict[re].Add(i);
                    }

                    foreach (RightExpr re in dict.Keys)
                        if ((re.left != "" || re.right != "") && dict[re].Count > 1)
                            dict_a.Add(re, dict[re]);

                    foreach (RightExpr re in dict_a.Keys)
                    {
                        List<int> lns = dict_a[re];
                        for (int i = 0; i < lns.Count - 1; ++i)
                        {
                            bool cbo = true;
                            for (int j = lns[i]; j < lns[i + 1]; ++j)
                                if (b[j].left == re.left || b[j].left == re.right)
                                {
                                    cbo = false;
                                    break;
                                }

                            if (cbo)
                            {
                                wo = true;

                                b[lns[i + 1]].command = "";
                                b[lns[i + 1]].first = b[lns[i]].left;
                                b[lns[i + 1]].second = "";
                                lns.RemoveAt(i + 1);
                                --i;
                            }
                        }
                    }
                }
            }
        }
    }
}
