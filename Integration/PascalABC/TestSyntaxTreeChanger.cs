using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompilerExceptions;
using System.Threading.Tasks;

using PascalABCCompiler;
using PascalABCCompiler.SyntaxTree;

using SyntaxVisitors;

namespace ParsePABC
{
    public class IfChangerVisitor : BaseChangeVisitor
    {
        public override void visit(if_node ifn)
        {

        }
    }

    public class TestSyntaxTreeChanger : ISyntaxTreeChanger
    {
        public void Change(syntax_tree_node sn)
        {
            sn.visit(new LoweringVisitor());

            Console.WriteLine("\nlowering:\n---");
            sn.visit(new SimplePrettyPrinterVisitor());

            var generator = new Gen3AddrCodeVisitor();
            sn.visit(generator);

            Console.WriteLine("\ncode:\n---");
            try
            {
                var code = generator.CreateCode();
                Console.WriteLine(code);
                Console.WriteLine("\nvars:\n---");
                Console.WriteLine(code.TableOfNamesToString());

                Console.WriteLine("\ntest:\n---");
                PascalABCTreeGenerator gen = new PascalABCTreeGenerator();
                sn = gen.generate(code);
                sn.visit(new SimplePrettyPrinterVisitor());
            }
            catch (SemanticException e)
            {
                Console.WriteLine("Semantic error: " + e.Message);
            }
        }
    }
}
