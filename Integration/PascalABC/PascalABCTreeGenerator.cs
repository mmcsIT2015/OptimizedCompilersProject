using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PascalABCCompiler.SyntaxTree;

namespace ParsePABC
{
    public class PascalABCTreeGenerator
    {
        public syntax_tree_node generate(iCompiler.ThreeAddrCode code)
        {
            block root = new block();
            root.defs = new declarations();
            //foreach (var name in code.tableOfNames.Keys)
            //{
                var decl = new var_def_statement();
                decl.vars = new ident_list();
                decl.vars.Add(new ident("i"));
                decl.vars.Add(new ident("j"));
                decl.vars_type = new named_type_reference("integer");
                root.defs.Add(decl);
            //}

            root.program_code = new statement_list();

            return root;
        }
    }
}
