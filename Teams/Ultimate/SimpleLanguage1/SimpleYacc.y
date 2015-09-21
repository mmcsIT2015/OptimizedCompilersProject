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
//			public BinaryNode binVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token CYCLE ASSIGN SEMICOLON PLUS MINUS DIV MULT LEFT_BR RIGHT_BR BEGIN END WHILE LARGER SMALLER IF ELSE 
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> ident expr 
%type <stVal> assign statement cycle condition
%type <blVal> stlist block
//%type <binVal> binExpr
%%

progr   : block { root = $1; }
		;

//binExpr : T operation F {$$ = new BinaryNode($1,$3,$2)}
//		;
		
//operation : PLUS
//			| MINUS
//			| MULT
//			| DIV
//		;
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
		| condition {$$ = $1; }
		| cycle   { $$ = $1; }
	;

condition : IF expr block
		| IF expr block ELSE block
		| IF expr block ELSE expr
		;
		
ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr{ $$ = new AssignNode($1 as IdNode, $3); }
		;

expr : expr SMALLER E
	| expr LARGER E
	| E
	;
	
E : T
     | E PLUS T
     | E MINUS T
     ;


T    : F
     | T MULT F
     | T DIV F 
     ;

F    : ident
     | INUM 
     | LEFT_BR expr RIGHT_BR
     ;
	 
block	: BEGIN stlist END { $$ = $2; }
		;
	 


cycle	: CYCLE expr statement { $$ = new CycleNode($2, $3); }
		;
	
%%

