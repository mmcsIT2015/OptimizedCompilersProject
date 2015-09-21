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
			public PredicateBinaryNode prVal;
			public IfNode ifVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END WHILE DO ASSIGN SEMICOLON PLUS MINUS MULT DIV LEFTBRACKET RIGHTBRACKET GREATER LESS EQUAL NOTEQUAL IF THEN ELSE
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident t f
%type <stVal> assign statement while 
%type <blVal> stlist block
%type <prVal> prexpr
%type <ifVal> if

%%

progr   : block { root = $1; }
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
		| while   { $$ = $1; }
		| if { $$ = $1; }
	;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		| ident ASSIGN prexpr { $$ = new AssignNode($1 as IdNode, $3); }
		;

prexpr	: expr GREATER expr { $$ = new PredicateBinaryNode($1, $3, PredicateOperationType.Greater); }
		| expr LESS expr { $$ = new PredicateBinaryNode($1, $3, PredicateOperationType.Less); }
		| expr EQUAL expr { $$ = new PredicateBinaryNode($1, $3, PredicateOperationType.Equal); }
		| expr NOTEQUAL expr { $$ = new PredicateBinaryNode($1, $3, PredicateOperationType.Notequal); }
		;

expr	: t { $$ = $1; }
		| expr PLUS t { $$ = new BinaryNode($1, $3, OperationType.Plus); }
		| expr MINUS t { $$ = new BinaryNode($1, $3, OperationType.Minus); }
		;

t		: f { $$ = $1; }
		| t MULT f { $$ = new BinaryNode($1, $3, OperationType.Mult); }
		| t DIV f { $$ = new BinaryNode($1, $3, OperationType.Div); }
		;

f		: ident { $$ = $1 as IdNode; }
		| INUM { $$ = new IntNumNode($1); }
		| RNUM { $$ = new RealNumNode($1); }
		| LEFTBRACKET expr RIGHTBRACKET { $$ = $2; }
		;

block	: BEGIN stlist END { $$ = $2; }
		;

while	: WHILE prexpr DO statement { $$ = new WhileNode($2, $4); }
		;

if		: IF prexpr THEN statement { $$ = new IfNode($2, $4); }
		| IF prexpr THEN statement ELSE statement { $$ = new IfNode($2, $4, $6); }
		;
	
%%

