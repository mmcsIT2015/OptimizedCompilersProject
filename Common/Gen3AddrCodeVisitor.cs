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

        private Stack<string> stack = new Stack<string>();
        private Dictionary<BinaryType, string> operators = new Dictionary<BinaryType, string>();
        private Dictionary<AssignType, string> assigns = new Dictionary<AssignType, string>();
        private UniqueIdsGenerator labelsGenerator = new UniqueIdsGenerator("L");
        private UniqueIdsGenerator tempVarsGenerator = new UniqueIdsGenerator("t");

        public Gen3AddrCodeVisitor()
        {
            CreateDictionaries();

            Code = new ThreeAddrCode();
            Code.NewBlock();
        }

        private void CreateDictionaries()
        {
            operators.Add(BinaryType.Plus, "+");
            operators.Add(BinaryType.Minus, "-");
            operators.Add(BinaryType.Mult, "*");
            operators.Add(BinaryType.Div, "/");
            operators.Add(BinaryType.Less, "<");
            operators.Add(BinaryType.More, ">");
            operators.Add(BinaryType.Equal, "==");
            operators.Add(BinaryType.NotEqual, "!=");
            operators.Add(BinaryType.LessEqual, "<=");
            operators.Add(BinaryType.MoreEqual, ">=");

            assigns.Add(AssignType.Assign, "=");
            assigns.Add(AssignType.AssignPlus, "+");
            assigns.Add(AssignType.AssignMinus, "-");
            assigns.Add(AssignType.AssignMult, "*");
            assigns.Add(AssignType.AssignDivide, "/");
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
            Debug.Assert(stack.Count() == 0, "Expression stack is not empty");

            node.Id.Accept(this);
            node.Expr.Accept(this);

            string expression = stack.Pop();
            string variable = stack.Pop();

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

            Debug.Assert(stack.Count() == 0, "Expression stack is not empty");
        }

        public void Visit(IfNode node)
        {
            node.Expr.Accept(this);
            string ifExpression = stack.Pop();
            var labelForTrue = labelsGenerator.Get();
            var labelForFalse = labelsGenerator.Get();

            Code.AddLine(new ThreeAddrCode.Line(ifExpression, labelForTrue, "if", ""));
            Label ifPosition = Code.GetLastPosition();
            if (node.StatElse != null)
            {
                node.StatElse.Accept(this); //тело для false/else
            }

            Code.AddLine(new ThreeAddrCode.Line("", "", "goto", ""));
            Label gotoPosition = Code.GetLastPosition();

            node.Stat.Accept(this); //тело для true
            Code.GetLine(gotoPosition.Key, gotoPosition.Value + 1).label = labelForTrue;

            Code.GetLine(gotoPosition).left = labelForFalse;

            Code.AddLine(ThreeAddrCode.Line.CreateEmpty());
            Code.GetLine(Code.GetLastPosition()).label = labelForFalse;
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

            string rightOperand = stack.Pop();
            string leftOperand = stack.Pop();

            string temp = tempVarsGenerator.Get();
            stack.Push(temp);

            Code.AddLine(new ThreeAddrCode.Line(temp, leftOperand, OperatorToString(node.Operation), rightOperand));
        }

        public void Visit(IdNode node)
        {
            stack.Push(node.Name);
        }

        public void Visit(IntNumNode node)
        {
            stack.Push(node.Num.ToString());
        }

        public void Visit(FloatNumNode node)
        {
            stack.Push(node.Num.ToString());
        }

        private string OperatorToString(BinaryType op)
        {
            if (operators.ContainsKey(op))
            {
                return operators[op];
            }

            Debug.Assert(false, "Not implemented conversion");
            return "";
        }

        private string AssignToString(AssignType assign)
        {
            if (assigns.ContainsKey(assign))
            {
                if (assign == AssignType.Assign)
                {
                    Debug.Assert(false, "Not expected behaviour");
                }

                return assigns[assign];
            }

            Debug.Assert(false, "Not implemented conversion");
            return "";
        }

    }
}
