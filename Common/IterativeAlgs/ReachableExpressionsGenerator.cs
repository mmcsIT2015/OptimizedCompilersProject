using System;
using System.Collections.Generic;
using System.Diagnostics;
using iCompiler.Line;
using iCompiler;

namespace iCompiler
{
    class ReachableExpressionsGenerator
    {
        private class ExpressionWrapper
        {
            private readonly Expr mExpressionLine;
            public ExpressionWrapper(Expr expression)
            {
                mExpressionLine = expression;
            }

            public static bool Equals(ExpressionWrapper expr1, ExpressionWrapper expr2)
            {
                if (expr1.mExpressionLine.Is<BinaryExpr>() && expr2.mExpressionLine.Is<BinaryExpr>())
                {
                    BinaryExpr binExpr1 = expr1.mExpressionLine as BinaryExpr;
                    BinaryExpr binExpr2 = expr2.mExpressionLine as BinaryExpr;

                    return binExpr1.first == binExpr2.first && binExpr1.operation == binExpr2.operation
                        && binExpr1.second == binExpr2.second;
                }
                else if (expr1.mExpressionLine.Is<UnaryExpr>() && expr2.mExpressionLine.Is<UnaryExpr>())
                {
                    UnaryExpr unaryExpr1 = expr1.mExpressionLine as UnaryExpr;
                    UnaryExpr unaryExpr2 = expr2.mExpressionLine as UnaryExpr;

                    return unaryExpr1.argument == unaryExpr2.argument && unaryExpr1.operation == unaryExpr2.operation;
                }
                else if (expr1.mExpressionLine.Is<Identity>() && expr2.mExpressionLine.Is<Identity>())
                {
                    Identity identExpr1 = expr1.mExpressionLine as Identity;
                    Identity identExpr2 = expr2.mExpressionLine as Identity;

                    return identExpr1.right == identExpr2.right;
                }

                return false;
            }

            public override bool Equals(object obj)
            {
                return (obj is ExpressionWrapper) && Equals(this, obj as ExpressionWrapper);
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public String GetRedifined()
            {
                if (mExpressionLine.Is<BinaryExpr>())
                {
                    return (mExpressionLine as BinaryExpr).left;
                }
                else if (mExpressionLine.Is<UnaryExpr>())
                {
                    return (mExpressionLine as UnaryExpr).left;
                }
                else if (mExpressionLine.Is<Identity>())
                {
                    return (mExpressionLine as Identity).left;
                }

                Debug.Assert(false, "[ ReachableExpressions ] GetRedifined");

                return "";
            }

            public bool IsRedifinedBy(HashSet<String> variables)
            {
                if (mExpressionLine.Is<BinaryExpr>())
                {
                    BinaryExpr expr = mExpressionLine as BinaryExpr;

                    return variables.Contains(expr.first) || variables.Contains(expr.second);
                }
                else if (mExpressionLine.Is<UnaryExpr>())
                {
                    UnaryExpr expr = mExpressionLine as UnaryExpr;

                    return variables.Contains(expr.argument);
                }
                else if (mExpressionLine.Is<Identity>())
                {
                    Identity expr = mExpressionLine as Identity;

                    return variables.Contains(expr.right);
                }

                Debug.Assert(false, "[ ReachableExpressions ] IsRedifinedBy");

                return false;
            }

            public Expr GetExpression()
            {
                return mExpressionLine;
            }
        }

        private class ExpressionGenKill
        {
            public HashSet<ExpressionWrapper> Gen = new HashSet<ExpressionWrapper>();
            public HashSet<ExpressionWrapper> Kill = new HashSet<ExpressionWrapper>();
        }

        private static List<Expr> FlushExprs(IEnumerable<ExpressionWrapper> rawExpressions)
        {
            List<Expr> result = new List<Expr>();

            foreach (ExpressionWrapper rawExpression in rawExpressions)
            {
                result.Add(rawExpression.GetExpression());
            }

            return result;
        }

