using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using PascalABCCompiler.SyntaxTree;

namespace ParsePABC
{
    public class PascalABCTreeGenerator
    {
        private string Map(ProgramTree.SimpleVarType type)
        {
            switch (type)
            {
                case ProgramTree.SimpleVarType.Bool: return "boolean";
                case ProgramTree.SimpleVarType.Float: return "real";
                case ProgramTree.SimpleVarType.Int: return "integer";
            }

            throw new Exception("Неизвестный тип: " + type);
        }

        private Operators Map(ProgramTree.Operator op)
        {
            switch (op)
            {
                case ProgramTree.Operator.Plus: return Operators.Plus;
                case ProgramTree.Operator.Minus: return Operators.Minus;
                case ProgramTree.Operator.Mult: return Operators.Multiplication;
                case ProgramTree.Operator.Div: return Operators.Division;
                case ProgramTree.Operator.Not: return Operators.LogicalNOT;
                case ProgramTree.Operator.Less: return Operators.Less;
                case ProgramTree.Operator.LessEqual: return Operators.LessEqual;
                case ProgramTree.Operator.Greater: return Operators.Greater;
                case ProgramTree.Operator.GreaterEqual: return Operators.GreaterEqual;
                case ProgramTree.Operator.Equal: return Operators.Equal;
            }

            throw new Exception("Неизвестный оператор: " + op);
        }

        private expression MakeExpr(string val)
        {
            int itemp;
            if (int.TryParse(val, out itemp))
            {
                return new int32_const(itemp);
            }

            double ftemp;
            var style = System.Globalization.NumberStyles.Any;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            if (double.TryParse(val, style, nfi, out ftemp))
            {
                return new double_const(ftemp);
            }

            return new ident(val);
        }

        public syntax_tree_node generate(iCompiler.ThreeAddrCode code)
        {
            block root = new block();
            root.defs = new declarations();
            foreach (var name in code.tableOfNames.Keys)
            {
                var decl = new var_def_statement();
                decl.vars = new ident_list(new ident(name));
                decl.vars_type = new named_type_reference(Map(code.tableOfNames[name]));
                root.defs.Add(decl);
            }

            root.program_code = new statement_list();
            foreach (var block in code.blocks) {
                foreach (var line in block)
                {
                    if (line.HasLabel())
                    {
                        root.defs.Add(new label_definitions(new ident(line.label)));

                        var st = new labeled_statement(line.label);
                        root.program_code.Add(st);
                    }

                    if (line is iCompiler.Line.Identity)
                    {
                        var st = line as iCompiler.Line.Identity;
                        var left = new ident(st.left);
                        var right = MakeExpr(st.right);
                        root.program_code.Add(new assign(left, right));
                    }
                    else if (line is iCompiler.Line.UnaryExpr)
                    {
                        var st = line as iCompiler.Line.UnaryExpr;
                        var left = new ident(st.left);
                        var arg = MakeExpr(st.argument);
                        var expr = new un_expr(arg, Map(st.operation));
                        root.program_code.Add(new assign(left, expr));
                    }
                    else if (line is iCompiler.Line.BinaryExpr)
                    {
                        var st = line as iCompiler.Line.BinaryExpr;
                        var left = new ident(st.left);
                        var first = MakeExpr(st.first);
                        var second = MakeExpr(st.second);
                        var expr = new bin_expr(first, second, Map(st.operation));
                        root.program_code.Add(new assign(left, expr));
                    }
                    else if (line is iCompiler.Line.СonditionalJump)
                    {
                        var st = line as iCompiler.Line.СonditionalJump;
                        var condition = MakeExpr(st.condition);
                        // TODO
                        var statement = new assign(new ident("test"), new ident("test"));
                        root.program_code.Add(new if_node(condition, statement));
                    }
                    else if (line is iCompiler.Line.GoTo)
                    {
                        var st = line as iCompiler.Line.GoTo;
                        root.program_code.Add(new goto_statement(st.target));
                    }
                }
            }

            return root;
        }
    }
}
