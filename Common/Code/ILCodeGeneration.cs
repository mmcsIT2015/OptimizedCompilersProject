using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ProgramTree;

namespace iCompiler
{
    public static class ILCodeGeneration
    {
        private const string ReportMessage = "[Report to the team SourceCode]";

        struct OperandInfo
        {
            public bool isConstant;
            public bool isFloat;
        }

        private static string Type2String(SimpleVarType type)
        {
            switch (type)
            {
                case SimpleVarType.Bool:
                    return "bool";
                case SimpleVarType.Float:
                    return "float32";
                case SimpleVarType.Int:
                    return "int32";
                default:
                    return "string";
            }
        }

        private static string AddRim(ThreeAddrCode code, string ilcode)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(".assembly program {}\n");
            sb.AppendLine(".method static public void main() cil managed");
            sb.AppendLine("{");
            sb.AppendLine("\t.entrypoint");
            sb.Append("\t.maxstack "); sb.AppendLine(code.tableOfNames.Count.ToString());
            if (code.tableOfNames.Count > 0)
            {
                sb.Append("\t.locals init (");
                List<string> vars = new List<string>(code.tableOfNames.Keys);
                sb.Append(Type2String(code.tableOfNames[vars[0]]) + " " + vars[0]);
                for (int i = 1; i < vars.Count; ++i)
                    sb.Append(", " + Type2String(code.tableOfNames[vars[i]]) + " " + vars[i]);
                sb.AppendLine(")");
            }
            sb.AppendLine();

            sb.Append(ilcode);

            sb.AppendLine("\tret");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenOperandLoading(string ops, OperandInfo op, OperandInfo rop)
        {
            StringBuilder sb = new StringBuilder();

            if (op.isConstant)
            {
                if (rop.isFloat)
                    sb.AppendLine("\tldc.r4 " + ops);
                else
                    sb.AppendLine("\tldc.i4 " + ops);
            }
            else
            {
                sb.AppendLine("\tldloc " + ops);
                if (rop.isFloat && !op.isFloat)
                    sb.AppendLine("\tconv.r4");
            }

            return sb.ToString();
        }

        private static string GenBinaryExpr(ThreeAddrCode code, Line.BinaryExpr be)
        {
            StringBuilder sb = new StringBuilder();
            if (be.IsArithmExpr() || be.IsBoolExpr())
            {
                OperandInfo fop = new OperandInfo(), sop = new OperandInfo(), rop = new OperandInfo();
                fop.isConstant = !code.tableOfNames.ContainsKey(be.first);
                fop.isFloat = fop.isConstant ? !be.FirstParamIsIntNumber() : (code.tableOfNames[be.first] == SimpleVarType.Float);
                sop.isConstant = !code.tableOfNames.ContainsKey(be.second);
                sop.isFloat = sop.isConstant ? !be.SecondParamIsIntNumber() : (code.tableOfNames[be.first] == SimpleVarType.Float);

                rop.isFloat = fop.isFloat || sop.isFloat;
                sb.Append(GenOperandLoading(be.first, fop, rop));
                sb.Append(GenOperandLoading(be.second, sop, rop));

                if (be.IsArithmExpr())
                {
                    if (be.operation == Operator.Plus)
                        sb.AppendLine("\tadd");
                    else if (be.operation == Operator.Minus)
                        sb.AppendLine("\tsub");
                    else if (be.operation == Operator.Mult)
                        sb.AppendLine("\tmul");
                    else if (be.operation == Operator.Div)
                        sb.AppendLine("\tdiv");
                    else
                        sb.AppendLine(ReportMessage + ": GenBinaryExpr - UnknownArithmExpr");
                }
                else if (be.IsBoolExpr())
                {
                    if (be.operation == Operator.Equal || be.operation == Operator.NotEqual)
                        sb.AppendLine("\tceq");
                    else if (be.operation == Operator.Less || be.operation == Operator.LessEqual)
                        sb.AppendLine("\tclt");
                    else if (be.operation == Operator.Greater || be.operation == Operator.GreaterEqual)
                        sb.AppendLine("\tcgt");
                    else
                        sb.AppendLine(ReportMessage + ": GenBinaryExpr - UnknownBoolExpr");

                    if (be.operation == Operator.NotEqual)
                        sb.AppendLine("\tnot");
                    else if (be.operation == Operator.LessEqual || be.operation == Operator.GreaterEqual)
                    {
                        sb.Append(GenOperandLoading(be.first, fop, rop));
                        sb.Append(GenOperandLoading(be.second, sop, rop));
                        sb.AppendLine("\tceq");
                        sb.AppendLine("\tor");
                    }
                }

                if (!rop.isFloat && code.tableOfNames[be.left] == SimpleVarType.Float)
                    sb.AppendLine("\tconv.r4");
                sb.AppendLine("\tstloc " + be.left);                
            }
            else
            {
                sb.AppendLine(ReportMessage + ": GenBinaryExpr - UnsupportedBinaryExpr");
            }
            return sb.ToString();
        }

        private static string GenUnaryExpr(ThreeAddrCode code, Line.UnaryExpr ue)
        {
            StringBuilder sb = new StringBuilder();



            if (ue.operation == Operator.Minus)
                sb.AppendLine("\tneg");
            else if (ue.operation == Operator.Not)
                sb.AppendLine("\tnot");
            else
                sb.AppendLine(ReportMessage + ": GenUnaryExpr - UnsupportedUnaryExpr");

            return sb.ToString();
        }

        public static string GenILCode(ThreeAddrCode code)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Block b in code.blocks)
                foreach (Line.Line l in b)
                {
                    if (!l.label.Equals(""))
                        sb.AppendLine(l.label + ":");

                    if (l.IsEmpty())
                        sb.AppendLine("\tnop");
                    else if (l.Is<Line.BinaryExpr>())
                        sb.Append(GenBinaryExpr(code, l as Line.BinaryExpr));
                    else if (l.Is<Line.UnaryExpr>())
                        sb.Append(GenUnaryExpr(code, l as Line.UnaryExpr);
                    else
                        sb.AppendLine(ReportMessage + ": GenILCode - UnsupportedLine");
                }
            
            return AddRim(code, sb.ToString());
        }
    }
}
