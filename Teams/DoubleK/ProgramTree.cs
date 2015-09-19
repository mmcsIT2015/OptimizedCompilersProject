using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };
	public enum OpType { PLUS, MINUS, PROD, DIV };

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
	
	public class DescrNode : StatementNode
	{
		public IdNode Id { get; set; }
		public DescrNode(IdNode id) { Id = id; }
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
	
	public class WhileNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public List<StatementNode> Statlist { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(ExprNode expr, List<StatementNode> statlist)
        {
            Expr = expr;
            Statlist = statlist;
        }

        public WhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }
	
	public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public List<StatementNode> Statlist { get; set; }
        public List<StatementNode> Statlist1 { get; set; }

        public StatementNode Stat { get; set; }
        public StatementNode Stat1 { get; set; }
        
        public IfNode(ExprNode expr, List<StatementNode> statlist)
        {
            Expr = expr;
            Statlist = statlist;
        }

        public IfNode(ExprNode expr, List<StatementNode> statlist, List<StatementNode> statlist1)
        {
            Expr = expr;
            Statlist = statlist;
            Statlist1 = statlist1;
        }

        public IfNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }

        public IfNode(ExprNode expr, StatementNode stat, StatementNode stat1)
        {
            Expr = expr;
            Stat = stat;
            Stat1 = stat1;
        }
    }
	
	public class BinaryNode : StatementNode
    {
        public IdNode Id1 { get; set; }
        public IdNode Id2 { get; set; }
        public int Op { get; set; }
        public BinaryNode(IdNode id1, IdNode id2, int op)
        {
            Id1 = id1;
            Id2 = id2;
            Op = op;
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
