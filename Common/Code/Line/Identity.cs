using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;
using System.Globalization;

namespace iCompiler.Line
{
    public class Identity : Expr
    {
        public string right;

        // Конструктор для создания тождества
        public Identity(string left, string right) :
            base(left)
        {
            this.right = right;
        }

        public virtual bool RightIsNumber() // является параметр правой части числом
        {
            double temp;
            var style = System.Globalization.NumberStyles.Any;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            return double.TryParse(right, style, nfi, out temp);
        }

        public override string ToString()
        {
            return left + " = " + right;
        }
    }
}
