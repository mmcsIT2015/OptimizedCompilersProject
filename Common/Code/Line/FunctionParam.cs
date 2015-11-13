using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace Compiler.Line
{
    class FunctionParam : NonEmptyLine
    {
        public string param;

        public FunctionParam(string param)
        {
            this.param = param;
        }
    }
}
