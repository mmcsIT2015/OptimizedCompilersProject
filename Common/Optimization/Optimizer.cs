using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    class Optimizer : IOptimizer
    {
        protected List<IOptimizer> mOptimizations = new List<IOptimizer>();

        public Optimizer(params IOptimizer[] optimizations)
        {
            foreach (var o in optimizations)
            {
                o.Assign(Code);
                mOptimizations.Add(o);
            }
        }

        public void AddOptimization(IOptimizer optimization)
        {
            mOptimizations.Add(optimization);
            optimization.Assign(Code);
        }

        public override void Optimize(params Object[] values)
        {
            foreach (var o in mOptimizations)
            {
                o.Optimize(values);
            }
        }

        public override void Assign(ThreeAddrCode code)
        {
            Code = code;
            foreach (var o in mOptimizations)
            {
                o.Assign(code);
            }
        }
    }
}
