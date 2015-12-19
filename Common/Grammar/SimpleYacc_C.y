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
			public string typeVal;
			public VarDeclListNode listDeclVal;
		  public List<VarDeclNode> listDeclRawVal;
        }

%using ProgramTree;

%namespace SimpleParserC

%token ASSIGN SEMICOLON PLUS MINUS MUL DIV LBRACKET RBRACKET BEGIN END IF ELSE WHILE DO LESS GREAT EQUAL INEQUAL LSHIFT COUT COMMA COLON NOT LESSEQUAL GREATEREQUAL GOTO ENDL
%token <iVal> INUM
%token <dVal> RNUM
%token <sVal> ID STRING_L
%token <typeVal> TYPE

%type <eVal> expr ident T F S U
%type <stVal> assign statement st do_while while if goto
%type <blVal> stlist block
%type <ioVal> cout cout_list
%type <funVal> funcall
%type <funStVal> funcallst
%type <paramVal> params
%type <listDeclVal> decl_type_list
%type <listDeclRawVal> decl_list	

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

cout_list : LSHIFT expr { $$ = new CoutNode(); $$.Add($2); }
	| LSHIFT ENDL { $$ = new CoutNode(); $$.Add(new EndlNode()); }
	| cout_list LSHIFT expr { $$ = $1; $$.Add($3); }
	| cout_list LSHIFT ENDL { $$ = $1; $$.Add(new EndlNode()); }
	;
cout	: COUT cout_list { $$ = $2; } ;  

params : expr { $$ = new List<ExprNode>(); $$.Add($1); }
    |	params COMMA expr
		{
			$1.Add($3);
			$$ = $1;
		}
    ;
	
statement: ident COLON st { $3.AddLabel($1 as IdNode); $$ = $3;}
	| st { $$ = $1; }
	;

st: assign SEMICOLON { $$ = $1; }
    | block { $$ = $1; }
    | do_while SEMICOLON { $$ = $1; }
    | while { $$ = $1; }
    | cout SEMICOLON { $$ = $1; }
    | if { $$ = $1; }
    | funcallst SEMICOLON { $$ = $1; }
	| decl_type_list SEMICOLON { $$ = $1; }
	| goto SEMICOLON { $$ = $1; }
    ;

goto: GOTO ident { $$ = new GotoNode($2 as IdNode); }
	;
	
decl_type_list: 	
	| TYPE decl_list 
	{ 
		SimpleVarType type;
		if ($1 == "int")
			type = SimpleVarType.Int;
		else if ($1 == "float")
			type = SimpleVarType.Float;
		else if ($1 == "string")
			type = SimpleVarType.Str;
		else if ($1 == "bool")
			type = SimpleVarType.Bool;
		else type = SimpleVarType.Unknown;
		$$ = new VarDeclListNode(type); 
		$$.VariablesList = $2; 
	}	
	;

decl_list:
	| ident { $$ = new List<VarDeclNode>(); $$.Add(new VarDeclNode($1 as IdNode)); }
	| assign { $$ = new List<VarDeclNode>(); ($1 as AssignNode).IsDeclaration = true; $$.Add(new VarDeclNode($1 as AssignNode)); }
	| decl_list COMMA ident { $1.Add(new VarDeclNode($3 as IdNode)); $$ = $1; }
	| decl_list COMMA assign { ($3 as AssignNode).IsDeclaration = true; $1.Add(new VarDeclNode($3 as AssignNode)); $$ = $1; }
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
    | expr LESS S { $$ = new BinaryNode($1, $3, Operator.Less); }
    | expr GREAT S { $$ = new BinaryNode($1, $3, Operator.Greater); }
    | expr EQUAL S { $$ = new BinaryNode($1, $3, Operator.Equal); }
    | expr INEQUAL S { $$ = new BinaryNode($1, $3, Operator.NotEqual); }
	| expr LESSEQUAL S { $$ = new BinaryNode($1, $3, Operator.LessEqual); }
	| expr GREATEREQUAL S { $$ = new BinaryNode($1, $3, Operator.GreaterEqual); }
    ;

S : T { $$ = $1; }
    | S PLUS T { $$ = new BinaryNode($1, $3, Operator.Plus); }
    | S MINUS T { $$ = new BinaryNode($1, $3, Operator.Minus); }
    ;

T : U { $$ = $1; }
    | T MUL U { $$ = new BinaryNode($1, $3, Operator.Mult); }
    | T DIV U { $$ = new BinaryNode($1, $3, Operator.Div); }
    ;

U : F { $$ = $1; }
	| MINUS U { $$ = new UnaryNode($2, Operator.Minus); }
	| NOT U { $$ = new UnaryNode($2, Operator.Not); }
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
