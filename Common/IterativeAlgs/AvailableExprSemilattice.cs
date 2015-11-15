using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class AvailableExprSemilattice : IntersectSemilattice<AvailableExprSemilattice.Expression>
    {
        // TODO что-то тут не так)
        public class Expression// : IEqualityComparer<Expression>
        {
            string first;
            ProgramTree.BinaryOperation op;
            string second;

            public Expression(string first, ProgramTree.BinaryOperation op, string second)
            {
                this.first = first;
                this.op = op;
                this.second = second;
            }

            //public override bool Equals(object obj) {
            //    if (obj is Expression) {
            //        var other = obj as Expression;
            //        return other.first == first && other.op == op && other.second == second;
            //    }

            //    return false;
            //}

            //public int GetHashCode(Expression obj)
            //{
            //    return (obj.first + obj.second + obj.op.ToString()).GetHashCode();
            //}
        }

        public AvailableExprSemilattice(ThreeAddrCode code):
            base(new HashSet<AvailableExprSemilattice.Expression>())
        {
            foreach (var block in code.blocks)
            {
                foreach (var line in block)
                {
                    if (line.IsNot<Line.Operation>()) continue;

                    var expr = line as Line.Operation;
                    mTop.Add(new Expression(expr.first, expr.operation, expr.second));
                }
            }
        }
    }
}
