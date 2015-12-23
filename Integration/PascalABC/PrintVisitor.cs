using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PascalABCCompiler.Errors;
using PascalABCCompiler.PCU;
using PascalABCCompiler.SemanticTreeConverters;
using System.CodeDom.Compiler;
using PascalABCCompiler;
using PascalABCCompiler.SyntaxTree;

namespace ParsePABC
{
    public class PrintVisitor : WalkingVisitorNew
    {
        private int off = 0;
        public int Indent = 8;
        public StringBuilder Code = new StringBuilder();

        public PrintVisitor()
        {
            OnEnter = Enter;
            OnLeave = Exit;
        }

        public void Println(string s)
        {
            Code.Append(new string(' ', off));
            Code.Append(s.Replace(",", ", ") + '\n');
        }

        public void Print(string s)
        {
            Code.Append(new string(' ', off));
            Code.Append(s.Replace(",", ", "));
        }

        public void PrintNoOffset(string s)
        {
            Code.Append(s.Replace(",", ", "));
        }

        public void PrintlnNoOffset(string s)
        {
            Code.Append(s.Replace(",", ", ") + '\n');
        }

        public void PrintlnNode(syntax_tree_node st)
        {
            Code.Append(new string(' ', off) + st.ToString().Replace(",", ", ") + '\n');
        }

        public virtual void Enter(syntax_tree_node st)
        {
            if (st is statement_list)
            {
                Println("");
                Println("begin");
                off += Indent;
            }
            else if (st is type_declarations)  // надо в visit и самому всё обрабатывать
            {
                var tds = st is type_declarations;
                Println("type ");
                off += Indent;
            }
            else if (st is case_variant)
            {
                var td = st as case_variant;
                Println(td.conditions.ToString() + ": ");
                off += Indent;
            }
            else if (st is type_declaration)
            {
                var td = st as type_declaration;
                Print(td.type_name + " = ");
                ProcessNode(td.type_def);
                visitNode = false;
            }
            else if (st is class_definition)
            {
                var cd = st as class_definition;
                PrintNoOffset("class");
                if (cd.class_parents != null && cd.class_parents.types.Count > 0)
                {
                    PrintlnNoOffset("(" + cd.class_parents.ToString() + ")");
                }
                else
                    PrintlnNoOffset("");
                off += Indent;
                ProcessNode(cd.body);
                visitNode = false;
            }
            else if (st is access_modifer_node)
            {
                var am = st as access_modifer_node;
                Println(am.access_level.ToString());
            }
            else if (st is variable_definitions)
            {
                var vds = st as variable_definitions;
                Println("var");
                off += Indent;
            }
            else if (st is var_def_statement)
            {
                var vds = st as var_def_statement;
                Println(vds.ToString());
            }
            else if (st is empty_statement || st is declarations || st is block || st is class_body || st is class_members || st is case_variants || st is program_module)
            {

            }
            else if (st is if_node)
            {
                var ifn = st as if_node;

                Println("if " + ifn.condition.ToString() + " then");
                off += Indent;
                ProcessNode(ifn.then_body);
                if (ifn.else_body == null)
                {
                    visitNode = false;
                    return;
                }
                off -= Indent;
                Println("else ");
                off += Indent;
                ProcessNode(ifn.else_body);
                visitNode = false;
            }
            else if (st is while_node)
            {
                var wn = st as while_node;

                Println("while " + wn.expr.ToString() + " do");
                off += Indent;
            }
            else if (st is case_node)
            {
                var cn = st as case_node;

                Println("case " + cn.param.ToString() + " of");
                off += Indent;
            }
            else if (st is labeled_statement)
            {
                var lst = st as labeled_statement;

                Println(lst.label_name.name + ": ");
                ProcessNode(lst.to_statement);
                visitNode = false;
            }
            else if (st is var_statement || st is procedure_header)
            {
                Println(st.ToString());
                visitNode = false;
            }
            else if (st is procedure_definition)
            {
                Println("");
            }
            else if (st is statement || st is variable_definitions || st is label_definitions)
            {
                PrintlnNode(st);
                if (st is labeled_statement)
                    visitNode = false;                    
            }
            else if (st is token_info)
            {
                var s = st.ToString();
                if (s == "begin" || s == "end")
                    return;
                PrintlnNode(st);
            }
            else
            {
                var q = st.GetType().GetMethod("ToString", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                if (q == null)
                    Println(st.GetType().Name);
            }
        }

        public virtual void Exit(syntax_tree_node st)
        {
            if (st is statement_list)
            {
                off -= Indent;
                Println("end");
            }
            else if (st is while_node || st is if_node || st is type_declarations || st is variable_definitions || st is case_variant)
            {
                off -= Indent;
            }
            else if (st is class_definition || st is case_node)
            {
                off -= Indent;
                Println("end;");
            }
        }
    }
}
