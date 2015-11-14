using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;

namespace Compiler.Line
{
    class Operation : NonEmptyLine
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
        public Operation(string left, string first, BinaryOperation op, string second)
        {
            this.left = left;
            this.first = first;
            this.operation = op;
            this.second = second;
        }

        // Конструктор для создания тождества
        public Operation(string left, string first)
        {
            this.left = left;
            this.first = first;

            this.second = "";
            this.operation = BinaryOperation.None;
        }

        public virtual bool IsUnary() //на будущее
        {
            return false;
        }

        public virtual bool IsIdentity() // является ли строка тождеством (a = b)
        {
            return second == "" && operation == BinaryOperation.None;
        }

        public virtual bool IsBoolExpr() // является ли строка арифметическим выражением (операции +,-,*,/)
        {
            return mBoolOps.Contains(operation);
        }

        public virtual bool IsArithmExpr() // является ли строка логическим выражением (операции <= >=, <, >, !=)
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

        /// Преобразует правую часть выражения в указанное значение
        /// (т.е. преобразует линию в тождество).
        /// Пример:
        /// line = `a = b + c`
        /// line.ToIdentity("z") -> `a = z`
        public void ToIdentity(string value)
        {
            first = value;
            second = "";
            operation = BinaryOperation.None;
        }

        private static string toString(BinaryOperation operation)
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
            return left + " = " + first + " " + (IsIdentity() ? "" : toString(operation) + " ") + second + "\n";
        }
    }
}
