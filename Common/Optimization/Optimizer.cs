using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iCompiler
{
    public class Optimizer : IOptimizer
    {
        protected List<IOptimizer> mOptimizations = new List<IOptimizer>();

        public Optimizer(params IOptimizer[] optimizations)
        {
            foreach (var o in optimizations)
            {
                mOptimizations.Add(o);
            }
        }

        public void AddOptimization(IOptimizer optimization)
        {
            mOptimizations.Add(optimization);
        }

        public override void Optimize(params Object[] values)
        {
            Debug.Assert(Code != null);

            do
            {
                NumberOfChanges = 0;
                foreach (var o in mOptimizations)
                {
                    o.Assign(Code);
                    o.Optimize(values);
                    NumberOfChanges += o.NumberOfChanges;
                }
            } while (NumberOfChanges > 0);
        }
    }
}
