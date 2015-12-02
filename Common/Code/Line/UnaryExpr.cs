using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;

namespace iCompiler.Line
{
    public class UnaryExpr : Expr
    {
        public Operator operation;
        public string argument;

        // Конструктор для унарного выражения
        public UnaryExpr(string left, Operator op, string arg) :
            base(left)
        {
            this.operation = op;
            this.argument = arg;
        }

        public virtual bool IsBoolExpr() // является ли строка логическим выражением (!b)
        {
            return operation == Operator.Not;
        }

        public virtual bool IsArithmExpr() // является ли строка арифметическим выражением (-b)
        {
            return operation == Operator.Minus;
        }

        public virtual bool ParamIsNumber() // является ли параметр числом
        {
            double temp;
            return double.TryParse(argument, out temp);
        }

        private static string ToString(Operator operation)
        {
            switch (operation)
            {
                case Operator.Minus: return "-";
                case Operator.Not: return "!";
                default: Debug.Assert(false, "[Not implemented]");  return "";
            }
        }

        public override string ToString()
        {
            return left + " = " +  ToString(operation) + argument;
        }
    }
}
