using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCompiler.Line;
using ProgramTree;
namespace iCompiler
{
    public enum VariableConstType { UNDEFINED, CONSTANT, NOT_A_CONSTANT };

    public class NameEqualsComparer : IEqualityComparer<ConstNACInfo>
    {
        public bool Equals(ConstNACInfo c1, ConstNACInfo c2)
        {
            bool res = c1.VarName == c2.VarName;
            return res;
        }

        public int GetHashCode(ConstNACInfo c)
        {
            return c.VarName.GetHashCode();
        }
    }

    public class ConstNACInfo : ICloneable
    {
        //Состояние переменной
        public VariableConstType mType;

        //Имя переменной
        private string mVName;

        private string mValue;
        public string VarName { get { return mVName; } }

        public bool NameEquals(ConstNACInfo c1)
        {
            return this.VarName == c1.VarName;
        }
        //Возвращает значение константы
        public string ConstVal
        {
            get
            {
                return mType == VariableConstType.CONSTANT ?
                    mValue : "";
            }
        }

        public Object Clone()
        {
            return new ConstNACInfo(this);
        }
        public override bool Equals(Object cInfo)
        {
            if (!(cInfo is ConstNACInfo))
                return false;
            ConstNACInfo c = cInfo as ConstNACInfo;
            return this.ToString() == c.ToString();
        }


        public ConstNACInfo(VariableConstType vType, string vName, string constVal = "") //, IEnumerable<Line.Expr> assigns)
        {
            mVName = vName;
            mType = vType;
            if (mType == VariableConstType.CONSTANT)
                mValue = constVal;
            else
                mValue = "";
        }

        public ConstNACInfo(ConstNACInfo c)
        {
            mVName = c.VarName;
            mType = c.mType;
            mValue = c.ConstVal;
        }

        private static VariableConstType Max(VariableConstType t1, VariableConstType t2)
        {
            return t1 < t2 ? t2 : t1;
        }

        public static ConstNACInfo MaxLowerBound(ConstNACInfo v1, ConstNACInfo v2)
        {
            if (v1 == null)
                return new ConstNACInfo(v2);
            else if (v2 == null)
                return new ConstNACInfo(v1);
            if ((v1.mType == VariableConstType.CONSTANT && v1.mType == v2.mType && v1.ConstVal == v2.ConstVal)
                   || (v1.mType == VariableConstType.CONSTANT && v2.mType == VariableConstType.UNDEFINED))
                return new ConstNACInfo(VariableConstType.CONSTANT, v1.VarName, v1.ConstVal);

            else if (v1.mType == VariableConstType.CONSTANT && v1.mType == v2.mType && v1.ConstVal != v2.ConstVal)
                return new ConstNACInfo(VariableConstType.NOT_A_CONSTANT, v1.VarName);

            else if (v1.mType == VariableConstType.UNDEFINED && v2.mType == VariableConstType.CONSTANT)
                return new ConstNACInfo(VariableConstType.CONSTANT, v2.VarName, v2.ConstVal);

            else
                return new ConstNACInfo(Max(v1.mType, v2.mType), v1.VarName);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(mVName);
            builder.Append(" (" + mType.ToString() + "): ");

            if (mType == VariableConstType.CONSTANT)
                builder.Append(ConstVal);
            return builder.ToString();
        }
    }
}
