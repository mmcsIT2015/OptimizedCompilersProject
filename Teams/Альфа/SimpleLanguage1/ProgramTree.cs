using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };
    public enum OperationType { Plus, Minus, Mult, Div};
    public enum PredicateOperationType { Greater, Less, Equal, Notequal };

    public class Node // базовый класс для всех узлов    
    {
    }

    public class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class BinaryNode : ExprNode
    {
        public ExprNode LeftOperand { get; set; }
        public ExprNode RightOperand { get; set; }
        public OperationType Operation { get; set; }
        public BinaryNode(ExprNode LeftOperand, ExprNode RightOperand, OperationType Operation)
        {
            this.LeftOperand = LeftOperand;
            this.RightOperand = RightOperand;
            this.Operation = Operation;
        }
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

    public class RealNumNode : ExprNode
    {
        public double Num { get; set; }
        public RealNumNode(double num) { Num = num; }
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
    public class PredicateBinaryNode : ExprNode
    {
        public ExprNode LeftOperand { get; set; }
        public ExprNode RightOperand { get; set; }
        public PredicateOperationType Operation { get; set; }
        public PredicateBinaryNode(ExprNode LeftOperand, ExprNode RightOperand, PredicateOperationType Operation)
        {
            this.LeftOperand = LeftOperand;
            this.RightOperand = RightOperand;
            this.Operation = Operation;
        }
    }
    public class IfNode : StatementNode
    {
        public PredicateBinaryNode Predicate { get; set; }
        public StatementNode IfStat { get; set; }
        public StatementNode ElseStat { get; set; }
        public IfNode(PredicateBinaryNode Predicate, StatementNode IfStat, StatementNode ElseStat = null)
        {
            this.Predicate = Predicate;
            this.IfStat = IfStat;
            this.ElseStat = ElseStat;
        }
    }
}