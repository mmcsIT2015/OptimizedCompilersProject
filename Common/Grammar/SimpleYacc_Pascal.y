%{
// Эти объявления добавляются в класс GPPGParser, представляющий собой парсер, генерируемый системой gppg
    public BlockNode root; // Корневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYaccPascal.cs

%union { 			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
			public FunctionNode funVal;
			public FunctionNodeSt funStVal;
			public List<ExprNode> paramVal;
       }

%using ProgramTree;

%namespace SimpleParserPascal

%token BEGIN END ASSIGN SEMICOLON PLUS MINUS PROD DIV LB RB COMMA IF ELSE THEN WHILE DO REPEAT UNTIL LESS MORE LESSEQUAL MOREEQUAL EQUAL NOTEQUAL NOT POINT
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident T F U
%type <stVal> assign statement if while repeatuntil
%type <blVal> stlist block
%type <funVal> funcall
%type <funStVal> funcallst
%type <paramVal> params

%%

progr   : block POINT{ root = $1; }
		;

stlist	: statement 
			{ 
				$$ = new BlockNode($1); 
			}
		| stlist SEMICOLON statement 
			{ 
				$1.Add($3); 
				$$ = $1; 
			}
		;
		
params : expr { $$ = new List<ExprNode>(); $$.Add($1); }
    |	params COMMA expr
      {
         $1.Add($3);
			$$ = $1;
		}
    ;
		
statement: assign { $$ = $1; }
		| block   { $$ = $1; }
		| if   { $$ = $1; }
		| while { $$ = $1; }
		| repeatuntil { $$ = $1; }
		| funcallst { $$ = $1; }
	;
	
funcallst : funcall { $$ = new FunctionNodeSt(); $$.Function = $1; }
		;

funcall	: ID LB params RB
		{
			$$ = new FunctionNode($1);
			$$.Parameters = $3;
		}
		;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr	: T { $$ = $1; }
		| T PROD expr { $$ = new BinaryNode($1, $3, BinaryOperation.Mult); }
		| T DIV expr { $$ = new BinaryNode($1, $3, BinaryOperation.Div); }
		;

T 		: U { $$ = $1; }
		| U PLUS expr { $$ = new BinaryNode($1, $3, BinaryOperation.Plus); }
		| U MINUS expr { $$ = new BinaryNode($1, $3, BinaryOperation.Minus); }
		;
		
U		: F { $$ = $1; }
		| F LESS expr { $$ = new BinaryNode($1, $3, BinaryOperation.Less); }
		| F MORE expr { $$ = new BinaryNode($1, $3, BinaryOperation.Greater); }
		| F LESSEQUAL expr { $$ = new BinaryNode($1, $3, BinaryOperation.LessEqual); }
		| F MOREEQUAL expr { $$ = new BinaryNode($1, $3, BinaryOperation.GreaterEqual); }
		| F EQUAL expr { $$ = new BinaryNode($1, $3, BinaryOperation.Equal); }
		| F NOTEQUAL expr { $$ = new BinaryNode($1, $3, BinaryOperation.NotEqual); }
		| MINUS expr { $$ = new UnaryNode($2, UnaryOperation.Minus); }
		| NOT expr { $$ = new UnaryNode($2, UnaryOperation.Not); }
		;
		
F       : ident  { $$ = $1 as IdNode; }
		| INUM { $$ = new IntNumNode($1); }
		| LB expr RB { $$ = $2; }
		| funcall { $$ = $1; }
		;
				
block	: BEGIN stlist END { $$ = $2; }
		;

if 		: IF expr THEN statement ELSE statement  { $$ = new IfNode($2, $4, $6); }
		| IF expr THEN statement { $$ = new IfNode($2, $4); }
		;
		
while  	: WHILE expr DO statement { $$ = new WhileNode($2, $4); }
		;

repeatuntil 	: 	REPEAT statement UNTIL expr { $$ = new DoWhileNode($4, $2); }
				;
	
%%