        private static Dictionary<Block, ExpressionGenKill> BuildExpressionsGenKill(ThreeAddrCode code)
        {
            Dictionary<Block, ExpressionGenKill> exprGenKillList = new Dictionary<Block, ExpressionGenKill>();

            for (int i = 0; i < code.blocks.Count; ++i)
            {
                ExpressionGenKill blockGenKill = new ExpressionGenKill();
                exprGenKillList.Add(code.blocks[i], blockGenKill);

                HashSet<String> redifined = new HashSet<String>();

                for (int j = code.blocks[i].Count - 1; j >= 0; --j)
                {
                    if (!(code.blocks[i][j] is Expr) 
                        || (code.blocks[i][j] is Identity) && (code.blocks[i][j] as Identity).RightIsNumber())
                    {
                        continue;
                    }

                    ExpressionWrapper reachableExpression = new ExpressionWrapper(code.blocks[i][j] as Expr);

                    redifined.Add(reachableExpression.GetRedifined());

                    if (!reachableExpression.IsRedifinedBy(redifined))
                    {
                        blockGenKill.Gen.Add(reachableExpression);
                    }
                }

                for (int k = 0; k < code.blocks.Count; ++k)
                {
                    if (k == i)
                    {
                        continue;
                    }

                    for (int j = 0; j < code.blocks[k].Count; ++j)
                    {
                        if (!(code.blocks[k][j] is Expr)
                            || (code.blocks[k][j] is Identity) && (code.blocks[k][j] as Identity).RightIsNumber())
                        {
                            continue;
                        }

                        ExpressionWrapper reachableExpression = new ExpressionWrapper(code.blocks[k][j] as Expr);

                        if (reachableExpression.IsRedifinedBy(redifined) 
                            && !blockGenKill.Gen.Contains(reachableExpression))
                        {
                            blockGenKill.Kill.Add(reachableExpression);
                        }
                    }
                }
            }

            return exprGenKillList;
        }

        public static InOutData<Expr> BuildReachableExpressionsGenKill(ThreeAddrCode code)
        {
            Dictionary<Block, ExpressionGenKill> rawGenKill = BuildExpressionsGenKill(code);

            InOutData<Expr> blocksExprs = new InOutData<Expr>();

            foreach (var blockGenKill in rawGenKill)
            {
                blocksExprs.In[blockGenKill.Key] = FlushExprs(blockGenKill.Value.Gen);
                blocksExprs.Out[blockGenKill.Key] = FlushExprs(blockGenKill.Value.Kill);
            }

            return blocksExprs;
        }

        private static Dictionary<Block, TransferFunction<ExpressionWrapper>> BuildTransferFuncForReachableEpressions(ThreeAddrCode code)
        {
            var funcs = new Dictionary<Block, TransferFunction<ExpressionWrapper>>();
            Dictionary<Block, ExpressionGenKill> blocksGenKill = BuildExpressionsGenKill(code);

            foreach (var block in code.blocks)
            {
                funcs[block] = new TransferFunction<ExpressionWrapper>(blocksGenKill[block].Gen, blocksGenKill[block].Kill);
            }

            return funcs;
        }

        public static InOutData<Expr> BuildReachableExpressions(ThreeAddrCode code)
        {
            HashSet<ExpressionWrapper> top = new HashSet<ExpressionWrapper>();
            foreach (Block block in code.blocks)
            {
                foreach (Line.Line line in block)
                {
                    if (line is Expr)
                    {
                        top.Add(new ExpressionWrapper(line as Expr));
                    }
                }
            }

            var semilattice = new IntersectSemilattice<ExpressionWrapper>(top);
            var funcs = BuildTransferFuncForReachableEpressions(code);
            var alg = new IterativeAlgo<ExpressionWrapper, TransferFunction<ExpressionWrapper>>(semilattice, funcs);

            alg.Run(code);

            InOutData<Expr> result = new InOutData<Expr>();

            foreach (Block block in code.blocks)
            {
                result.In[block] = FlushExprs(alg.In[block]);
                result.Out[block] = FlushExprs(alg.Out[block]);
            }

            return result;
        }
    }

}