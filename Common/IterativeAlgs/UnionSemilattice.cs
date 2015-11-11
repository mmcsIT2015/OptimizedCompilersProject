using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class UnionSemilattice<T> : ISemilattice<T>
    {
        private HashSet<T> mTop = new HashSet<T>(); // T - пустое множество

        public IEnumerable<T> Join(IEnumerable<T> lhs, IEnumerable<T> rhs)
        {
            return lhs.Union(rhs);
        }

        public IEnumerable<T> Top()
        {
            return mTop;
        }
    }
}
