using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalABCCompiler.SyntaxTree;
using System.Diagnostics;
using CompilerExceptions;
using iCompiler.Line;
using ProgramTree;
using iCompiler;
using System.Globalization;

namespace ParsePABC
{
    class Gen3AddrCodeVisitor : WalkingVisitorNew
    {
        public iCompiler.ThreeAddrCode CreateCode()
        {
            EraseEmptyLines();
            CompleteTableOfNames();
            var code = new iCompiler.ThreeAddrCode(mLines);
            code.tableOfNames = mTableOfNames;
            return code;
        }

        private iCompiler.Block mLines = new iCompiler.Block();
        private Stack<string> mStack = new Stack<string>();
        private Stack<syntax_tree_node> mAuxStack = new Stack<syntax_tree_node>();
        private Dictionary<string, ProgramTree.SimpleVarType> mTableOfNames = new Dictionary<string, ProgramTree.SimpleVarType>();

        public Gen3AddrCodeVisitor()
        {
            OnLeave = Leave;
            OnEnter = Enter;
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
                    throw new SemanticException("Используется необъявленная переменная: " + variable);
                }
            }
        }

        private void CompleteTableOfNames()
        {
            // тип переменных в услови СonditionalJump - bool
            foreach (var line in mLines.Where(e => e is iCompiler.Line.ConditionalJump).Select(e => e as iCompiler.Line.ConditionalJump))
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
                if (line is iCompiler.Line.UnaryExpr)
                {
                    var unary = line as iCompiler.Line.UnaryExpr;
                    CheckDefinitionVariable(unary.left);

                    if (unary.ArgIsNumber()) continue;
                    else CheckDefinitionVariable(unary.argument);

                    if (mTableOfNames.ContainsKey(unary.left)) continue;

                    if (unary.IsBoolExpr())
                    {
                        mTableOfNames.Add(unary.left, SimpleVarType.Bool);
                    }
                    else
                    {
                        mTableOfNames.Add(unary.left, mTableOfNames[unary.argument]);
                    }
                }
                else if (line is iCompiler.Line.Identity)
                {
                    var identity = line as iCompiler.Line.Identity;
                    CheckDefinitionVariable(identity.left);

                    if (identity.RightIsNumber()) continue;
                    else CheckDefinitionVariable(identity.right);

                    if (mTableOfNames.ContainsKey(identity.left)) continue;

                    mTableOfNames.Add(identity.left, mTableOfNames[identity.right]);
                }
                else if (line is iCompiler.Line.BinaryExpr)
                {
                    var expr = line as iCompiler.Line.BinaryExpr;
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

        private ProgramTree.SimpleVarType Map(string type)
        {
            if (type == "integer") return ProgramTree.SimpleVarType.Int;
            if (type == "real") return ProgramTree.SimpleVarType.Float;
            if (type == "boolean") return ProgramTree.SimpleVarType.Bool;

            throw new Exception("Неизвестный тип: " + type);
        }

        private ProgramTree.Operator Map(Operators op)
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
            else if (node is double_const)
            {
                mStack.Push((node as double_const).val.ToString());
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
                if (mAuxStack.Count() > 0 && mAuxStack.Peek() == node)
                {
                    mAuxStack.Pop();
                    return;
                }

                var stat = node as goto_statement;
                mLines.Add(new iCompiler.Line.GoTo(stat.label.name));
            }

            if (mAuxStack.Count() > 0 && node == mAuxStack.Peek())
            {
                var top = mAuxStack.Pop();
                if (mAuxStack.Count() > 0 && mAuxStack.Peek() is if_node)
                {
                    var condition = mStack.Pop();
                    var last = mAuxStack.Pop() as if_node;

                    Debug.Assert(last.then_body is goto_statement); /* последствия lowering'а */
                    
                    var ifLabel = (last.then_body as goto_statement).label.name;
                    mLines.Add(new iCompiler.Line.ConditionalJump(condition, ifLabel));
                    mAuxStack.Push(last.then_body); // чтобы для goto не генерировать инструкцию снова
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
            else if (node is var_def_statement)
            {
                var defs = node as var_def_statement;
                var type = Map((defs.vars_type as named_type_reference).names.First().name);
                foreach (var id in defs.vars.idents)
                {
                    mTableOfNames.Add(id.name, type);
                    var iv = defs.inital_value;
                    if (iv is ident || iv is int32_const || iv is double_const)
                    {
                        string right = "";
                        if (iv is ident) {
                            right = (iv as ident).name;
                            CheckDefinitionVariable(right);
                        }
                        else if (iv is int32_const) {
                            right = (iv as int32_const).val.ToString();
                        }
                        else if (iv is double_const) {
                            right = (iv as double_const).val.ToString();
                            right = right.Replace(',', '.');
                        }

                        Debug.Assert(right.Length > 0);
                        mLines.Add(new iCompiler.Line.Identity(id.name, right));
                    }
                }
            }
            
        }
    }
}
