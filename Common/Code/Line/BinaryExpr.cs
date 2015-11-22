using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;

namespace Compiler.Line
{
    class BinaryExpr : NonEmptyLine
    {
        private static ISet<BinaryOperation> mBoolOps = new HashSet<BinaryOperation> {
            BinaryOperation.Equal,
            BinaryOperation.Less,
            BinaryOperation.LessEqual,
            BinaryOperation.Greater,
            BinaryOperation.GreaterEqual,
            BinaryOperation.NotEqual,
        };

        private static ISet<BinaryOperation> mArithmOps = new HashSet<BinaryOperation> {
            BinaryOperation.Plus,
            BinaryOperation.Minus,
            BinaryOperation.Div,
            BinaryOperation.Mult
        };

        public string left;
        public string first;
        public string second;
        public BinaryOperation operation;

        // Конструктор для бинарного выражения в правой части
        public BinaryExpr(string left, string first, BinaryOperation op, string second)
        {
            this.left = left;
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
            return double.TryParse(first, out temp);
        }

        public virtual bool SecondParamIsNumber() // является ли второй параметр правой части числом
        {
            double temp;
            return double.TryParse(second, out temp);
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

        private static string ToString(BinaryOperation operation)
        {
            switch (operation)
            {
                case BinaryOperation.Div: return "/";
                case BinaryOperation.Equal: return "==";
                case BinaryOperation.Greater: return ">";
                case BinaryOperation.GreaterEqual: return ">=";
                case BinaryOperation.Less: return "<";
                case BinaryOperation.LessEqual: return "<=";
                case BinaryOperation.Minus: return "-";
                case BinaryOperation.Mult: return "*";
                case BinaryOperation.None: return "";
                case BinaryOperation.NotEqual: return "!=";
                case BinaryOperation.Plus: return "+";
                default: Debug.Assert(false, "[Not implemented]");  return "";
            }
        }

        public override string ToString()
        {
            return left + " = " + first + " " + ToString(operation) + " " + second;
        }
    }
}
