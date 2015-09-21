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

%token LBR RBR ASG SM DO WHL IF ELS PL MN ML DV LT GT EQ LTE GTE LPR RPR SM
%token <iVal> INUM
%token <dVal> RNUM
%token <sVal> ID

%type <eVal> ident expr Q S E T F
%type <stVal> statement if ifelse while dowhile
%type <blVal> stlist block

%%

progr       : block { root = $1; }
            ;

stlist	    : statement
              {
                $$ = new BlockNode($1);
              }
            | stlist statement
              {
                $1.Add($2);
                $$ = $1;
              }
            ;



// STATEMENTS

statement   : expr SM
            | block
            | if
            | ifelse
            | while
            | dowhile SM
            ;

block	    : LBR stlist RBR
            ;

if          : IF LPR expr RPR statement;

    // produces Reduce/Reduce warnings
ifelse      : if
            | if ELS statement
            ;

while       : WHL LPR expr RPR statement
            ;

dowhile     : DO statement WHL LPR expr RPR
            ;




// EXPRESIONS

ident 	    : ID { $$ = new IdNode($1); }
            ;

expr        : Q;

Q           : S
            | ident ASG S //{ $$ = new ExprNode($1 as IdNode, $3); }
            ;


S           : E
            | S EQ E
            | S GT E
            | S LT E
            | S GTE E
            | S LTE E
            ;

E           : T
            | E PL T
            | E MN T
            ;

T           : F
            | T ML F
            | T DV F
            ;

F	        : ident  { $$ = $1 as IdNode; }
            | INUM { $$ = new IntNumNode($1); }
            | LPR S RPR
            ;

%%

