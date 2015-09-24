%{
// Эти объявления добавляются в класс GPPGParser, представляющий собой парсер, генерируемый системой gppg
    public BlockNode root; // Корневой узел синтаксического дерева 
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
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END CYCLE ASSIGN SEMICOLON LEFT_R_BRACKET RIGHT_R_BRACKET IF ELSE FOR
%token PLUS MINUS MULT DIV MORE LESS MORE_EQUAL LESS_EQUAL
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident 
%type <stVal> assign statement cycle 
%type <blVal> stlist block
%type <bExprVal> binmult binsum bincond
%type <condVal> cond
%type <forVal> for_cycle

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

cycle	: CYCLE LEFT_R_BRACKET bincond RIGHT_R_BRACKET statement { $$ = new CycleNode($3, $5); }
		;
	
%%

