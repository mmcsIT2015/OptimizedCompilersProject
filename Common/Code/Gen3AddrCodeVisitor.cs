using System;
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
            //VerifyCorrectnessOfProgram();
            return new Compiler.ThreeAddrCode(mLines); ;
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

        private void VerifyCorrectnessOfProgram() {
            //bool isValid = true;
            for (int i = 0; i < mLines.Count; ++i)
            {
                var line = mLines[i];
                if (line.IsNot<Line.BinaryExpr>() && line.IsNot<Line.UnaryExpr>() && line.IsNot<Line.Identity>()) continue;

                if (line.Is<Line.BinaryExpr>())
                {
                    var lineBinExpr = line as Line.BinaryExpr;

                    if (leftIDSet.Contains(lineBinExpr.first) && leftIDSet.Contains(lineBinExpr.second) //если обе переменные определены ранее
                        || leftIDSet.Contains(lineBinExpr.first) && lineBinExpr.SecondParamIsNumber() // если первая переменная определена и второй операнд число
                        || leftIDSet.Contains(lineBinExpr.second) && lineBinExpr.FirstParamIsNumber() // если вторая переменная определена и первый операнд число
                        || lineBinExpr.FirstParamIsNumber() && lineBinExpr.SecondParamIsNumber() // если оба операнда числа
                        )
                    {
                        leftIDSet.Add(lineBinExpr.left);
                    }
                    else
                        throw new SemanticException("Одна или две переменные в правой части BinaryExpr не определены. Выражение " + lineBinExpr.ToString());
                }
                else if (line.Is<Line.UnaryExpr>())
                {
                    var lineUnExpr = line as Line.UnaryExpr;
                    if (lineUnExpr.ParamIsNumber() || leftIDSet.Contains(lineUnExpr.argument)) //если операнд число или переменная, определенная ранее
                        leftIDSet.Add(lineUnExpr.left);
                    else
                        throw new SemanticException("Переменная в правой части UnaryExpr не определена. Выражение " + lineUnExpr.ToString());
                }
                else //if (line.Is<Line.Identity>())
                {
                    var lineIdentExpr = line as Line.Identity;
                    if (!(lineIdentExpr.RightIsNumber() || leftIDSet.Contains(lineIdentExpr.right))) //если не число и не переменная, определенная ранее
                        throw new SemanticException("Переменная в правой части Identity не определена. Выражение " + lineIdentExpr.ToString());
                    else if (lineIdentExpr.left == lineIdentExpr.right) // если выражение вида x = x
                        throw new SemanticException("Неожиданно встретилось выражение вида x = x. Выражение " + lineIdentExpr.ToString());
                    else
                        leftIDSet.Add(lineIdentExpr.left);
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

        public void Visit(AssignNode node)
        {
            node.Id.Accept(this);
            string variable = mStack.Pop();

            if (node.Expr is UnaryNode)
            {
                var unary = node.Expr as UnaryNode;
                if (unary.Expr is IntNumNode)
                {
                    mLines.Add(new Line.UnaryExpr(variable, unary.Op, (unary.Expr as IntNumNode).Num.ToString()));
                    return;
                }
                else if (unary.Expr is FloatNumNode)
                {
                    mLines.Add(new Line.UnaryExpr(variable, unary.Op, (unary.Expr as FloatNumNode).Num.ToString()));
                    return;
                }
                else if (unary.Expr is IdNode)
                {
                    mLines.Add(new Line.UnaryExpr(variable, unary.Op, (unary.Expr as IdNode).Name));
                    return;
                }
            }

            node.Expr.Accept(this);
            string expression = mStack.Pop();

            if (node.AssOp != AssignType.Assign)
            {
                throw new ArgumentException("Разрешено только присваивание типа `AssignType.Assign`!");
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
            if (node.Expr is StringLiteralNode)
            {
                var desc = "Унарный оператор " + node.Op + " не может быть применен в операнду типа `string`!\n";
                desc += "> " + node.Op + (node.Expr as StringLiteralNode).Str;
                throw new SemanticException(desc);
            }

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
