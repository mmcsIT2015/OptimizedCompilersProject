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
            ifn.then_body = new empty_statement();
            ifn.else_body = new empty_statement();
        }
    }

    public class SyntaxTreeChanger : ISyntaxTreeChanger
    {
        public iCompiler.ThreeAddrCode Code { get; protected set; }
        public PrintVisitor Printer { get; protected set; }

        public void Change(syntax_tree_node sn)
        {
            sn.visit(new LoweringVisitor());
            var generator = new Gen3AddrCodeVisitor();
            sn.visit(generator);

            try
            {
                Code = generator.CreateCode();
                //Console.WriteLine("\nbefore:\n---");
                //Console.WriteLine(Code);

                var optimizer = new iCompiler.Optimizer();
                optimizer.AddOptimization(new iCompiler.DraggingConstantsOptimization());
                optimizer.AddOptimization(new iCompiler.CommonSubexpressionsOptimization());
                //optimizer.AddOptimization(new iCompiler.ConstantsPropagationOptimization());
                optimizer.AddOptimization(new iCompiler.ReachExprOptimization());
                optimizer.AddOptimization(new iCompiler.ActiveVarsOptimization());
                optimizer.AddOptimization(new iCompiler.ConstantFolding());
                optimizer.Assign(Code);
                optimizer.Optimize();

                //Console.WriteLine("\nafter:\n---");
                //Console.WriteLine(Code);

                //Console.WriteLine("\ntest:\n---");
                PascalABCTreeGenerator gen = new PascalABCTreeGenerator();
                var program_block = gen.Generate(Code);
                (sn as program_module).program_block = program_block;
                
                Printer = new PrintVisitor();
                sn.visit(Printer);
            }
            catch (SemanticException e)
            {
                Console.WriteLine("Semantic error: " + e.Message);
            }
        }
    }
}
