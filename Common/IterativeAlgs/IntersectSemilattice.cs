using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class IntersectSemilattice<T> : ISemilattice<T>
    {
        protected HashSet<T> mTop;

        public IntersectSemilattice(HashSet<T> top)
        {
            mTop = top;
        }

        public IEnumerable<T> Join(IEnumerable<T> lhs, IEnumerable<T> rhs)
        {
            return lhs.Intersect(rhs);
        }

        public IEnumerable<T> Top()
        {
            return mTop;
        }
    }
}
