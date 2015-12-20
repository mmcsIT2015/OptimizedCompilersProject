using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCompiler.Line;
using ProgramTree;
namespace iCompiler
{


    class SemilatticeForDragingConsts : ISemilattice<ConstNACInfo>
    {

        private List<ConstNACInfo> mTop = new List<ConstNACInfo>();

        public SemilatticeForDragingConsts()
        {

        }

        
        public IEnumerable<ConstNACInfo> Join(IEnumerable<ConstNACInfo> lhs, IEnumerable<ConstNACInfo> rhs)
        {
            List<ConstNACInfo> resL = new List<ConstNACInfo>();
            var lrhs = lhs.Union(rhs);
            foreach(var e in lrhs)
            {
                var tmpr = rhs.First(e.NameEquals);
                var tmpl = rhs.First(e.NameEquals);
                resL.Add(ConstNACInfo.MaxLowerBound(tmpl, tmpr));
            }
            return resL;
        }

        public IEnumerable<ConstNACInfo> Top()
        {
            return mTop;
        }
    }
}
