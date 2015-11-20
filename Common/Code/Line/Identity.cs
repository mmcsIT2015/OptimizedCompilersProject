using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;

namespace Compiler.Line
{
    class Identity : NonEmptyLine
    {
        public string left;
        public string right;

        // Конструктор для создания тождества
        public Identity(string left, string right)
        {
            this.left = left;
            this.right = right;
        }

        public virtual bool RightIsNumber() // является параметр правой части числом
        {
            double temp;
            return double.TryParse(right, out temp);
        }

        public override string ToString()
        {
            return left + " = " + right;
        }
    }
}
