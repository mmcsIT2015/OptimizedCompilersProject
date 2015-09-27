using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;

using Label = System.Collections.Generic.KeyValuePair<int, int>; // хранит номер блока и номер строки в этом блоке

namespace SimpleLang
{
    class Gen3AddrCodeVisitor : IVisitor
    {
        public ThreeAddrCode Code { get; set; }

        private Stack<string> exprStack = new Stack<string>();

        public Gen3AddrCodeVisitor()
        {
            Code = new ThreeAddrCode();
            Code.NewBlock();
        }

        public void Visit(BlockNode node)
        {
            foreach (StatementNode st in node.StList)
            {
                st.Accept(this);
            }
        }

        public void Visit(VarNode node)
        {
            // TODO
        }

        public void Visit(AssignNode node)
        {
            Debug.Assert(exprStack.Count() == 0, "Expression stack is not empty");

            node.Id.Accept(this);
            node.Expr.Accept(this);

            string expression = exprStack.Pop();
            string variable = exprStack.Pop();

            if (node.AssOp != AssignType.Assign)
            {
                Code.AddLine(new ThreeAddrCode.Line(variable, variable, AssignToString(node.AssOp), expression));
            }
            else if (node.Expr is BinaryNode)
            {
                Code.blocks.Last().Last().left = variable;                
            }
            else
            {
                Code.AddLine(new ThreeAddrCode.Line(variable, "", expression));
            }

            Debug.Assert(exprStack.Count() == 0, "Expression stack is not empty");
        }

        public void Visit(IfNode node)
        {
            node.Expr.Accept(this);
            string ifExpression = exprStack.Pop();
            Code.AddLine(new ThreeAddrCode.Line(ifExpression, "", "if", ""));
            Label ifGotoLabel = Code.GetLastLabel();
            if (node.StatElse != null)
            {
                node.StatElse.Accept(this);
            }
            Code.AddLine(new ThreeAddrCode.Line("", "", "goto", ""));
            Label afterIfGotoLabel = Code.GetLastLabel();
            Code.GetLine(ifGotoLabel).first = ThreeAddrCode.Line.GetNextLabel();
            node.Stat.Accept(this);
            Code.GetLine(afterIfGotoLabel).left = ThreeAddrCode.Line.GetNextLabel();
        }

        public void Visit(CoutNode node)
        {
            // TODO
        }

        public void Visit(WhileNode node)
        {
            // TODO
        }

        public void Visit(DoWhileNode node)
        {
            // TODO
        }

        public void Visit(BinaryNode node)
        {
            node.LeftOperand.Accept(this);
            node.RightOperand.Accept(this);

            string rightOperand = exprStack.Pop();
            string leftOperand = exprStack.Pop();

            string varTempName = ThreeAddrCode.GetTempVariable();
            exprStack.Push(varTempName);

            Code.AddLine(new ThreeAddrCode.Line(varTempName, leftOperand, OperatorToString(node.Operation), rightOperand));
        }

        public void Visit(IdNode node)
        {
            exprStack.Push(node.Name);
        }

        public void Visit(IntNumNode node)
        {
            exprStack.Push(node.Num.ToString());
        }

        public void Visit(FloatNumNode node)
        {
            exprStack.Push(node.Num.ToString());
        }


        static private string OperatorToString(BinaryType op)
        {
            switch(op)
            {
                case BinaryType.Plus:
                    {
                        return "+";
                    }
                case BinaryType.Minus:
                    {
                        return "-";
                    }
                case BinaryType.Mult:
                    {
                        return "*";
                    }
                case BinaryType.Div:
                    {
                        return "/";
                    }
                case BinaryType.Less:
                    {
                        return "<";
                    }
                case BinaryType.More:
                    {
                        return ">";
                    }
                case BinaryType.Equal:
                    {
                        return "==";
                    }
                case BinaryType.NotEqual:
                    {
                        return "!=";
                    }
                case BinaryType.LessEqual:
                    {
                        return "<=";
                    }
                case BinaryType.MoreEqual:
                    {
                        return "<=";
                    }
                default:
                    {
                        Debug.Assert(false, "Not implemented conversion");
                        return "";
                    }
            }
        }

        static private string AssignToString(AssignType assign)
        {
            switch (assign)
            {
                case AssignType.Assign:
                    {
                        Debug.Assert(false, "Not expected behaviour");
                        return "=";
                    }
                case AssignType.AssignPlus:
                    {
                        return "+";
                    }
                case AssignType.AssignMinus:
                    {
                        return "-";
                    }
                case AssignType.AssignMult:
                    {
                        return "*";
                    }
                case AssignType.AssignDivide:
                    {
                        return "/";
                    }
                default:
                    {
                        Debug.Assert(false, "Not implemented conversion");
                        return "";
                    }
            }
        }

    }
}
