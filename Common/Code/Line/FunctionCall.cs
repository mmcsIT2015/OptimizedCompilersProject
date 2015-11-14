using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace Compiler.Line
{
    class FunctionCall : NonEmptyLine
    {
        public string name;
        public int parameters; //число параметров
        public string destination; //возвращаемый параметр (если он есть)

        public FunctionCall(string name, int nparams, string dest = null)
        {
            this.name = name;
            this.parameters = nparams;
            this.destination = dest;
        }

        public virtual bool IsVoid() // есть ли у функции возвращаемый параметр
        {
            return destination == null || destination.Count() == 0;
        }

        public override string ToString()
        {
            if (IsVoid())
            {
                return "call " + name + ", " + parameters + "\n";
            }
            else
            {
                return destination + " = call " + name + ", " + parameters + "\n";
            }
        }
    }
}
