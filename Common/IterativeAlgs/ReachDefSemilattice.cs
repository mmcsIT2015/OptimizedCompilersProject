using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class ReachDefSemilattice : UnionSemilattice<ThreeAddrCode.Index>
    {
        public ReachDefSemilattice(ThreeAddrCode code)
        {

        }
    }
}
