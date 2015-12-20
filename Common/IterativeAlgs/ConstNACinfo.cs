﻿using System;
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
            return c1.VarName == c2.VarName;
        }

        public int GetHashCode(ConstNACInfo c)
        {
            return c.VarName.GetHashCode();
        }
    }

    public class ConstNACInfo
    {
        //Состояние переменной
        public VariableConstType mType = VariableConstType.UNDEFINED;

        //Хранит список выражений, в которых текущей переменной что-то присваивается
        public List<Line.Expr> mAssigns;

        //Имя переменной
        private string mVName;

        public string VarName { get { return mVName; } }// get { return mAssigns.First().left; } }

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
                    (mAssigns.First() as Identity).right : "";
            }
        }
        /* public bool Equal(ConstNACInfo cInfo)
         {
             return this.VarName == cInfo.VarName;
         }
         */
        public ConstNACInfo(VariableConstType vType, string vName, IEnumerable<Line.Expr> assigns)
        {
            mVName = vName;
            mType = vType;
            mAssigns = assigns.ToList<Line.Expr>();
            checkVariable();
        }

        /// <summary>
        /// Пытается выполнить арифметическую операцию в бинарном выражении. В случае успеха возвращает Identity, 
        /// иначе - BinaryExpr
        /// </summary>
        private Expr doOperation(BinaryExpr line)
        {
            if (!line.FirstParamIsNumber() || !line.SecondParamIsNumber() || !line.IsArithmExpr())
                return line;
            double x = double.Parse(line.first);
            double y = double.Parse(line.second);
            switch (line.operation)
            {
                case Operator.Minus:
                    return new Identity(line.left, (x - y).ToString());
                case Operator.Plus:
                    return new Identity(line.left, (x + y).ToString());
                case Operator.Mult:
                    return new Identity(line.left, (x * y).ToString());
                case Operator.Div:
                    return new Identity(line.left, (x / y).ToString());
                default:
                    return line;
            }
        }

        /// <summary>
        /// Проверяет, не стала ли переменная CONST или NAC
        /// </summary>
        private bool checkVariable()
        {
            string currentConstValue = "";
            foreach (var e in mAssigns)
            {
                if (e.IsNot<Identity>())
                    return false;
                if (!(e as Identity).RightIsNumber())
                    return false;
                if ((e as Identity).right != currentConstValue && currentConstValue != "")
                {
                    this.mType = VariableConstType.NOT_A_CONSTANT;
                    return false;
                }
                currentConstValue = (e as Identity).right;
            }
            this.mType = VariableConstType.CONSTANT;
            this.mAssigns.Insert(0, new Identity(this.VarName, currentConstValue));
            return true;
        }


        /// <summary>
        /// Заменяет переменную на ее константное значение
        /// </summary>
        public void replaceConsts(IEnumerable<ConstNACInfo> varsInfo)
        {
            List<Line.Expr> assigns = new List<Line.Expr>();

            foreach (var e in mAssigns)
            {
                //Проходим по всем бинарным выражениям
                if (e.Is<BinaryExpr>())
                {
                    string fstVal = (e as BinaryExpr).first;
                    string sndVal = (e as BinaryExpr).second;
                    var tmpFst = varsInfo.First<ConstNACInfo>((e as BinaryExpr).first.Equals);
                    var tmpSnd = varsInfo.First<ConstNACInfo>((e as BinaryExpr).second.Equals);

                    //Заменяем входящие в правую часть константы, если возможно
                    if (tmpFst != null)
                    {
                        if (tmpFst.mType == VariableConstType.CONSTANT)
                            fstVal = tmpFst.ConstVal;
                    }
                    if (tmpSnd != null)
                    {
                        if (tmpSnd.mType == VariableConstType.CONSTANT)
                            sndVal = tmpFst.ConstVal;
                    }
                    //Пытаемся выполнить арифметическую операцию
                    Expr newExpr = doOperation(new BinaryExpr(e.left, fstVal, (e as BinaryExpr).operation, sndVal));

                    assigns.Add(newExpr);
                }
                //Если выражение является тождеством
                else if (e.Is<Identity>())
                {
                    var tmpFst = varsInfo.First<ConstNACInfo>((e as Identity).right.Equals);
                    if (tmpFst != null)
                        assigns.Add(tmpFst.mAssigns.First());
                    else
                        assigns.Add(e);
                }
            }
            checkVariable();
            this.mAssigns = assigns;
        }

        private static VariableConstType Max(VariableConstType t1, VariableConstType t2)
        {
            return t1 < t2 ? t2 : t1;
        }

        public static ConstNACInfo MaxLowerBound(ConstNACInfo v1, ConstNACInfo v2)
        {
            if (v1 == null)
                return v2;
            else if (v2 == null)
                return v1;
            if ((v1.mType == VariableConstType.CONSTANT && v1.mType == v2.mType && v1.ConstVal == v2.ConstVal)
                   || (v1.mType == VariableConstType.CONSTANT && v2.mType == VariableConstType.UNDEFINED))
                return new ConstNACInfo(VariableConstType.CONSTANT, v1.VarName, v1.mAssigns.Union(v2.mAssigns));

            else if (v1.mType == VariableConstType.CONSTANT && v1.mType == v2.mType && v1.ConstVal != v2.ConstVal)
                return new ConstNACInfo(VariableConstType.NOT_A_CONSTANT, v1.VarName, v1.mAssigns);

            else if (v1.mType == VariableConstType.UNDEFINED && v2.mType == VariableConstType.CONSTANT)
                return new ConstNACInfo(VariableConstType.CONSTANT, v2.ConstVal, v2.mAssigns.Union(v1.mAssigns));

            else
                return new ConstNACInfo(Max(v1.mType, v2.mType), v1.VarName, v1.mAssigns.Union(v2.mAssigns));
        }

        public override string ToString()
        { 
            var builder = new StringBuilder();
            builder.Append(mVName + " (" + mType.ToString() + "): \r\n");
            foreach (var el in mAssigns)
                builder.Append(el.ToString());
            return builder.ToString();
        }
    }
}
