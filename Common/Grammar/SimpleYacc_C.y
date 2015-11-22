%{
    public BlockNode root;
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYaccC.cs

%union {
			public double dVal;
			public int iVal;
			public string sVal;
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
			public CoutNode ioVal;
			public FunctionNode funVal;
			public FunctionNodeSt funStVal;
			public List<ExprNode> paramVal;
			public SimpleVarType typeVal;
        }

%using ProgramTree;

%namespace SimpleParserC

%token ASSIGN SEMICOLON PLUS MINUS MUL DIV LBRACKET RBRACKET BEGIN END IF ELSE WHILE DO LESS GREAT EQUAL INEQUAL LSHIFT COUT COMMA NOT
%token <iVal> INUM
%token <dVal> RNUM
%token <sVal> ID STRING_L
%token <typeVal> TYPE

%type <eVal> expr ident T F S U
%type <stVal> assign statement do_while while if decl_assign
%type <blVal> stlist block
%type <ioVal> cout
%type <funVal> funcall
%type <funStVal> funcallst
%type <paramVal> params

%%

progr   : block { root = $1; }
		;

stlist	: statement { $$ = new BlockNode($1);	}
    | stlist statement
		{
			$1.Add($2);
			$$ = $1;
		}
	 ;

cout	: COUT LSHIFT expr { $$ = new CoutNode($3); }
    | cout LSHIFT expr
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

statement: assign SEMICOLON { $$ = $1; }
    | block { $$ = $1; }
    | do_while SEMICOLON { $$ = $1; }
    | while { $$ = $1; }
    | cout SEMICOLON { $$ = $1; }
    | if { $$ = $1; }
    | funcallst SEMICOLON { $$ = $1; }
		| decl_assign SEMICOLON { $$ = $1; }
    ;

decl_assign: TYPE assign { $$ = new VarDeclNode($1, $2 as AssignNode); }
	| TYPE ident { $$ = new VarDeclNode($1, $2 as IdNode); }
	;

funcallst : funcall { $$ = new FunctionNodeSt(); $$.Function = $1; }
	;

funcall	: ID LBRACKET params RBRACKET
	{
		$$ = new FunctionNode($1);
		$$.Parameters = $3;
	}
	;

ident 	: ID { $$ = new IdNode($1); }
	;

assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
	;

expr : S { $$ = $1; }
    | expr LESS S { $$ = new BinaryNode($1, $3, BinaryOperation.Less); }
    | expr GREAT S { $$ = new BinaryNode($1, $3, BinaryOperation.Greater); }
    | expr EQUAL S { $$ = new BinaryNode($1, $3, BinaryOperation.Equal); }
    | expr INEQUAL S { $$ = new BinaryNode($1, $3, BinaryOperation.NotEqual); }
    ;

S : T { $$ = $1; }
    | S PLUS T { $$ = new BinaryNode($1, $3, BinaryOperation.Plus); }
    | S MINUS T { $$ = new BinaryNode($1, $3, BinaryOperation.Minus); }
    ;

T : U { $$ = $1; }
    | T MUL U { $$ = new BinaryNode($1, $3, BinaryOperation.Mult); }
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
    | LBRACKET expr RBRACKET { $$ = $2; }
    | funcall { $$ = $1; }
    ;

if	: IF LBRACKET expr RBRACKET statement { $$ = new IfNode($3, $5); }
    | IF LBRACKET expr RBRACKET statement ELSE statement { $$ = new IfNode($3, $5, $7); }
    ;

block	: BEGIN stlist END { $$ = $2; }
	;

do_while	: DO statement WHILE LBRACKET expr RBRACKET { $$ = new DoWhileNode($5, $2); }
	;

while	: WHILE LBRACKET expr RBRACKET statement { $$ = new WhileNode($3, $5); }
    ;

%%
