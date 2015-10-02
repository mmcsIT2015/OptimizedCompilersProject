using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    class ConstantFolding : IOptimizer
    {
        ThreeAddrCode tac;

        public ConstantFolding(ThreeAddrCode tac)
        {
            this.tac = tac;
        }

        //static HashSet<string> ops1 = new HashSet<string> { "+=", "-=", "*=", "/=" };
        static HashSet<string> ops2 = new HashSet<string> { "+", "-", "*", "/" };
        
        public void Optimize()
        {
            FoldConstants(tac);
            ApplyAlgebraicEqualities(tac);
        }

        static void FoldConstants(ThreeAddrCode tac)
        {
            foreach (Block bl in tac.blocks)
            {
                foreach (ThreeAddrCode.Line ln in bl)
                {
                    string cmd = ln.command.Trim();

                    if(ops2.Contains(cmd))
                    {
                        double fst, snd;
                        if(!double.TryParse(ln.first, out fst))
                            continue;
                        if(!double.TryParse(ln.second, out snd))
                            continue;
                        switch(cmd)
                        {
                            case "+": 
                                ln.first = (fst + snd).ToString();
                                break;
                            case "-": 
                                ln.first = (fst - snd).ToString();
                                break;
                            case "*": 
                                ln.first = (fst * snd).ToString();
                                break;
                            case "/": 
                                ln.first = (fst / snd).ToString();
                                break;
                        }
                        ln.command = ln.second = "";
                    }
                }
            }
        }

        static void ApplyAlgebraicEqualities(ThreeAddrCode tac)
        {
            foreach (Block bl in tac.blocks)
            {
                foreach (ThreeAddrCode.Line ln in bl)
                {
                    string cmd = ln.command.Trim();

                    if (ops2.Contains(cmd))
                    {
                        double fst, snd;
                        bool b1 = double.TryParse(ln.first, out fst);
                        bool b2 = double.TryParse(ln.second, out snd);

                        if (b1 && !b2)
                        {
                            if (fst == 0)
                            {
                                if (cmd.Equals("+"))
                                    ln.first = ln.second;
                                if (cmd.Equals("-"))
                                    ln.first = (snd*(-1)).ToString();
                                if (cmd.Equals("*") || cmd.Equals("/"))
                                    ln.first = "0";
                                ln.command = ln.second = "";
                            }
                            if (fst == 1)
                            {
                                if (cmd.Equals("*"))
                                {
                                    ln.first = ln.second;
                                    ln.command = ln.second = "";
                                }
                            }
                        }

                        if (!b1 && b2)
                        {
                            if (snd == 0)
                            {
                                if (cmd.Equals("*"))
                                {
                                    ln.first = "0";
                                    ln.command = ln.second = "";
                                }
                                if (cmd.Equals("+") || cmd.Equals("-"))
                                    ln.command = ln.second = "";
                            }
                            if (snd == 1)
                            {
                                if (cmd.Equals("*") || cmd.Equals("/"))
                                    ln.command = ln.second = "";
                            }
                        }            
                    }
                }
            }
        }
    }
}
