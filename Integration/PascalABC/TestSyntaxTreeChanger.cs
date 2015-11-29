using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PascalABCCompiler;
using PascalABCCompiler.SyntaxTree;

using SyntaxVisitors;

namespace ParsePABC1
{
    public class IfChangerVisitor : BaseChangeVisitor
    {
        public override void visit(if_node ifn)
        {

        }
    }

    public class TestVisitor : BaseChangeVisitor
    {
        public override void visit(ident ifn)
        {
            Console.WriteLine("ident> " + ifn.name);
        }
        public override void visit(int32_const ifn)
        {
            ifn.val = 56;
            Console.WriteLine("val> " + ifn.val);
        }
    }

    public class TestSyntaxTreeChanger : ISyntaxTreeChanger
    {
        public void Change(syntax_tree_node sn)
        {
            sn.visit(new SimplePrettyPrinterVisitor());

            sn.visit(new LoweringVisitor());
            //sn.visit(new TestVisitor());

            Console.WriteLine("\nafter:\n---");
            sn.visit(new SimplePrettyPrinterVisitor());
        }
    }
}
