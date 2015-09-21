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
			public ProcedureNode pcVal;
			public ArgsNode aVal;
			public ExprAndPredicateNode eapbVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END WHILE DO ASSIGN ASSIGNPLUS ASSIGNMINUS ASSIGNMULT ASSIGNDIVIDE SEMICOLON PLUS MINUS MULT DIV LEFTBRACKET RIGHTBRACKET GREATER LESS EQUAL NOTEQUAL IF THEN ELSE REPEAT UNTIL COMMA
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident t f
%type <stVal> assign statement while 
%type <blVal> stlist block
%type <prVal> prexpr
%type <ifVal> if
%type <pcVal> proc
%type <aVal> arglist
%type <eapbVal> exprAndPredBin

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
		| proc { $$ = $1; }
		;

proc	: ident LEFTBRACKET arglist RIGHTBRACKET { $$ = new ProcedureNode($1 as IdNode, $3); }
		| ident { $$ = new ProcedureNode($1 as IdNode, null); }
		;

arglist	: exprAndPredBin { $$ = new ArgsNode($1); }
		| arglist COMMA exprAndPredBin
			{ 
				$1.Add($3); 
				$$ = $1; 
			}
		;

exprAndPredBin : expr { $$ = $1; }
		| prexpr { $$ = $1; }
		;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN exprAndPredBin { $$ = new AssignNode($1 as IdNode, $3, AssignType.Assign); }
		| ident ASSIGNPLUS exprAndPredBin { $$ = new AssignNode($1 as IdNode, $3, AssignType.Assign); }
		| ident ASSIGNMINUS exprAndPredBin { $$ = new AssignNode($1 as IdNode, $3, AssignType.AssignMinus); }
		| ident ASSIGNMULT exprAndPredBin { $$ = new AssignNode($1 as IdNode, $3, AssignType.AssignMult); }
		| ident ASSIGNDIVIDE exprAndPredBin { $$ = new AssignNode($1 as IdNode, $3, AssignType.AssignDivide); }
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

while	: WHILE exprAndPredBin DO statement { $$ = new WhileNode($2, $4, CycleType.WhileDo); }
		| REPEAT statement UNTIL exprAndPredBin { $$ = new WhileNode($4, $2, CycleType.DoUntil); }
		;

if		: IF exprAndPredBin THEN statement { $$ = new IfNode($2, $4); }
		| IF exprAndPredBin THEN statement ELSE statement { $$ = new IfNode($2, $4, $6); }
		;
	
%%

