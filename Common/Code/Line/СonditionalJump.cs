using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

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

        public override string ToString()
        {
            return "if " + condition + " goto " + target;
        }
    }
}
