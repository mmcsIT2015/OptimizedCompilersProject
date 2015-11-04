using System.Collections.Generic;
using System.Text;
using SimpleLang;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public enum BinaryOperation { None, Plus, Minus, Mult, Div, Less, Greater, Equal, NotEqual, LessEqual, GreaterEqual };

    public enum VarType { Int, Bool, Double };
	public enum UnaryOperation { Not, Minus } ;
	
    public class Node // базовый класс для всех узлов    
    {
        public virtual void Accept(IVisitor visitor)
        {
            
        }
    }

    public class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class FloatNumNode : ExprNode
    {
        public double Num { get; set; }
        public FloatNumNode(double num) { Num = num; }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class BinaryNode : ExprNode
    {
        public ExprNode LeftOperand { get; set; }
        public ExprNode RightOperand { get; set; }
        public BinaryOperation Operation { get; set; }

        public BinaryNode(ExprNode lhs, ExprNode rhs, BinaryOperation operation)
        {
            LeftOperand = lhs;
            RightOperand = rhs;
            Operation = operation;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class StatementNode : Node // базовый класс для всех операторов
    {
    }

    public class FunctionNode : IdNode
    {
        public List<ExprNode> Parameters = new List<ExprNode>();

        public FunctionNode(string name) : base(name) {

        }

        public void AddParam(ExprNode expr)
        {
            Parameters.Add(expr);
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class FunctionNodeSt : StatementNode
    {
        public FunctionNode Function { get; set; }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class VarNode : StatementNode
    {
        public VarType Type { get; set; }
        public IdNode Id { get; set; }

        public VarNode(VarType type, IdNode id)
        {
            Type = type;
            Id = id;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

	public class UnaryNode : ExprNode
    {
        public ExprNode Expr { get; set; }
        public UnaryOperation Op { get; set; }
        public UnaryNode(ExprNode expr, UnaryOperation op)
        {
            Expr = expr;
            Op = op;
        }
    }
	
		
    public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public StatementNode StatElse { get; set; }

        public IfNode(ExprNode expr, StatementNode stat, StatementNode statelse = null)
        {
            Expr = expr;
            Stat = stat;
            StatElse = statelse;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

	public class WriteNode : StatementNode
    {
        public List<ExprNode> ExprList = new List<ExprNode>();

        public WriteNode(ExprNode expr)
        {
            Add(expr);
        }

        public void Add(ExprNode expr)
        {
            ExprList.Add(expr);
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class DoWhileNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }

        public DoWhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class CoutNode : StatementNode
    {
        public List<ExprNode> ExprList = new List<ExprNode>();

        public CoutNode(ExprNode expr)
        {
            Add(expr);
        }

        public void Add(ExprNode expr)
        {
            ExprList.Add(expr);
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

}