using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    class ReachableExprSemilattice : IntersectSemilattice<ReachableExprsGenerator.ExpressionWrapper>
    {
        public ReachableExprSemilattice(ThreeAddrCode code):
            base(new HashSet<ReachableExprsGenerator.ExpressionWrapper>())
        {
            foreach (Block block in code.blocks)
            {
                foreach (Line.Line line in block)
                {
                    if (line is Line.Expr)
                    {
                        var expr = line as Line.Expr;
                        mTop.Add(new ReachableExprsGenerator.ExpressionWrapper(expr));
                    }
                }
            }

        }
    }
}
