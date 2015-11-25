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
			public SimpleVarType typeVal;
       }

%using ProgramTree;

%namespace SimpleParserPascal

%token BEGIN END ASSIGN SEMICOLON PLUS MINUS PROD DIV LB RB COMMA IF ELSE THEN WHILE DO REPEAT UNTIL LESS MORE LESSEQUAL MOREEQUAL EQUAL NOTEQUAL NOT POINT
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID STRING_L
%token <typeVal> TYPE

%type <eVal> expr ident S T U F
%type <stVal> assign statement if while repeatuntil decl_assign
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
		| decl_assign SEMICOLON { $$ = $1; }
	;
	
funcallst : funcall { $$ = new FunctionNodeSt(); $$.Function = $1; }
		;

funcall	: ID LB params RB
		{
			$$ = new FunctionNode($1);
			$$.Parameters = $3;
		}
		;
		
decl_assign: TYPE assign { $$ = new VarDeclNode($1, $2 as AssignNode); }
	| TYPE ident { $$ = new VarDeclNode($1, $2 as IdNode); }
	;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr : S { $$ = $1; }
	| expr LESS S { $$ = new BinaryNode($1, $3, BinaryOperation.Less); }
	| expr MORE S { $$ = new BinaryNode($1, $3, BinaryOperation.Greater); }
	| expr LESSEQUAL S { $$ = new BinaryNode($1, $3, BinaryOperation.LessEqual); }
	| expr MOREEQUAL S { $$ = new BinaryNode($1, $3, BinaryOperation.GreaterEqual); }
	| expr EQUAL S { $$ = new BinaryNode($1, $3, BinaryOperation.Equal); }
	| expr NOTEQUAL S { $$ = new BinaryNode($1, $3, BinaryOperation.NotEqual); }
	;

S : T { $$ = $1; }
    | S PLUS T { $$ = new BinaryNode($1, $3, BinaryOperation.Plus); }
    | S MINUS T { $$ = new BinaryNode($1, $3, BinaryOperation.Minus); }
    ;

T : U { $$ = $1; }
    | T PROD U { $$ = new BinaryNode($1, $3, BinaryOperation.Mult); }
    | T DIV U { $$ = new BinaryNode($1, $3, BinaryOperation.Div); }
    ;

U : F { $$ = $1; }
	| MINUS U { $$ = new UnaryNode($2, UnaryOperation.Minus); }
	| NOT U { $$ = new UnaryNode($2, UnaryOperation.Not); }
	;

F : ident { $$ = $1 as IdNode; }
    | INUM { $$ = new IntNumNode($1); }
	| RNUM { $$ = new FloatNumNode($1); }
	| STRING_L { $$ = new StringLiteralNode($1); }
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

