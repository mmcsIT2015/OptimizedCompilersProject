using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;
using System.Globalization;

namespace iCompiler.Line
{
    public class BinaryExpr : Expr
    {
        private static ISet<Operator> mBoolOps = new HashSet<Operator> {
            Operator.Equal,
            Operator.Less,
            Operator.LessEqual,
            Operator.Greater,
            Operator.GreaterEqual,
            Operator.NotEqual,
        };

        private static ISet<Operator> mArithmOps = new HashSet<Operator> {
            Operator.Plus,
            Operator.Minus,
            Operator.Div,
            Operator.Mult
        };

        public string first;
        public string second;
        public Operator operation;

        // Конструктор для бинарного выражения в правой части
        public BinaryExpr(string left, string first, Operator op, string second) :
            base(left)
        {
            this.first = first;
            this.operation = op;
            this.second = second;
        }

        public virtual bool IsBoolExpr() // является ли строка логическим выражением (операции <= >=, <, >, !=)
        {
            return mBoolOps.Contains(operation);
        }

        public virtual bool IsArithmExpr() // является ли строка арифметическим выражением (операции +,-,*,/)
        {
            return mArithmOps.Contains(operation);
        }

        public virtual bool FirstParamIsNumber() // является ли первый параметр правой части числом
        {
            double temp;
            var style = System.Globalization.NumberStyles.Any;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            return double.TryParse(first, style, nfi, out temp);
        }

        public virtual bool SecondParamIsNumber() // является ли второй параметр правой части числом
        {
            double temp;
            var style = System.Globalization.NumberStyles.Any;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            return double.TryParse(second, style, nfi, out temp);
        }

        public virtual bool FirstParamIsIntNumber() // является ли первый параметр правой части целым числом
        {
            int temp;
            return int.TryParse(first, out temp);
        }

        public virtual bool SecondParamIsIntNumber() // является ли второй параметр правой части целым числом
        {
            int temp;
            return int.TryParse(second, out temp);
        }

        private static string ToString(Operator operation)
        {
            switch (operation)
            {
                case Operator.Div: return "/";
                case Operator.Equal: return "==";
                case Operator.Greater: return ">";
                case Operator.GreaterEqual: return ">=";
                case Operator.Less: return "<";
                case Operator.LessEqual: return "<=";
                case Operator.Minus: return "-";
                case Operator.Mult: return "*";
                case Operator.None: return "";
                case Operator.NotEqual: return "!=";
                case Operator.Plus: return "+";
                default: Debug.Assert(false, "[Not implemented]");  return "";
            }
        }

        public override string ToString()
        {
            return left + " = " + first + " " + ToString(operation) + " " + second;
        }

        public override bool IsEqualRightSide(Expr expr)
        {
            if (expr.IsNot<BinaryExpr>()) return false;

            var bin = expr as BinaryExpr;
            return bin.first == first && bin.second == second && bin.operation == operation;
        }
    }
}
