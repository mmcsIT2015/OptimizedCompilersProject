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

            var varNames = lhs.Select(el => el.VarName).Union(rhs.Select(el => el.VarName));

            foreach (var v in varNames)
            {
                ConstNACInfo tmpL = lhs.FirstOrDefault(el => el.VarName.Equals(v));
                ConstNACInfo tmpR = rhs.FirstOrDefault(el => el.VarName.Equals(v));
                resL.Add(ConstNACInfo.MaxLowerBound(tmpL, tmpR));
            }
            return resL;
        }

        public IEnumerable<ConstNACInfo> Top()
        {
            return mTop;
        }
    }
}
