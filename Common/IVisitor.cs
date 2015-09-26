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
        void Visit(AssignNode node);
        void Visit(IfNode node);
        void Visit(BinaryNode node);
        void Visit(CoutNode node);
        void Visit(WhileNode node);
        void Visit(DoWhileNode node);
    }
}
