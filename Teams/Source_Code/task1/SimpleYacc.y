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
			public CoutNode cVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END WHILE DO ASSIGN SEMICOLON PLUS MINUS MULT DIV BRACKETL BRACKETR LESS MORE EQUAL NOTEQUAL LESSEQUAL MOREEQUAL INT BOOL IF ELSE COUT STREAM
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident T F L
%type <stVal> assign statement cycle var if
%type <blVal> stlist block
%type <cVal> cout

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
		| cycle   { $$ = $1; }
		| var 	  { $$ = $1; }
		| if	  { $$ = $1; }
		| cout 	  { $$ = $1; }
	;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

var		: INT ident { $$ = new VarNode(VarType.Int, $2 as IdNode); }
		| BOOL ident { $$ = new VarNode(VarType.Bool, $2 as IdNode); }
		;
		
if		: IF BRACKETL expr BRACKETR statement { $$ = new IfNode($3, $5); }
		| IF BRACKETL expr BRACKETR statement ELSE statement { $$ = new IfNode($3, $5, $7); }
		;
	
expr	: L { $$ = $1; }
		| expr LESS L { $$ = new BinaryNode($1, $3, BinaryType.Less); }
		| expr MORE L { $$ = new BinaryNode($1, $3, BinaryType.More); }
		| expr EQUAL L { $$ = new BinaryNode($1, $3, BinaryType.Equal); }
		| expr NOTEQUAL L { $$ = new BinaryNode($1, $3, BinaryType.NotEqual); }
		| expr LESSEQUAL L { $$ = new BinaryNode($1, $3, BinaryType.LessEqual); }
		| expr MOREEQUAL L { $$ = new BinaryNode($1, $3, BinaryType.MoreEqual); }
		;
		
L 		: T { $$ = $1; }
		| L PLUS T { $$ = new BinaryNode($1, $3, BinaryType.Plus); }
		| L MINUS T { $$ = new BinaryNode($1, $3, BinaryType.Minus); }
		;

T    	: F { $$ = $1; }
		| T MULT F { $$ = new BinaryNode($1, $3, BinaryType.Mult); }
		| T DIV F { $$ = new BinaryNode($1, $3, BinaryType.Div); }
		;

F    	: ident { $$ = $1 as IdNode; }
		| INUM { $$ = new IntNumNode($1); }
		| BRACKETL expr BRACKETR { $$ = $2; }
		;
		
block	: BEGIN stlist END { $$ = $2; }
		;

cycle	: WHILE BRACKETL expr BRACKETR statement { $$ = new CycleNode(CycleType.While, $3, $5); }
		| DO statement WHILE BRACKETL expr BRACKETR { $$ = new CycleNode(CycleType.DoWhile, $5, $2); }
		;
	
cout	: COUT STREAM expr { $$ = new CoutNode($3); }
		| cout STREAM expr { $1.Add($3); $$ = $1; }
		;
	
%%

