%{
    public BlockNode root;
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
       }

%using ProgramTree;

%namespace SimpleParser

%token LBR RBR ASG SM DO WHL IF ELS PL MN ML DV LT GT EQ IEQ LTE GTE LPR RPR SM
%token <iVal> INUM
%token <dVal> RNUM
%token <sVal> ID
%token <sVal> BOOL INT DOUBLE

%type <eVal> ident expr S E T F
%type <stVal> statement assign declaration definition ifelse while dowhile
%type <blVal> stlist block

%%

progr       : block { root = $1; }
            ;

stlist	    : statement { $$ = new BlockNode($1); }
            | stlist statement
                {
                    $1.Add($2);
                    $$ = $1;
                }
            ;




statement   : declaration SM { $$ = $1; }
            | definition SM { $$ = $1; }
            | assign SM { $$ = $1; }
            | block { $$ = $1; }
            | ifelse { $$ = $1; }
            | while { $$ = $1; }
            | dowhile SM { $$ = $1; }
            ;

assign      : ident ASG expr { $$ = new AssignNode($1 as IdNode, $3); }
            ;
            
definition  : declaration { $$ = $1; }
            ;
            
declaration : BOOL ident { $$ = new VarNode(VarType.Bool, $2 as IdNode); }
            | INT ident { $$ = new VarNode(VarType.Int, $2 as IdNode); }
            | DOUBLE ident { $$ = new VarNode(VarType.Double, $2 as IdNode); }
            ;

block	    : LBR stlist RBR { $$ = $2; }
            ;

ifelse      : IF LPR expr RPR statement { $$ = new IfNode($3, $5); }
            | IF LPR expr RPR statement ELS statement { $$ = new IfNode($3, $5, $7); }
            ;

while       : WHL LPR expr RPR statement { $$ = new WhileNode($3, $5); }
            ;

dowhile     : DO statement WHL LPR expr RPR { $$ = new DoWhileNode($5, $2); }
            ;

            
            

            
ident 	    : ID { $$ = new IdNode($1); }
            ;

expr        : S { $$ = $1; }
            ;

S           : S EQ E { $$ = new BinaryNode($1, $3, BinaryType.Equal); }
            | S GT E { $$ = new BinaryNode($1, $3, BinaryType.More); }
            | S LT E { $$ = new BinaryNode($1, $3, BinaryType.Less); }
            | S GTE E { $$ = new BinaryNode($1, $3, BinaryType.MoreEqual); }
            | S LTE E { $$ = new BinaryNode($1, $3, BinaryType.LessEqual); }
            | E { $$ = $1; }
            ;

E           : E PL T { $$ = new BinaryNode($1, $3, BinaryType.Plus); }
            | E MN T { $$ = new BinaryNode($1, $3, BinaryType.Minus); }
            | T { $$ = $1; }
            ;

T           : T ML F { $$ = new BinaryNode($1, $3, BinaryType.Mult); }
            | T DV F { $$ = new BinaryNode($1, $3, BinaryType.Div); }
            | F { $$ = $1; }
            ;

F	        : ident  { $$ = $1; }
            | INUM { $$ = new IntNumNode($1); }
            | RNUM { $$ = new FloatNumNode($1); }
            | LPR expr RPR { $$ = $2; }
            ;

%%

