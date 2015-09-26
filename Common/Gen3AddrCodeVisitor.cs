using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    class Gen3AddrCodeVisitor : IVisitor
    {
        public ThreeAddrCode Code { get; set; }

        public void Visit(BlockNode node)
        {
        }

        public void Visit(AssignNode node)
        {
        }

        public void Visit(IfNode node)
        {
        }

        public void Visit(BinaryNode node)
        {
        }

        public void Visit(CoutNode node)
        {
        }

        public void Visit(WhileNode node)
        {
        }

        public void Visit(DoWhileNode node)
        {
        }
    }
}
