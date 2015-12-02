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
            Console.WriteLine("lines: " + mLines.Count());
            var code = new iCompiler.ThreeAddrCode(mLines);
            return code;
        }

        private iCompiler.Block mLines = new iCompiler.Block();
        private Stack<string> mStack = new Stack<string>();

        public Gen3AddrCodeVisitor()
        {
            OnLeave = Leave;
            OnEnter = Enter;
        }

        ProgramTree.BinaryOperation Map(Operators op)
        {
            switch (op)
            {
                case Operators.Plus:
                    return ProgramTree.BinaryOperation.Plus;
                case Operators.Minus:
                    return ProgramTree.BinaryOperation.Minus;
                case Operators.Multiplication:
                    return ProgramTree.BinaryOperation.Mult;
                case Operators.Division:
                    return ProgramTree.BinaryOperation.Div;
            }

            Console.WriteLine("[ERROR]");
            return ProgramTree.BinaryOperation.None;
        }

        ProgramTree.UnaryOperation MapUnary(Operators op)
        {
            switch (op)
            {
                case Operators.Minus:
                    return ProgramTree.UnaryOperation.Minus;
            }

            Console.WriteLine("[ERROR]");
            return ProgramTree.UnaryOperation.Not; 
        }

        public virtual void Leave(syntax_tree_node node)
        {
            Console.WriteLine("Leave: " + node.GetType() + ": " + node.ToString());

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
                mStack.Push((node as ident).name);
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
                var operation = MapUnary((node as un_expr).operation_type);

                string temp = UniqueIdsGenerator.Instance().Get("t");
                mStack.Push(temp);

                mLines.Add(new iCompiler.Line.UnaryExpr(temp, operation, arg));
            }
        }

        public virtual void Enter(syntax_tree_node node)
        {
            Console.WriteLine("Enter: " + node.GetType() + ": " + node.ToString());

        }
    }
}
