using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Globalization;

namespace iCompiler.Line
{
    public class ConditionalJump : GoTo
    {
        public string condition;

        public ConditionalJump(string condition, string target):
            base(target)
        {
            this.condition = condition;
        }

        public virtual bool ConditionIsNumber() // является ли условие числом
        {
            double temp;
            var style = System.Globalization.NumberStyles.Any;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            return double.TryParse(condition, style, nfi, out temp);
        }

        public virtual bool ConditionIsIntNumber() // является ли условиие целым числом
        {
            int temp;
            return int.TryParse(condition, out temp);
        }

        public override string ToString()
        {
            return "if " + condition + " goto " + target;
        }
    }
}
