using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    public interface IVisitor
    {
        void Visit(BlockNode node);
        void Visit(VarNode node);
        void Visit(AssignNode node);
        void Visit(IfNode node);
        void Visit(CoutNode node);
        void Visit(WhileNode node);
        void Visit(DoWhileNode node);
        void Visit(BinaryNode node);
        void Visit(IdNode node);
        void Visit(IntNumNode node);
<<<<<<< HEAD
        void Visit(FloatNumNode node);
        void Visit(FunctionNodeSt node);
        void Visit(FunctionNode node);
        void Visit(UnaryNode node);
=======
        void Visit(FloatNumNode node);
        void Visit(FunctionNodeSt node);
        void Visit(FunctionNode node);
>>>>>>> 5ed01454fc05e4df8cd497e3fbd6fc8385e6541a
    }
}
