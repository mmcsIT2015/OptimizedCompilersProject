using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalABCCompiler.SyntaxTree;
using System.Diagnostics;
using CompilerExceptions;
using iCompiler.Line;
using iCompiler;

namespace ParsePABC
{
    class Gen3AddrCodeVisitor : WalkingVisitorNew
    {
        public iCompiler.ThreeAddrCode CreateCode()
        {
            EraseEmptyLines();
            var code = new iCompiler.ThreeAddrCode(mLines);
            return code;
        }

        private iCompiler.Block mLines = new iCompiler.Block();
        private Stack<string> mStack = new Stack<string>();
        private Stack<syntax_tree_node> mAuxStack = new Stack<syntax_tree_node>();

        public Gen3AddrCodeVisitor()
        {
            OnLeave = Leave;
            OnEnter = Enter;
        }

        private void ReplaceAllReferencesToLabel(string what, string forWhat)
        {
            foreach (var line in mLines)
            {
                if (line.label == what) line.label = forWhat;
                if (line is iCompiler.Line.GoTo) line.ChangeTargetIfEqual(what, forWhat);
            }
        }

        private void EraseEmptyLines()
        {
            Debug.Assert(mLines.Count != 0);

            for (int i = mLines.Count - 2; i >= 0; --i)
            {
                if (mLines[i].Is<iCompiler.Line.EmptyLine>())
                {
                    string forWhat = mLines[i + 1].HasLabel() ? mLines[i + 1].label : mLines[i].label;
                    mLines[i + 1].label = forWhat;
                    ReplaceAllReferencesToLabel(mLines[i].label, forWhat);

                    mLines.RemoveAt(i);
                }
            }
        }

        ProgramTree.Operator Map(Operators op)
        {
            switch (op)
            {
                case Operators.Plus:
                    return ProgramTree.Operator.Plus;
                case Operators.Minus:
                    return ProgramTree.Operator.Minus;
                case Operators.Multiplication:
                    return ProgramTree.Operator.Mult;
                case Operators.Division:
                    return ProgramTree.Operator.Div;
                case Operators.LogicalNOT:
                    return ProgramTree.Operator.Not;
                case Operators.Less:
                    return ProgramTree.Operator.Less;
                case Operators.LessEqual:
                    return ProgramTree.Operator.LessEqual;
                case Operators.Greater:
                    return ProgramTree.Operator.Greater;
                case Operators.GreaterEqual:
                    return ProgramTree.Operator.GreaterEqual;
                case Operators.Equal:
                    return ProgramTree.Operator.Equal;
                case Operators.NotEqual:
                    return ProgramTree.Operator.NotEqual;
            }

            throw new Exception("Неизвестный оператор: " + op);
        }

        public virtual void Leave(syntax_tree_node node)
        {
            //Console.WriteLine(" Leave: " + node.GetType() + ": " + node.ToString());

            if (node is variable_definitions)
            {
                mStack.Clear(); // нам в стеке объявления не нужны
            }
            if (node is assign)
            {
                if ((node as assign).from is ident || (node as assign).from is int32_const)
                {
                    var from = mStack.Pop();
                    var to = mStack.Pop();

                    mLines.Add(new iCompiler.Line.Identity(to, from));
                }
                else
                {
                    var from = mStack.Pop();
                    var to = mStack.Pop();
                    if (mLines.Last().Is<iCompiler.Line.BinaryExpr>())
                    {
                        var last = mLines.Last() as iCompiler.Line.BinaryExpr;
                        Debug.Assert(last.left == from);

                        last.left = to;
                    }
                    else if (mLines.Last().Is<iCompiler.Line.UnaryExpr>())
                    {
                        var last = mLines.Last() as iCompiler.Line.UnaryExpr;
                        Debug.Assert(last.left == from);

                        last.left = to;
                    }
                }
            }
            else if (node is ident)
            {
                if (!(node as ident).name.Contains('#')) // `#` содержат генерируемые (не нами) метки
                {
                    mStack.Push((node as ident).name);
                }
            }
            else if (node is int32_const)
            {
                mStack.Push((node as int32_const).val.ToString());
            }
            else if (node is bin_expr)
            {
                string rhs = mStack.Pop();
                string lhs = mStack.Pop();
                var operation = Map((node as bin_expr).operation_type);

                string temp = UniqueIdsGenerator.Instance().Get("t");
                mStack.Push(temp);

                mLines.Add(new iCompiler.Line.BinaryExpr(temp, lhs, operation, rhs));
            }
            else if (node is un_expr)
            {
                string arg = mStack.Pop();
                var operation = Map((node as un_expr).operation_type);

                string temp = UniqueIdsGenerator.Instance().Get("t");
                mStack.Push(temp);

                mLines.Add(new iCompiler.Line.UnaryExpr(temp, operation, arg));
            }
            else if (node is goto_statement)
            {
                var stat = node as goto_statement;

                mLines.Add(new iCompiler.Line.GoTo(stat.label.name));
            }

            if (mAuxStack.Count() > 0 && node == mAuxStack.Peek())
            {
                var top = mAuxStack.Pop();

                if (mAuxStack.Count() > 0 && mAuxStack.Peek() is if_node)
                {
                    var temp = mStack.Pop();
                    var last = mAuxStack.Pop() as if_node;
                    var ifLabel = UniqueIdsGenerator.Instance().Get("l");
                    mLines.Add(new iCompiler.Line.СonditionalJump(temp, ifLabel));
                    mStack.Push("if:" + mLines.Count());

                    mLines.Add(new iCompiler.Line.GoTo("")); // goto на тело else
                    mLines.Add(new iCompiler.Line.EmptyLine(ifLabel)); // метка на тело then

                    mAuxStack.Push(last.then_body);
                }
                else // конец if'а
                {
                    var gotoStatment = mLines[int.Parse(mStack.Pop().Split(':')[1])];

                    var label = UniqueIdsGenerator.Instance().Get("l");
                    mLines.Add(new iCompiler.Line.EmptyLine(label)); // метка на тело else

                    (gotoStatment as iCompiler.Line.GoTo).target = label;
                }
            }
        }

        public virtual void Enter(syntax_tree_node node)
        {
            //Console.WriteLine("Enter: " + node.GetType() + ": " + node.ToString());
            if (node is if_node)
            {
                mAuxStack.Push(node);
                mAuxStack.Push((node as if_node).condition);
            }
            else if (node is labeled_statement)
            {
                var label = (node as labeled_statement).label_name.name;
                mLines.Add(new iCompiler.Line.EmptyLine(label));
            }
        }
    }
}
