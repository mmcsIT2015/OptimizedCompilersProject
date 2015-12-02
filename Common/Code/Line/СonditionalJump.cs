﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace iCompiler.Line
{
    public class СonditionalJump : GoTo
    {
        public string condition;

        public СonditionalJump(string condition, string target):
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
