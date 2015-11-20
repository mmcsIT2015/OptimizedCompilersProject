﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;
using CompilerExceptions;

using Label = System.Collections.Generic.KeyValuePair<int, int>; // хранит номер блока и номер строки в этом блоке

namespace Compiler
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
    public class Gen3AddrCodeVisitor : IVisitor
    {
        /// <summary>
        /// Код, возвращаемый этой ф-ей, уже разбит на блоки
        /// </summary>
        /// <returns></returns>
        public Compiler.ThreeAddrCode CreateCode()
        {
            EraseEmptyLines();
            Compiler.ThreeAddrCode code = new Compiler.ThreeAddrCode(mLines); 
            //VerifyCorrectnessOfProgram(code);
            return code;
        }

        private Compiler.Block mLines = new Compiler.Block();
        private Stack<string> mStack = new Stack<string>();
        private HashSet<String> leftIDSet = new HashSet<string>();

        public Gen3AddrCodeVisitor()
        {
            UniqueIdsGenerator.Instance().Reset();
        }

        private void ReplaceAllReferencesToLabel(string what, string forWhat)
        {
            foreach (var line in mLines)
            {
                if (line.label == what) line.label = forWhat;
                if (line is Line.GoTo) line.ChangeTargetIfEqual(what, forWhat);
            }
        }

        private void VerifyCorrectnessOfProgram(Compiler.ThreeAddrCode code) {
            //bool isValid = true;
            List<ThreeAddrCode.DefUseInfo> DefUseInfoList = code.GetDefUseInfo();
            List<HashSet<String> > DefSetList = new List<HashSet<string> >();

            for (int i = 0; i < DefUseInfoList.Count; ++i)
                DefSetList.Add(DefUseInfoList[i].Def);

            for (int i = 0; i < DefSetList.Count; ++i)
                for (int j = 0; j < code.blocks[i].Count; ++j)
                {
                    var line = code.blocks[i][j];
                    if (line.IsNot<Line.BinaryExpr>() && line.IsNot<Line.UnaryExpr>()) continue;

                    if (line.Is<Line.BinaryExpr>())
                    {
                        var lineOp = line as Line.BinaryExpr;
                        if (!lineOp.FirstParamIsNumber() && DefSetList[i].Contains(lineOp.first) && !lineOp.SecondParamIsNumber() && DefSetList[i].Contains(lineOp.second)//оба операнда переменные
                            || lineOp.FirstParamIsNumber() && !lineOp.SecondParamIsNumber() && DefSetList[i].Contains(lineOp.second) //первый операнд число, а второй переменная
                            || !lineOp.FirstParamIsNumber() && DefSetList[i].Contains(lineOp.first) && lineOp.SecondParamIsNumber() //первый операнд переменная, а второй число
                            || lineOp.FirstParamIsNumber() && lineOp.second == ""// первый операнд число, а второй пуст
                            || lineOp.SecondParamIsNumber() && lineOp.first == ""//второй операнд число, а первый пуст
                            || lineOp.FirstParamIsNumber() && lineOp.SecondParamIsNumber() //оба операнда числа
                            || !lineOp.FirstParamIsNumber() && DefSetList[i].Contains(lineOp.first) && lineOp.second == ""//первый операнд переменная, а второй пуст
                            || !lineOp.SecondParamIsNumber() && DefSetList[i].Contains(lineOp.second) && lineOp.first == ""//второй операнд переменная, а первый пуст
                            )

                            leftIDSet.Add(lineOp.left);
                        else
                            throw new SemanticException("Одна или две переменные в правой части не определены в пределах одного базового блока");
                    }
                    else
                    {
                        var lineOp = line as Line.UnaryExpr;
                        if (lineOp.ParamIsNumber() || !lineOp.ParamIsNumber() && DefSetList[i].Contains(lineOp.argument)) //если операнд число или переменная
                            leftIDSet.Add(lineOp.left);
                        else
                            throw new SemanticException("Переменная в правой части не определена в пределах одного базового блока");
                    }
                }

            //if (!isValid)
            //{
            //    throw new SemanticException("Тут будут подробности");
            //}
        }

        private void EraseEmptyLines()
        {
            Debug.Assert(mLines.Count != 0);

            for (int i = mLines.Count - 2; i >= 0; --i)
            {
                if (mLines[i].Is<Line.EmptyLine>())
                {
                    string forWhat = mLines[i + 1].label.Count() != 0 ? mLines[i + 1].label : mLines[i].label;
                    mLines[i + 1].label = forWhat;
                    ReplaceAllReferencesToLabel(mLines[i].label, forWhat);

                    mLines.RemoveAt(i);
                }
            }
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
                throw new ArgumentException("permissible only `AssignType.Assign`!");
            }
            else if (node.Expr is BinaryNode)
            {
                (mLines.Last() as Line.BinaryExpr).left = variable;                
            }
            else
            {
                mLines.Add(new Line.Identity(variable, expression));
            }
        }

        public void Visit(StringLiteralNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(VarDeclNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(IfNode node)
        {
            node.Expr.Accept(this);
            string condition = mStack.Pop();
            var labelForTrue = UniqueIdsGenerator.Instance().Get("l");
            var labelForFalse = UniqueIdsGenerator.Instance().Get("l");

            Line.СonditionalJump conditionalLine = new Line.СonditionalJump(condition, labelForTrue);
            mLines.Add(conditionalLine);

            if (node.StatElse != null)
            {
                node.StatElse.Accept(this); //тело для false/else
            }
            
            mLines.Add(new Line.GoTo(labelForFalse));
            var gotoPosition = mLines.Count - 1;

            node.Stat.Accept(this); //тело для true
        
            if (mLines[gotoPosition + 1].label.Count() != 0)
            {
                conditionalLine.target = mLines[gotoPosition + 1].label;
            }
            else mLines[gotoPosition + 1].label = labelForTrue;

            mLines.Add(new Line.EmptyLine());
            mLines.Last().label = labelForFalse;
        }

        public void Visit(CoutNode node)
        {
            // TODO Представить cout как вызов функции в грамматике
            List<string> parameters = new List<string>();
            foreach (var expr in node.ExprList)
            {
                expr.Accept(this);
                var variable = mStack.Pop();
                parameters.Add(variable);
            }

            foreach (var param in parameters)
            {
                mLines.Add(new Line.FunctionParam(param));
            }

            mLines.Add(new Line.FunctionCall("cout", parameters.Count));
        }

        public void Visit(FunctionNode node)
        {
            List<string> parameters = new List<string>();
            foreach (var expr in node.Parameters)
            {
                expr.Accept(this);
                var variable = mStack.Pop();
                parameters.Add(variable);
            }

            foreach (var param in parameters)
            {
                mLines.Add(new Line.FunctionParam(param));
            }

            var temp = UniqueIdsGenerator.Instance().Get("t");
            mLines.Add(new Line.FunctionCall(node.Name, parameters.Count, temp));
            mStack.Push(temp);
        }

        public void Visit(FunctionNodeSt node)
        {
            List<string> parameters = new List<string>();
            foreach (var expr in node.Function.Parameters)
            {
                expr.Accept(this);
                var variable = mStack.Pop();
                parameters.Add(variable);
            }

            foreach (var param in parameters)
            {
                mLines.Add(new Line.FunctionParam(param));
            }
            
            mLines.Add(new Line.FunctionCall(node.Function.Name, parameters.Count));
        }

        public void Visit(WhileNode node)
        {
            string gotoLabel = UniqueIdsGenerator.Instance().Get("l");
            string labelForTrue = UniqueIdsGenerator.Instance().Get("l");
            string labelForFalse = UniqueIdsGenerator.Instance().Get("l");

            var gotoPosition = mLines.Count();
            node.Expr.Accept(this);
            string condition = mStack.Pop();
            mLines.Add(new Line.СonditionalJump(condition, labelForTrue));
            mLines[gotoPosition].label = gotoLabel;

            mLines.Add(new Line.GoTo(labelForFalse));
            var truePosition = mLines.Count();
            node.Stat.Accept(this);
            mLines.Add(new Line.GoTo(gotoLabel));
            mLines[truePosition].label = labelForTrue;

            mLines.Add(new Line.EmptyLine());
            mLines.Last().label = labelForFalse;
        }

        public void Visit(DoWhileNode node)
        {
            var firstStPosition = mLines.Count();
            string firstStLabel = UniqueIdsGenerator.Instance().Get("l");

            node.Stat.Accept(this);
            node.Expr.Accept(this);
            string condition = mStack.Pop();

            mLines[firstStPosition].label = firstStLabel;
            mLines.Add(new Line.СonditionalJump(condition, firstStLabel));
        }

        public void Visit(BinaryNode node)
        {
            node.LeftOperand.Accept(this);
            node.RightOperand.Accept(this);

            string rhs = mStack.Pop();
            string lhs = mStack.Pop();

            string temp = UniqueIdsGenerator.Instance().Get("t");
            mStack.Push(temp);

            mLines.Add(new Line.BinaryExpr(temp, lhs, node.Operation, rhs));
        }

        public void Visit(UnaryNode node)
        {
            node.Expr.Accept(this);

            string expr = mStack.Pop();

            string temp = UniqueIdsGenerator.Instance().Get("t");
            mStack.Push(temp);

            mLines.Add(new Line.UnaryExpr(temp, node.Op, expr));
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
    }
}
