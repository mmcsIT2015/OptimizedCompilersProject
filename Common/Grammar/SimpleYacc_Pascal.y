%{
// Эти объявления добавляются в класс GPPGParser, представляющий собой парсер, генерируемый системой gppg
    public BlockNode root; // Корневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYaccPascal.cs

%union { 			
			public double dVal; 
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

%token BEGIN END ASSIGN SEMICOLON PLUS VAR MINUS PROD DIV LB RB GOTO COMMA IF ELSE THEN WHILE DO REPEAT UNTIL LESS MORE LESSEQUAL MOREEQUAL EQUAL NOTEQUAL NOT POINT COLON
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID STRING_L
%token <typeVal> TYPE

%type <eVal> expr ident S T U F
%type <stVal> assign statement st if while repeatuntil decl_assign goto
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

statement: ident COLON st { $3.AddLabel($1 as IdNode); $$ = $3;}
			| st { $$ = $1; }
;
	
st: 	assign { $$ = $1; }
		| block   { $$ = $1; }
		| if   { $$ = $1; }
		| while { $$ = $1; }
		| repeatuntil { $$ = $1; }
		| funcallst { $$ = $1; }
		| decl_assign { $$ = $1; }
		| goto { $$ = $1; }
	;
	
goto: GOTO ident { $$ = new GotoNode($2 as IdNode); }
	;	
	
funcallst : funcall { $$ = new FunctionNodeSt(); $$.Function = $1; }
		;

funcall	: ID LB params RB
		{
			$$ = new FunctionNode($1);
			$$.Parameters = $3;
		}
		;
		
decl_assign: VAR ident COLON TYPE ASSIGN expr 
		{ 
			List<VarDeclNode> ls = new List<VarDeclNode>();
			ls.Add(new VarDeclNode(new AssignNode($2 as IdNode, $6)));
			var listNode = new VarDeclListNode($4); 
			listNode.VariablesList = ls;
			$$ = listNode;
		}
	| VAR ident COLON TYPE 
		{ 
			var ls = new List<VarDeclNode>();
			ls.Add(new VarDeclNode($2 as IdNode));
			var listNode = new VarDeclListNode($4); 
			listNode.VariablesList = ls;
			$$ = listNode;
		}
	;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr : S { $$ = $1; }
	| expr LESS S { $$ = new BinaryNode($1, $3, Operator.Less); }
	| expr MORE S { $$ = new BinaryNode($1, $3, Operator.Greater); }
	| expr LESSEQUAL S { $$ = new BinaryNode($1, $3, Operator.LessEqual); }
	| expr MOREEQUAL S { $$ = new BinaryNode($1, $3, Operator.GreaterEqual); }
	| expr EQUAL S { $$ = new BinaryNode($1, $3, Operator.Equal); }
	| expr NOTEQUAL S { $$ = new BinaryNode($1, $3, Operator.NotEqual); }
	;

S : T { $$ = $1; }
    | S PLUS T { $$ = new BinaryNode($1, $3, Operator.Plus); }
    | S MINUS T { $$ = new BinaryNode($1, $3, Operator.Minus); }
    ;

T : U { $$ = $1; }
    | T PROD U { $$ = new BinaryNode($1, $3, Operator.Mult); }
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

