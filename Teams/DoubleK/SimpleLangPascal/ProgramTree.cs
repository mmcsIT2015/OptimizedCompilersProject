using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

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
	
	public class BinOpNode : ExprNode
	{
		public ExprNode Expr1 { get; set; }
		public ExprNode Expr2 { get; set; }
		public string Op { get; set; }
		public BinOpNode(ExprNode expr1, ExprNode expr2, string op)
		{
			Expr1 = expr1;
			Expr2 = expr2;
			Op = op;
		}
	}

    public class UnOpNode : ExprNode
    {
        public ExprNode Expr { get; set; }
        public string Op { get; set; }
        public UnOpNode(ExprNode expr, string op)
        {
            Expr = expr;
            Op = op;
        }
    }

    public class ExprBlockNode : StatementNode
    {
        public List<ExprNode> ExprList = new List<ExprNode>();
        public ExprBlockNode(ExprNode expr)
        {
            Add(expr);
        }
        public void Add(ExprNode expr)
        {
            ExprList.Add(expr);
        }
    }

    public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public ElseNode Elsy { get; set; }

        public IfNode(ExprNode expr, StatementNode stat, ElseNode elsy)
        {
            Expr = expr;
            Stat = stat;
            Elsy = elsy;
        }
    }

    public class ElseNode : StatementNode
    {
        public StatementNode Stat { get; set; }
        public ElseNode(StatementNode stat)
        {
            Stat = stat;
        }
        public ElseNode()
        {
            ;
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
	
	public class WhileNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class RepeatUntilNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public RepeatUntilNode(ExprNode expr, StatementNode stat)
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

}