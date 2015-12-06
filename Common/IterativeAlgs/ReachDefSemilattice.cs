using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    class ReachableDefSemilattice : UnionSemilattice<ThreeAddrCode.Index>
    {
        public ReachableDefSemilattice(ThreeAddrCode code)
        {

        }
    }
}
