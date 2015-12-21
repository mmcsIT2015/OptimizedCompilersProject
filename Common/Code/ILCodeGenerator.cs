using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ProgramTree;

namespace iCompiler
{
    public static class ILCodeGenerator
    {
        private const string ReportMessage = "[Report to the team SourceCode]";

        struct OperandInfo
        {
            public bool isConstant;
            public bool isFloat;
        }

        struct FunctionParamInfo
        {
            public string param;
            public SimpleVarType type;
            public OperandInfo op;
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
                sop.isFloat = sop.isConstant ? !be.SecondParamIsIntNumber() : (code.tableOfNames[be.second] == SimpleVarType.Float);

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

            OperandInfo aop = new OperandInfo(), rop = new OperandInfo();
            aop.isConstant = !code.tableOfNames.ContainsKey(ue.argument);
            aop.isFloat = aop.isConstant ? !ue.ArgIsIntNumber() : (code.tableOfNames[ue.argument] == SimpleVarType.Float);

            rop.isFloat = code.tableOfNames[ue.left] == SimpleVarType.Float;
            sb.Append(GenOperandLoading(ue.argument, aop, rop));

            if (ue.operation == Operator.Minus)
                sb.AppendLine("\tneg");
            else if (ue.operation == Operator.Not)
                sb.AppendLine("\tnot");
            else
                sb.AppendLine(ReportMessage + ": GenUnaryExpr - UnsupportedUnaryExpr");

            sb.AppendLine("\tstloc " + ue.left);

            return sb.ToString();
        }

        private static string GenIdentity(ThreeAddrCode code, Line.Identity id)
        {
            StringBuilder sb = new StringBuilder();

            OperandInfo op = new OperandInfo(), rop = new OperandInfo();
            op.isConstant = !code.tableOfNames.ContainsKey(id.right);
            op.isFloat = op.isConstant ? !id.RightIsIntNumber() : (code.tableOfNames[id.right] == SimpleVarType.Float);

            rop.isFloat = code.tableOfNames[id.left] == SimpleVarType.Float;
            sb.Append(GenOperandLoading(id.right, op, rop));

            sb.AppendLine("\tstloc " + id.left);

            return sb.ToString();
        }

        private static string GenConditionalJump(ThreeAddrCode code, Line.ConditionalJump cj)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("\tldloc " + cj.condition);
            sb.AppendLine("\tbrtrue " + cj.target);

            return sb.ToString();
        }

        private static string GenGoTo(ThreeAddrCode code, Line.GoTo gt)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("\tbr " + gt.target);

            return sb.ToString();
        }

        private static FunctionParamInfo ProcessFunctionParam(ThreeAddrCode code, Line.FunctionParam par)
        {
            FunctionParamInfo res = new FunctionParamInfo();
            res.param = par.param;
            res.op = new OperandInfo();
            res.op.isConstant = !code.tableOfNames.ContainsKey(par.param);
            res.op.isFloat = res.op.isConstant ? !par.ParamIsIntNumber() : (code.tableOfNames[par.param] == SimpleVarType.Float);
            res.type = res.op.isConstant ? (res.op.isFloat ? SimpleVarType.Float : SimpleVarType.Int) : code.tableOfNames[par.param];
            return res;
        }

        private static string GenFunctionCall(ThreeAddrCode code, Line.FunctionCall fc, List<FunctionParamInfo> pars)
        {
            StringBuilder sb = new StringBuilder();

            /*
            StringBuilder pars_sb = new StringBuilder();

            for (int i = 0; i < pars.Count; ++i)
            {
                OperandInfo rop = new OperandInfo();
                rop.isFloat = pars[i].op.isFloat;
                sb.Append(GenOperandLoading(pars[i].param, pars[i].op, rop));
                pars_sb.Append(Type2String(pars[i].type) + ", ");
            }
            */

            if (fc.name.Equals("cout"))
            {
                for (int i = 0; i < pars.Count; ++i)
                    if (pars[i].param.Equals("endl"))
                        sb.AppendLine("\tcall void [mscorlib]System.Console::WriteLine()");
                    else
                    {
                        OperandInfo rop = new OperandInfo();
                        rop.isFloat = pars[i].op.isFloat;
                        sb.Append(GenOperandLoading(pars[i].param, pars[i].op, rop));
                        sb.AppendLine("\tcall void [mscorlib]System.Console::Write(" + Type2String(pars[i].type) + ")");
                    }
            }
            else
                sb.AppendLine(ReportMessage + ": GenFunctionalCall - UnsupportedFunction");
            
            return sb.ToString();
        }

        public static string Generate(ThreeAddrCode code)
        {
            StringBuilder sb = new StringBuilder(), sb_header = new StringBuilder();
            List<FunctionParamInfo> pars = new List<FunctionParamInfo>();

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
                        sb.Append(GenUnaryExpr(code, l as Line.UnaryExpr));
                    else if (l.Is<Line.Identity>())
                        sb.Append(GenIdentity(code, l as Line.Identity));
                    else if (l.Is<Line.ConditionalJump>())
                        sb.Append(GenConditionalJump(code, l as Line.ConditionalJump));
                    else if (l.Is<Line.GoTo>())
                        sb.Append(GenGoTo(code, l as Line.GoTo));
                    else if (l.Is<Line.FunctionParam>())
                        pars.Add(ProcessFunctionParam(code, l as Line.FunctionParam));
                    else if (l.Is<Line.FunctionCall>())
                    {
                        sb.Append(GenFunctionCall(code, l as Line.FunctionCall, pars));

                        if ((l as Line.FunctionCall).name.Equals("cout"))
                            sb_header.AppendLine(".assembly extern mscorlib {}");

                        pars.Clear();
                    }
                    else
                        sb.AppendLine(ReportMessage + ": GenILCode - UnsupportedLine");
                }
            
            return sb_header.Append(AddRim(code, sb.ToString())).ToString();
        }
    }
}
