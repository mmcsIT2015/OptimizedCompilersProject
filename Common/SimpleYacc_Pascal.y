%{
// Эти объявления добавляются в класс GPPGParser, представляющий собой парсер, генерируемый системой gppg
    public BlockNode root; // Корневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%union { 			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
			public WriteNode wVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END ASSIGN SEMICOLON PLUS MINUS PROD DIV LB RB WRITE WRITELN COMMA IF ELSE THEN WHILE DO REPEAT UNTIL LESS MORE LESSEQUAL MOREEQUAL EQUAL NOTEQUAL NOT POINT
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident T F U
%type <stVal> assign statement if while repeatuntil
%type <blVal> stlist block
%type <wVal> exprlist write

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
		| T PROD expr { $$ = new BinaryNode($1, $3, BinaryType.Mult); }
		| T DIV expr { $$ = new BinaryNode($1, $3, BinaryType.Div); }
		;

T 		: U { $$ = $1; }
		| U PLUS expr { $$ = new BinaryNode($1, $3, BinaryType.Plus); }
		| U MINUS expr { $$ = new BinaryNode($1, $3, BinaryType.Minus); }
		;
		
U		: F { $$ = $1; }
		| F LESS expr { $$ = new BinaryNode($1, $3, BinaryType.Less); }
		| F MORE expr { $$ = new BinaryNode($1, $3, BinaryType.More); }
		| F LESSEQUAL expr { $$ = new BinaryNode($1, $3, BinaryType.LessEqual); }
		| F MOREEQUAL expr { $$ = new BinaryNode($1, $3, BinaryType.MoreEqual); }
		| F EQUAL expr { $$ = new BinaryNode($1, $3, BinaryType.Equal); }
		| F NOTEQUAL expr { $$ = new BinaryNode($1, $3, BinaryType.NotEqual); }
		| NOT expr { $$ = new UnaryNode($2, UnaryType.Not); }
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
				$$ = new WriteNode($1); 
			}
			| exprlist COMMA expr 
				{ 
					$1.Add($3); 
					$$ = $1; 
				}
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

