﻿using System;
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

        private Dictionary<string, expression> RollExpressionsToNormalForm(iCompiler.ThreeAddrCode code)
        {
            var usedVars = new Dictionary<string, List<string>>();
            var exprs = new Dictionary<string, expression>();

            for (int i = 0; i < code.blocks.Count; i++)
            {
                for (int j = 0; j < code.blocks[i].Count; j++)
                {
                    var line = code.blocks[i][j];
                    if (line.IsEmpty()) continue;
                    if (line is iCompiler.Line.GoTo) continue;
                    if (line is iCompiler.Line.FunctionCall || line is iCompiler.Line.FunctionParam) continue;

                    String defOfExpr = (line as iCompiler.Line.Expr).left;
                    if (defOfExpr.First() != '@') defOfExpr = code.GetLineId(line).ToString();

                    if (line is iCompiler.Line.BinaryExpr)
                    {
                        var bin = line as iCompiler.Line.BinaryExpr;

                        expression left = exprs.ContainsKey(bin.first) ? exprs[bin.first] : MakeExpr(bin.first);
                        expression right = exprs.ContainsKey(bin.second) ? exprs[bin.second] : MakeExpr(bin.second);
                        Operators op = Map(bin.operation);

                        var rightPartOfExpr = new bin_expr(left, right, op);
                        if (exprs.ContainsKey(defOfExpr)) exprs.Remove(defOfExpr);
                        exprs.Add(defOfExpr, rightPartOfExpr);
                    }
                    else if (line is iCompiler.Line.Identity)
                    {
                        var id = line as iCompiler.Line.Identity;

                        var rightPartOfExpr = exprs.ContainsKey(id.right) ? exprs[id.right] : MakeExpr(id.right);
                        if (exprs.ContainsKey(defOfExpr)) exprs.Remove(defOfExpr);
                        exprs.Add(defOfExpr, rightPartOfExpr);
                    }
                    else if (line is iCompiler.Line.UnaryExpr)
                    {
                        var un = line as iCompiler.Line.UnaryExpr;

                        expression expr = exprs.ContainsKey(un.argument) ? exprs[un.argument] : MakeExpr(un.argument);
                        Operators op = Map(un.operation);

                        var rightPartOfExpr = new un_expr(expr, op);
                        if (exprs.ContainsKey(defOfExpr)) exprs.Remove(defOfExpr);
                        exprs.Add(defOfExpr, rightPartOfExpr);
                    }
                }
            }

            return exprs;
        }

        private var_def_statement GetDeclVariables(iCompiler.ThreeAddrCode code, ProgramTree.SimpleVarType type)
        {
            var decl = new var_def_statement();
            decl.vars_type = new named_type_reference(Map(type));
            decl.vars = new ident_list();
            bool found = false;
            foreach (var name in code.tableOfNames.Keys)
            {
                if (code.tableOfNames[name] == type)
                {
                    found = true;
                    decl.vars.Add(new ident(name));
                }
            }

            return found ? decl : null;
        }

        public block Generate(iCompiler.ThreeAddrCode code)
        {
            block root = new block();
            root.defs = new declarations();

            var intDecl = GetDeclVariables(code, ProgramTree.SimpleVarType.Int);
            if (intDecl != null) root.defs.Add(intDecl);

            var boolDecl = GetDeclVariables(code, ProgramTree.SimpleVarType.Bool);
            if (boolDecl != null) root.defs.Add(boolDecl);

            var floatDecl = GetDeclVariables(code, ProgramTree.SimpleVarType.Float);
            if (floatDecl != null) root.defs.Add(floatDecl);

            root.program_code = new statement_list();
            var labels = new label_definitions();
            labels.labels = new ident_list();
            
            foreach (var block in code.blocks) {
                foreach (var line in block)
                {
                    var lineId = code.GetLineId(line).ToString();
                    if (line.HasLabel())
                    {
                        labels.labels.Add(new ident(line.label));

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
                    else if (line is iCompiler.Line.ConditionalJump)
                    {
                        var st = line as iCompiler.Line.ConditionalJump;
                        var condition = MakeExpr(st.condition);
                        var if_body = new goto_statement(new ident(st.target));
                        root.program_code.Add(new if_node(condition, if_body));
                    }
                    else if (line is iCompiler.Line.GoTo)
                    {
                        var st = line as iCompiler.Line.GoTo;
                        root.program_code.Add(new goto_statement(st.target));
                    }
                }
            }

            root.defs.Add(labels);
            return root;
        }
    }
}
