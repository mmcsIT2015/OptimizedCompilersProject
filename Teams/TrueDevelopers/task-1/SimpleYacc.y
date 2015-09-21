%{
// ��� ���������� ����������� � ����� GPPGParser, �������������� ����� ������, ������������ �������� gppg
    public BlockNode root; // �������� ���� ��������������� ������
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
			public CoutNode ioVal;
        }

%using ProgramTree;

%namespace SimpleParser

%token ASSIGN SEMICOLON PLUS MINUS MUL DIV LBRACKET RBRACKET BEGIN END IF ELSE WHILE DO LESS GREAT EQUAL INEQUAL LSHIFT COUT
%token <iVal> INUM
%token <dVal> RNUM
%token <sVal> ID

%type <eVal> expr ident T F S
%type <stVal> assign statement do_while while if
%type <blVal> stlist block
%type <ioVal> cout

%%

progr   : block { root = $1; }
		;

stlist	: statement { $$ = new BlockNode($1);	}
		| stlist SEMICOLON statement
			{
				$1.Add($3);
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

statement: assign { $$ = $1; }
    | block { $$ = $1; }
		| do_while { $$ = $1; }
		| while { $$ = $1; }
		| cout { $$ = $1; }
    | if { $$ = $1; }
    ;

ident 	: ID { $$ = new IdNode($1); }
		;

assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr : S { $$ = $1; }
    | expr LESS S { $$ = new BinaryNode($1, $3, BinaryType.Less); }
    | expr GREAT S { $$ = new BinaryNode($1, $3, BinaryType.More); }
    | expr EQUAL S { $$ = new BinaryNode($1, $3, BinaryType.Equal); }
    | expr INEQUAL S { $$ = new BinaryNode($1, $3, BinaryType.NotEqual); }
    ;

S : T { $$ = $1; }
    | S PLUS T { $$ = new BinaryNode($1, $3, BinaryType.Plus); }
    | S MINUS T { $$ = new BinaryNode($1, $3, BinaryType.Minus); }
    ;

T : F { $$ = $1; }
    | T MUL F { $$ = new BinaryNode($1, $3, BinaryType.Mult); }
    | T DIV F { $$ = new BinaryNode($1, $3, BinaryType.Div); }
    ;

F : ident { $$ = $1 as IdNode; }
    | INUM { $$ = new IntNumNode($1); }
    | LBRACKET expr RBRACKET { $$ = $2; }
    ;

if	: IF LBRACKET expr RBRACKET statement { $$ = new IfNode($3, $5); }
    | IF LBRACKET expr RBRACKET statement ELSE statement { $$ = new IfNode($3, $5, $7); }
    ;

block	: BEGIN stlist END { $$ = $2; }
		;

do_while	: DO statement WHILE LBRACKET expr RBRACKET SEMICOLON { $$ = new DoWhileNode($5, $2); }
		;

while	: WHILE LBRACKET expr RBRACKET statement { $$ = new WhileNode($3, $5); }
    ;

%%
