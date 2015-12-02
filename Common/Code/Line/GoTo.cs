﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace iCompiler.Line
{
    public class GoTo : NonEmptyLine
    {
        public string target;

        public GoTo(string target) {
            this.target = target;
        }

        public override void ChangeTargetIfEqual(string check, string forWhat)
        {
            if (target == check) target = forWhat;
        }

        public override string ToString()
        {
            return "goto " + target;
        }
    }
}
