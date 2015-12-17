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

    public class SyntaxTreeChanger : ISyntaxTreeChanger
    {
        public iCompiler.ThreeAddrCode Code { get; protected set; }

        public void Change(syntax_tree_node sn)
        {
            sn.visit(new LoweringVisitor());

            Console.WriteLine("\nlowering:\n---");
            sn.visit(new SimplePrettyPrinterVisitor());

            var generator = new Gen3AddrCodeVisitor();
            sn.visit(generator);

            Console.WriteLine("\ncode:");
            try
            {
                Code = generator.CreateCode();
                Console.WriteLine("\nbefore:\n---");
                Console.WriteLine(Code);

                var optimizer = new iCompiler.Optimizer();
                optimizer.AddOptimization(new iCompiler.DraggingConstantsOptimization());
                optimizer.AddOptimization(new iCompiler.ReachExprOptimization());
                optimizer.AddOptimization(new iCompiler.ActiveVarsOptimization());
                optimizer.AddOptimization(new iCompiler.ConstantFolding());
                optimizer.Assign(Code);
                optimizer.Optimize();

                Console.WriteLine("\nafter:\n---");
                Console.WriteLine(Code);

                Console.WriteLine("\ntest:\n---");
                PascalABCTreeGenerator gen = new PascalABCTreeGenerator();
                sn = gen.generate(Code);
                sn.visit(new SimplePrettyPrinterVisitor());
            }
            catch (SemanticException e)
            {
                Console.WriteLine("Semantic error: " + e.Message);
            }
        }
    }
}
