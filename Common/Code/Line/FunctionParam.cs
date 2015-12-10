using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Globalization;

namespace iCompiler.Line
{
    public class FunctionParam : NonEmptyLine
    {
        public string param;

        public FunctionParam(string param)
        {
            this.param = param;
        }

        public virtual bool ParamIsNumber() // является параметр числом
        {
            double temp;
            var style = System.Globalization.NumberStyles.Any;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            return double.TryParse(param, style, nfi, out temp);
        }

        public virtual bool ParamIsIntNumber() // является ли параметр целым числом
        {
            int temp;
            return int.TryParse(param, out temp);
        }

        public override string ToString()
        {
            return "param " + param;
        }
    }
}
