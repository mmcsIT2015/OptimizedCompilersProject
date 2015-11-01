using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang.New
{
    class Block : List<Line.Line>
    {
        //private List<HashSet<string>> mDefUseData; //TODO

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var line in this)
            {
                if (line.HasLabel()) builder.Append(line.label + ":");
                builder.Append('\t', 1);

                if (line is Line.СonditionalJump)
                {
                    var jump = line as Line.СonditionalJump;
                    builder.Append("if " + jump.condition + " goto " + jump.target + "\n");
                }
                else if (line is Line.GoTo)
                {
                    builder.Append("goto " + (line as Line.GoTo).target + "\n");
                }
                else if (line is Line.FunctionParam)
                {
                    builder.Append("goto " + (line as Line.FunctionParam).param + "\n");
                }
                else if (line is Line.FunctionCall)
                {
                    var call = line as Line.FunctionCall;
                    if (call.IsVoid()) builder.Append("call " + call.name + ", " + call.parameters + "\n");
                    else builder.Append(call.destination + " = call " + call.name + ", " + call.parameters + "\n");
                }
                else
                {
                    if (line.IsEmpty())
                    {
                        builder.Append("<empty statement>\n");
                    }
                    else
                    {
                        var binary = line as Line.Operation;

                        builder.Append(binary.left + " = " + binary.first + " ");
                        builder.Append((binary.IsIdentity() ? "" : binary.operation.ToString() + " ") + binary.second + "\n");
                    }
                }
            }

            builder.Replace("  ", " ");

            return builder.ToString();
        }
    };
}
