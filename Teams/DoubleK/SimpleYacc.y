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
			public ExprBlockNode exblVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END CYCLE ASSIGN SEMICOLON PLUS MINUS PROD DIV LB RB WRITE WRITELN COMMA IF ELSE THEN WHILE DO REPEAT UNTIL LESS MORE LESSEQUAL MOREEQUAL EQUAL NOTEQUAL NOT POINT
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident T F U
%type <stVal> assign statement cycle if while repeatuntil
%type <blVal> stlist block
%type <exblVal> exprlist write

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

statement: assign { $$ = $1; }
		| block   { $$ = $1; }
		| cycle   { $$ = $1; }
		| write   { $$ = $1; }
		| if   { $$ = $1; }
		| while { $$ = $1; }
		| repeatuntil { $$ = $1; }
	;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr	: T { $$ = $1; }
		| T PROD expr { $$ = new BinOpNode($1, $3, "*"); }
		| T DIV expr { $$ = new BinOpNode($1, $3, "/"); }
		;

T 		: U { $$ = $1; }
		| U PLUS expr { $$ = new BinOpNode($1, $3, "+"); }
		| U MINUS expr { $$ = new BinOpNode($1, $3, "-"); }
		;
		
U		: F { $$ = $1; }
		| F LESS expr { $$ = new BinOpNode($1, $3, "<"); }
		| F MORE expr { $$ = new BinOpNode($1, $3, ">"); }
		| F LESSEQUAL expr { $$ = new BinOpNode($1, $3, "<="); }
		| F MOREEQUAL expr { $$ = new BinOpNode($1, $3, ">="); }
		| F EQUAL expr { $$ = new BinOpNode($1, $3, "=="); }
		| F NOTEQUAL expr { $$ = new BinOpNode($1, $3, "!="); }
		| NOT expr { $$ = new UnOpNode($2, "!"); }
		;
		
F       : ident  { $$ = $1 as IdNode; }
		| MINUS INUM { $$ = new IntNumNode($2); }
		| INUM { $$ = new IntNumNode($1); }
		| LB expr RB { $$ = $2; }
		;
				
write	: WRITE LB exprlist RB { $$ = $3; }
		| WRITELN LB exprlist RB { $$ = $3; }
		;

exprlist	: expr 
			{ 
				$$ = new ExprBlockNode($1); 
			}
			| exprlist COMMA expr 
				{ 
					$1.Add($3); 
					$$ = $1; 
				}
			;
		
	
block	: BEGIN stlist END { $$ = $2; }
		;

cycle	: CYCLE expr statement { $$ = new CycleNode($2, $3); }
		;
		
if 		: IF expr THEN statement ELSE statement  { $$ = new IfNode($2, $4, new ElseNode($6)); }
		| IF expr THEN statement { $$ = new IfNode($2, $4, new ElseNode()); }
		;
		
while  	: WHILE expr DO statement { $$ = new WhileNode($2, $4); }
		;

repeatuntil 	: 	REPEAT statement UNTIL expr { $$ = new RepeatUntilNode($4, $2); }
				;
	
%%

