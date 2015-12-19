using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;
using CompilerExceptions;

using Label = System.Collections.Generic.KeyValuePair<int, int>; // хранит номер блока и номер строки в этом блоке

namespace iCompiler
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
        /// Класс описывает ошибку компиляции кода
        /// </summary>
        public class ErrorDescription
        {
            public enum ErrorType { LexError, SyntaxError, SemanticError }
            public ErrorDescription(string Message, int Line, ErrorType Type)
            {
                this.Message = Message;
                this.Line = Line;
                this.Type = Type;
            }
            string Message { get; set; }
            int Line { get; set; }
            ErrorType Type { get; set; }

            public override string ToString()
            {
                switch(Type)
                {
                    case ErrorType.LexError:
                        return "Lexer error: " + Message; //+line
                    case ErrorType.SemanticError:
                        return "Semantic error: " + Message; //+line
                    case ErrorType.SyntaxError:
                        return "Syntax error: " + Message; //+line
                    default:
                        return "Cannot happen";
                }
            }

            public override bool Equals(object obj)
            {                
                if (obj.GetType() != this.GetType()) return false;
                var err = obj as ErrorDescription;
                return err.Message == this.Message;                    
            }

            public override int GetHashCode()
            {
                return Message.GetHashCode();
            }
        }        

        /// <summary>
        /// Код, возвращаемый этой ф-ей, уже разбит на блоки
        /// </summary>
        /// <returns></returns>
        public iCompiler.ThreeAddrCode CreateCode()
        {
            EraseEmptyLines();
            CompleteTableOfNames();
            var code = new iCompiler.ThreeAddrCode(mLines);
            code.tableOfNames = mTableOfNames;
            return code;
        }

        public HashSet<ErrorDescription> mErrors = new HashSet<ErrorDescription>();
        private iCompiler.Block mLines = new iCompiler.Block();
        private Stack<string> mStack = new Stack<string>();
        private Dictionary<string, SimpleVarType> mTableOfNames = new Dictionary<string, SimpleVarType>();

        public Gen3AddrCodeVisitor()
        {            
            UniqueIdsGenerator.Instance().Reset();
        }

        private SimpleVarType Cast(SimpleVarType lhs, SimpleVarType rhs)
        {
            if (lhs == rhs) return lhs;
            else if (lhs == SimpleVarType.Bool || rhs == SimpleVarType.Bool)
            {
                return SimpleVarType.Bool;
            }
            else if (lhs == SimpleVarType.Float || rhs == SimpleVarType.Float)
            {
                return SimpleVarType.Float;
            }

            return SimpleVarType.Int;
        }

        private void CheckDefinitionVariable(string variable)
        {
            if (variable[0] != '@')
            {
                if (!mTableOfNames.ContainsKey(variable))
                {
                    mErrors.Add(new ErrorDescription("Используется необъявленная переменная: " + variable, 0, ErrorDescription.ErrorType.SemanticError));                    
                }
            }
        }

        private void CompleteTableOfNames()
        {
            // тип переменных в услови СonditionalJump - bool
            foreach (var line in mLines.Where(e => e is Line.СonditionalJump).Select(e => e as Line.СonditionalJump))
            {
                if (mTableOfNames.ContainsKey(line.condition))
                {
                    mTableOfNames[line.condition] = SimpleVarType.Bool;
                }
                else
                {
                    mTableOfNames.Add(line.condition, SimpleVarType.Bool);
                }
            }

            foreach (var line in mLines)
            {
                if (line is Line.UnaryExpr)
                {
                    var unary = line as Line.UnaryExpr;
                    CheckDefinitionVariable(unary.left);

                    if (unary.ArgIsNumber()) continue;
                    else CheckDefinitionVariable(unary.argument);

                    if (mTableOfNames.ContainsKey(unary.left)) continue;

                    if (unary.IsBoolExpr()) {
                        mTableOfNames.Add(unary.left, SimpleVarType.Bool);
                    }
                    else
                    {
                        mTableOfNames.Add(unary.left, mTableOfNames[unary.argument]);
                    }
                }
                else if (line is Line.Identity)
                {
                    var identity = line as Line.Identity;
                    CheckDefinitionVariable(identity.left);

                    if (identity.RightIsNumber()) continue;
                    else CheckDefinitionVariable(identity.right);

                    if (mTableOfNames.ContainsKey(identity.left)) continue;

                    mTableOfNames.Add(identity.left, mTableOfNames[identity.right]);
                }
                else if (line is Line.BinaryExpr)
                {
                    var expr = line as Line.BinaryExpr;
                    CheckDefinitionVariable(expr.left);
                    if (!expr.FirstParamIsNumber()) CheckDefinitionVariable(expr.first);
                    if (!expr.SecondParamIsNumber()) CheckDefinitionVariable(expr.second);

                    if (mTableOfNames.ContainsKey(expr.left)) continue;

                    if (expr.IsBoolExpr())
                    {
                        mTableOfNames.Add(expr.left, SimpleVarType.Bool);
                    }
                    else
                    {
                        if (expr.FirstParamIsNumber() && expr.SecondParamIsNumber())
                        {
                            if (!expr.FirstParamIsIntNumber() || !expr.SecondParamIsIntNumber())
                            {
                                mTableOfNames.Add(expr.left, SimpleVarType.Float);
                            }
                            else
                            {
                                mTableOfNames.Add(expr.left, SimpleVarType.Int);
                            }
                        }
                        else if (!expr.FirstParamIsNumber() && !expr.SecondParamIsNumber())
                        {
                            var type = Cast(mTableOfNames[expr.first], mTableOfNames[expr.second]);
                            mTableOfNames.Add(expr.left, type);
                        }
                        else if (!expr.FirstParamIsNumber())
                        {
                            var secondType = expr.SecondParamIsIntNumber() ? SimpleVarType.Int : SimpleVarType.Float;
                            var resultType = Cast(mTableOfNames[expr.first], secondType);
                            mTableOfNames.Add(expr.left, resultType);
                        }
                        else //if (!expr.SecondParamIsNumber())
                        {
                            var firstType = expr.FirstParamIsIntNumber() ? SimpleVarType.Int : SimpleVarType.Float;
                            var resultType = Cast(firstType, mTableOfNames[expr.second]);
                            mTableOfNames.Add(expr.left, resultType);
                        }
                    }
                }
            }
        }

        private void ReplaceAllReferencesToLabel(string what, string forWhat)
        {
            foreach (var line in mLines)
            {
                if (line.label == what) line.label = forWhat;
                if (line is Line.GoTo) line.ChangeTargetIfEqual(what, forWhat);
            }
        }

        private void CheckRealLabel(StatementNode node, int index = -1)
        {
            if (node.HasLabel())
            {
                if (index == -1) index = mLines.Count() - 1;
                var oldLabel = mLines.Last().label;
                mLines[index].label = node.Label.Name;
                if (oldLabel.Length > 0)
                {
                    ReplaceAllReferencesToLabel(oldLabel, node.Label.Name);
                }
            }
        }

        private void EraseEmptyLines()
        {
            Debug.Assert(mLines.Count != 0);

            for (int i = mLines.Count - 2; i >= 0; --i)
            {
                if (mLines[i].Is<Line.EmptyLine>())
                {
                    string forWhat = mLines[i + 1].HasLabel() ? mLines[i + 1].label : mLines[i].label;
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
                if (unary.Expr is IntNumNode || unary.Expr is FloatNumNode || unary.Expr is IdNode)
                {
                    unary.Expr.Accept(this);
                    mLines.Add(new Line.UnaryExpr(variable, unary.Op, mStack.Pop()));
                    CheckRealLabel(node);
                    return;
                }
            }

            int nextLine = mLines.Count();

            node.Expr.Accept(this);
            string expression = mStack.Pop();

            if (node.AssOp != AssignType.Assign)
            {
                mErrors.Add(new ErrorDescription("Разрешено только присваивание типа `AssignType.Assign`!", 0, ErrorDescription.ErrorType.SemanticError));                
                //throw new ArgumentException();
            }
            else if (node.Expr is BinaryNode)
            {
                (mLines.Last() as Line.BinaryExpr).left = variable;
                CheckRealLabel(node, nextLine);
            }
            else
            {
                mLines.Add(new Line.Identity(variable, expression));
                CheckRealLabel(node, nextLine);
            }
            if (!node.IsDeclaration)
                CheckDefinitionVariable(variable);
        }

        public void Visit(StringLiteralNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(VarDeclListNode node)
        {
            if (node.HasLabel())
            {
                string desc = "Некорректная метка: " + node.Label.Name + "! Запрещено использование меток при объявлении переменной!";
                mErrors.Add(new ErrorDescription(desc, 0, ErrorDescription.ErrorType.SemanticError));
            }

            foreach (var item in node.VariablesList)
            {
                item.Accept(this);
                mTableOfNames.Add(item.GetID().Name, node.VariablesType);                
            }
        }

        public void Visit(VarDeclNode node)
        {
            if (mTableOfNames.ContainsKey(node.GetID().Name))
            {
                string desc = "Повторное объявление переменной: " + node.GetID().Name + "!";
                mErrors.Add(new ErrorDescription(desc, 0, ErrorDescription.ErrorType.SemanticError));
            }

            if (node.IsAssigned())
            {
                Visit(node.ValueAssignment);
            }
        }

        public void Visit(GotoNode node)
        {
            mLines.Add(new Line.GoTo(node.Target.Name));
            CheckRealLabel(node);
        }

        public void Visit(IfNode node)
        {
            int nextLine = mLines.Count();

            node.Expr.Accept(this);
            string condition = mStack.Pop();
            var labelForTrue = UniqueIdsGenerator.Instance().Get("l");
            var labelForFalse = UniqueIdsGenerator.Instance().Get("l");

            var conditionalLine = new Line.СonditionalJump(condition, labelForTrue);
            mLines.Add(conditionalLine);
            CheckRealLabel(node, nextLine);

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
            int nextLine = mLines.Count();

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
            CheckRealLabel(node, nextLine);
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
            int nextLine = mLines.Count();

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
            CheckRealLabel(node, nextLine);
        }

        public void Visit(WhileNode node)
        {
            string labelForTrue = UniqueIdsGenerator.Instance().Get("l");
            string labelForFalse = UniqueIdsGenerator.Instance().Get("l");

            var jumpPosition = mLines.Count();

            node.Expr.Accept(this);
            string condition = mStack.Pop();

            mLines.Add(new Line.СonditionalJump(condition, labelForTrue));

            string jumpLabel;
            if (node.HasLabel()) jumpLabel = node.Label.Name;
            else if (mLines[jumpPosition].HasLabel()) jumpLabel = mLines[jumpPosition].label;
            else jumpLabel = UniqueIdsGenerator.Instance().Get("l");
            mLines[jumpPosition].label = jumpLabel;

            mLines.Add(new Line.GoTo(labelForFalse));
            var truePosition = mLines.Count();
            node.Stat.Accept(this);
            mLines.Add(new Line.GoTo(jumpLabel));
            if (mLines[truePosition].HasLabel())
            {
                ReplaceAllReferencesToLabel(mLines[truePosition].label, labelForTrue);
            }
            mLines[truePosition].label = labelForTrue;

            mLines.Add(new Line.EmptyLine());
            mLines.Last().label = labelForFalse;
        }

        public void Visit(DoWhileNode node)
        {
            var entryPoint = mLines.Count();

            node.Stat.Accept(this);
            node.Expr.Accept(this);
            string condition = mStack.Pop();

            string label;
            if (node.HasLabel()) label = node.Label.Name;
            else if (mLines[entryPoint].HasLabel()) label = mLines[entryPoint].label;
            else label = UniqueIdsGenerator.Instance().Get("l");

            mLines.Add(new Line.СonditionalJump(condition, label));
            mLines[entryPoint].label = label;
        }

        public void Visit(BinaryNode node)
        {
            if (node.Operation == Operator.Not)
            {
                var desc = "Оператор " + node.Operation + " не является бинарным оператором!";
                mErrors.Add(new ErrorDescription(desc, 0, ErrorDescription.ErrorType.SemanticError));                
            }

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
                mErrors.Add(new ErrorDescription(desc, 0, ErrorDescription.ErrorType.SemanticError));
            }

            if (node.Op != Operator.Minus && node.Op != Operator.Not && node.Op != Operator.Plus)
            {
                var desc = "Недопустимый унарный оператор: " + node.Op + "!\n";
                desc += "Разрешены лишь операторы " + Operator.Minus + ", " + Operator.Not + " и " + Operator.Plus + ".";
                mErrors.Add(new ErrorDescription(desc, 0, ErrorDescription.ErrorType.SemanticError));
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
            string str = node.Num.ToString();
            str = str.Replace(',', '.');
            mStack.Push(str);
        }

        public void Visit(EndlNode endlNode)
        {
            string str = "endl";
            mStack.Push(str);
        }
    }
}
