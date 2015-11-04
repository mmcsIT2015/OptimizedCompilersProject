%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9]
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
ID {Alpha}{AlphaDigit}*

%%

{INTNUM} { 
  bool isIntNum = int.TryParse(yytext, out yylval.iVal); 
  if (isIntNum) {
	return (int)Tokens.INUM; 
  }

  throw new SyntaxException(yytext + " не является числом типа `INTNUM`");
}

{REALNUM} { 
  bool isRealNum = double.TryParse(yytext, out yylval.dVal); 
  if (isRealNum) {
	return (int)Tokens.RNUM; 
  }
  
  throw new SyntaxException(yytext + " не является числом типа `REALNUM`");
}

{ID}  {
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID)
	yylval.sVal = yytext;
  return res;
}

"{" { return (int)Tokens.BEGIN; }
"}" { return (int)Tokens.END; }
"(" { return (int)Tokens.LBRACKET; }
")" { return (int)Tokens.RBRACKET; }
"+" { return (int)Tokens.PLUS; }
"-" { return (int)Tokens.MINUS; }
"*" { return (int)Tokens.MUL; }
"/" { return (int)Tokens.DIV; }
"=" { return (int)Tokens.ASSIGN; }
";" { return (int)Tokens.SEMICOLON; }
"<" { return (int)Tokens.LESS; }
"<<" { return (int)Tokens.LSHIFT; }
">" { return (int)Tokens.GREAT; }
"==" { return (int)Tokens.EQUAL; }
"!=" { return (int)Tokens.INEQUAL; }
"," { return (int)Tokens.COMMA; }

[^\t \r\n] {
	LexError();
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol);
%}

%%

public override void yyerror(string format, params object[] args) // обработка синтаксических ошибок
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): Встречено {2}, а ожидалось {3}", yyline, yycol, args[0], string.Join(" или ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
  string errorMsg = string.Format("({0},{1}): Неизвестный символ {2}", yyline, yycol, yytext);
  throw new LexException(errorMsg);
}

class ScannerHelper
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper()
  {
    keywords = new Dictionary<string,int>();
    keywords.Add("do",(int)Tokens.DO);
    keywords.Add("cout",(int)Tokens.COUT);
    keywords.Add("if",(int)Tokens.IF);
    keywords.Add("else",(int)Tokens.ELSE);
    keywords.Add("while",(int)Tokens.WHILE);
  }
  public static int GetIDToken(string s)
  {
	if (keywords.ContainsKey(s))
	  return keywords[s];
	else
      return (int)Tokens.ID;
  }

}
