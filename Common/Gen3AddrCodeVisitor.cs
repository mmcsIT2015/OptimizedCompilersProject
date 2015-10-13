using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;

using Label = System.Collections.Generic.KeyValuePair<int, int>; // хранит номер блока и номер строки в этом блоке

namespace SimpleLang
{
    /// <summary>
    /// Выполняет обход дерева и возвращает полученный трехадресный код
    /// Пример использования:
    /// ====
    ///     Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
    ///     codeGenerator.Visit(parser.root);
    /// 
    ///     Console.WriteLine(codeGenerator.Code);
    ///     
    /// </summary>
    class Gen3AddrCodeVisitor : IVisitor
    {
        public ThreeAddrCode Code { get; set; }

        private Stack<string> mStack = new Stack<string>();
        private Dictionary<BinaryType, string> mOperators = new Dictionary<BinaryType, string>();
        private Dictionary<AssignType, string> mAssigns = new Dictionary<AssignType, string>();
        private UniqueIdsGenerator mLabelsGenerator = UniqueIdsGenerator.Instance();
        private UniqueIdsGenerator mTempVarsGenerator = UniqueIdsGenerator.Instance();

        public int mLabelRandomPartLength = 4;
        public int mVarRandomPartLength = 8;

        public Gen3AddrCodeVisitor()
        {
            CreateDictionaries();

            Code = new ThreeAddrCode();
            Code.NewBlock();
        }

        private void CreateDictionaries()
        {
            mOperators.Add(BinaryType.Plus, "+");
            mOperators.Add(BinaryType.Minus, "-");
            mOperators.Add(BinaryType.Mult, "*");
            mOperators.Add(BinaryType.Div, "/");
            mOperators.Add(BinaryType.Less, "<");
            mOperators.Add(BinaryType.More, ">");
            mOperators.Add(BinaryType.Equal, "==");
            mOperators.Add(BinaryType.NotEqual, "!=");
            mOperators.Add(BinaryType.LessEqual, "<=");
            mOperators.Add(BinaryType.MoreEqual, ">=");

            mAssigns.Add(AssignType.Assign, "=");
            mAssigns.Add(AssignType.AssignPlus, "+");
            mAssigns.Add(AssignType.AssignMinus, "-");
            mAssigns.Add(AssignType.AssignMult, "*");
            mAssigns.Add(AssignType.AssignDivide, "/");
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
            node.Id.Accept(this);
            node.Expr.Accept(this);

            string expression = mStack.Pop();
            string variable = mStack.Pop();

            if (node.AssOp != AssignType.Assign)
            {
                Code.AddLine(new ThreeAddrCode.Line(variable, variable, AssignToOperation(node.AssOp), expression));
            }
            else if (node.Expr is BinaryNode)
            {
                Code.blocks.Last().Last().left = variable;                
            }
            else
            {
                Code.AddLine(new ThreeAddrCode.Line(variable, "", expression));
            }
        }

        public void Visit(IfNode node)
        {
            node.Expr.Accept(this);
            string ifExpression = mStack.Pop();
            var labelForTrue = mLabelsGenerator.Get(mLabelRandomPartLength);
            var labelForFalse = mLabelsGenerator.Get(mLabelRandomPartLength);

            Code.AddLine(new ThreeAddrCode.Line(ifExpression, labelForTrue, "if", ""));
            Label ifPosition = Code.GetLastPosition();
            if (node.StatElse != null)
            {
                node.StatElse.Accept(this); //тело для false/else
            }

            Code.AddLine(new ThreeAddrCode.Line(labelForFalse, "", "goto", ""));
            Label gotoPosition = Code.GetLastPosition();

            node.Stat.Accept(this); //тело для true
            Code.GetLine(gotoPosition.Key, gotoPosition.Value + 1).label = labelForTrue;

            Code.AddLine(ThreeAddrCode.Line.CreateEmpty());
            Code.GetLine(Code.GetLastPosition()).label = labelForFalse;
        }

        public void Visit(CoutNode node)
        {
            List<string> parameters = new List<string>();
            foreach (var expr in node.ExprList)
            {
                expr.Accept(this);
                var variable = mStack.Pop();
                parameters.Add(variable);
            }

            foreach (var param in parameters)
            {
                Code.AddLine(new ThreeAddrCode.Line(param, "", "param", ""));
            }

            Code.AddLine(new ThreeAddrCode.Line("cout", "", "call", parameters.Count.ToString()));
        }

        public void Visit(WhileNode node)
        {
            string gotoLabel = mLabelsGenerator.Get(mLabelRandomPartLength);
            string labelForTrue = mLabelsGenerator.Get(mLabelRandomPartLength);
            string labelForFalse = mLabelsGenerator.Get(mLabelRandomPartLength);

            Label gotoPosition = new Label(Code.GetLastPosition().Key, Code.GetLastPosition().Value + 1);
            node.Expr.Accept(this);
            string ifExpression = mStack.Pop();
            Code.AddLine(new ThreeAddrCode.Line(ifExpression, labelForTrue, "if", ""));
            Code.GetLine(gotoPosition).label = gotoLabel;

            Code.AddLine(new ThreeAddrCode.Line(labelForFalse, "", "goto", ""));
            Label truePosition = new Label(Code.GetLastPosition().Key, Code.GetLastPosition().Value + 1);
            node.Stat.Accept(this);
            Code.AddLine(new ThreeAddrCode.Line(gotoLabel, "", "goto", ""));
            Code.GetLine(truePosition).label = labelForTrue;

            Code.AddLine(ThreeAddrCode.Line.CreateEmpty());
            Code.GetLine(Code.GetLastPosition()).label = labelForFalse;
        }

        public void Visit(DoWhileNode node)
        {
            Label firstStPosition = new Label(Code.GetLastPosition().Key, Code.GetLastPosition().Value + 1);
            string firstStLabel = mLabelsGenerator.Get(mLabelRandomPartLength);

            node.Stat.Accept(this);
            node.Expr.Accept(this);
            string ifExpression = mStack.Pop();

            Code.GetLine(firstStPosition).label = firstStLabel;
            Code.AddLine(new ThreeAddrCode.Line(ifExpression, firstStLabel, "if", ""));
        }

        public void Visit(BinaryNode node)
        {
            node.LeftOperand.Accept(this);
            node.RightOperand.Accept(this);

            string rightOperand = mStack.Pop();
            string leftOperand = mStack.Pop();

            string temp = mTempVarsGenerator.Get(mVarRandomPartLength);
            mStack.Push(temp);

            Code.AddLine(new ThreeAddrCode.Line(temp, leftOperand, OperatorToString(node.Operation), rightOperand));
        }

        public void Visit(IdNode node)
        {
            mStack.Push(node.Name);
        }

        public void Visit(IntNumNode node)
        {
            mStack.Push(node.Num.ToString());
        }

        public void Visit(FloatNumNode node)
        {
            mStack.Push(node.Num.ToString());
        }

        private string OperatorToString(BinaryType op)
        {
            if (mOperators.ContainsKey(op))
            {
                return mOperators[op];
            }

            return "";
        }

        private string AssignToOperation(AssignType assign)
        {
            if (mAssigns.ContainsKey(assign))
            {
                if (assign == AssignType.Assign)
                {
                    Debug.Assert(false, "Not expected behaviour");
                }

                return mAssigns[assign];
            }

            return "";
        }

    }
}
