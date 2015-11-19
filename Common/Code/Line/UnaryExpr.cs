using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;

namespace Compiler.Line
{
    class UnaryExpr : NonEmptyLine
    {
        public string left;
        public UnaryOperation operation;
        public string argument;

        // Конструктор для унарного выражения
        public UnaryExpr(string left, UnaryOperation op, string arg)
        {
            this.left = left;
            this.operation = op;
            this.argument = arg;
        }

        public virtual bool IsBoolExpr() // является ли строка логическим выражением (!b)
        {
            return operation == UnaryOperation.Not;
        }

        public virtual bool IsArithmExpr() // является ли строка арифметическим выражением (-b)
        {
            return operation == UnaryOperation.Minus;
        }

        public virtual bool ParamIsNumber() // является ли параметр числом
        {
            double temp;
            return double.TryParse(argument, out temp);
        }

        private static string ToString(UnaryOperation operation)
        {
            switch (operation)
            {
                case UnaryOperation.Minus: return "-";
                case UnaryOperation.Not: return "!";
                default: Debug.Assert(false, "[Not implemented]");  return "";
            }
        }

        public override string ToString()
        {
            return left + " = " +  ToString(operation) + argument;
        }
    }
}
