using System.Collections.Generic;
using System.Text;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public enum BinaryType { Plus, Minus, Mult, Div, Less, More, Equal, NotEqual, LessEqual, MoreEqual };

    public enum CycleType { While, DoWhile };

    public enum VarType { Int, Bool };

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

    public class BinaryNode : ExprNode
    {
        public ExprNode LeftOperand { get; set; }
        public ExprNode RightOperand { get; set; }
        public BinaryType Operation { get; set; }
        public BinaryNode(ExprNode leftoperand, ExprNode rightoperand, BinaryType operation) { LeftOperand = leftoperand; RightOperand = rightoperand; Operation = operation; }
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

    public class VarNode : StatementNode
    {
        public VarType Type { get; set; }

        public IdNode Id { get; set; }

        public VarNode(VarType type, IdNode id) { Type = type; Id = id; }
    }

    public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public StatementNode StatElse { get; set; }

        public IfNode(ExprNode expr, StatementNode stat, StatementNode statelse = null) { Expr = expr; Stat = stat; StatElse = statelse; }
    }

    public class CycleNode : StatementNode
    {
        public CycleType Type { get; set; }
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public CycleNode(CycleType type, ExprNode expr, StatementNode stat)
        {
            Type = type;
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

}