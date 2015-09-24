%{
// ��� ���������� ����������� � ����� GPPGParser, �������������� ����� ������, ������������ �������� gppg
    public BlockNode root; // �������� ���� ��������������� ������ 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%union { 
			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
			public BinExprNode bExprVal;
			public ConditionNode condVal;
      public ForCycleNode forVal;
      public PrintNode printVal;
      public ExprListNode exprlVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END CYCLE ASSIGN SEMICOLON LEFT_R_BRACKET RIGHT_R_BRACKET IF ELSE FOR CIN COUT ENDL DO
%token PLUS MINUS MULT DIV MORE LESS MORE_EQUAL LESS_EQUAL SHIFT_LEFT SHIFT_RIGHT
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident 
%type <stVal> assign statement cycle 
%type <blVal> stlist block
%type <bExprVal> binmult binsum bincond
%type <condVal> cond
%type <forVal> for_cycle
%type <printVal> print_expr
%type <exprlVal> print_expr_list

%%

progr   : block { root = $1; }
		;

stlist	: statement
			{ 
				$$ = new BlockNode($1); 
			}
		| stlist statement 
			{ 
				$1.Add($2); 
				$$ = $1; 
			}
		;

statement: assign SEMICOLON { $$ = $1; }
		| block   { $$ = $1; }
		| cycle   { $$ = $1; }
		| cond	  { $$ = $1 as StatementNode; }
    | for_cycle { $$ = $1 as StatementNode; }
    | print_expr SEMICOLON { $$ = $1 as StatementNode; }
		;

print_expr_list: bincond { $$ = new ExprListNode(); $$.Add($1); }
               | bincond SHIFT_LEFT print_expr_list { $$.Add($1); }
               ;
               
print_expr: COUT SHIFT_LEFT print_expr_list { $$ = new PrintNode($3, false); }
          ;
		
for_cycle : FOR LEFT_R_BRACKET assign SEMICOLON bincond SEMICOLON assign RIGHT_R_BRACKET statement { $$ = new ForCycleNode($3 as AssignNode, $5, $7 as AssignNode, $9); }          
          ;
          
cond	: IF LEFT_R_BRACKET bincond RIGHT_R_BRACKET statement ELSE statement { $$ = new ConditionNode($3, $5, $7); }
      | IF LEFT_R_BRACKET bincond RIGHT_R_BRACKET statement { $$ = new ConditionNode($3, $5, null); }
		;
    
ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN bincond { $$ = new AssignNode($1 as IdNode, $3 as ExprNode); }
		;

bincond	: bincond MORE binsum { $$ = new BinExprNode($1, $3, OperationType.More); }
    | ENDL { $$ = new BinExprNode(null, null, OperationType.Endl); }
		| bincond LESS binsum { $$ = new BinExprNode($1, $3, OperationType.Less); }
    | bincond MORE_EQUAL binsum { $$ = new BinExprNode($1, $3, OperationType.MoreEqual); }
    | bincond LESS_EQUAL binsum { $$ = new BinExprNode($1, $3, OperationType.LessEqual); }
		| binsum { $$ = $1 as BinExprNode; }    
		;

binsum	: binsum PLUS binmult { $$ = new BinExprNode($1, $3, OperationType.Plus); }
		| binsum MINUS binmult { $$ = new BinExprNode($1, $3, OperationType.Minus); }
		| binmult { $$ = $1 as BinExprNode; }		
		;

binmult	: binmult MULT expr { $$ = new BinExprNode($1, $3, OperationType.Mult); }
		| binmult DIV expr { $$ = new BinExprNode($1, $3, OperationType.Div); }
		| expr { $$ = $1 as BinExprNode; }
		;
		
expr	: ident  { $$ = $1 as IdNode; }
		| INUM { $$ = new IntNumNode($1); }	
		| LEFT_R_BRACKET binsum RIGHT_R_BRACKET { $$ = $2 as ExprNode; }
		;
		
block	: BEGIN stlist END { $$ = $2; }
		;

cycle	: CYCLE LEFT_R_BRACKET bincond RIGHT_R_BRACKET statement { $$ = new CycleNode($3, $5, true); }
      | DO statement CYCLE LEFT_R_BRACKET bincond RIGHT_R_BRACKET SEMICOLON { $$ = new CycleNode($5, $2, false); }
		;
	
%%

