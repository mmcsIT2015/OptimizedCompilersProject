using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };
    public enum OperationType { Plus, Minus, Mult, Div, More, Less, MoreEqual, LessEqual };

    public class Node // базовый класс для всех узлов    
    {
    }

    public class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
    }

    public class BinExprNode : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public OperationType opType { get; set; }
        public BinExprNode(ExprNode Left, ExprNode Right, OperationType opType)
        {
            this.Left = Left;
            this.Right = Right;
            this.opType = opType;
        }
    }

    public class StatementNode : Node // базовый класс для всех операторов
    {
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(IdNode id, ExprNode expr, AssignType assop = AssignType.Assign)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }
    }

    public class ConditionNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode blockTrue { get; set; }
        public StatementNode blockFalse { get; set; }

        public ConditionNode(ExprNode expr, StatementNode blockTrue, StatementNode blockFalse)
        {
            this.Expr = expr;
            this.blockFalse = blockFalse;
            this.blockTrue = blockTrue;
        }
    }

    public class CycleNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class ForCycleNode : StatementNode
    {
        public AssignNode AssignSt { get; set; }
        public ExprNode Expr { get; set; }
        public AssignNode Cycle { get; set; }
        public StatementNode Block { get; set; }
        public ForCycleNode(AssignNode AssignSt, ExprNode expr, AssignNode cycle, StatementNode block)
        {
            this.Expr = expr;
            this.Cycle = cycle;
            this.Block = block;
            this.AssignSt = AssignSt;
        }
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
    }

}