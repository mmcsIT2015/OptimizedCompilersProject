%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  [\+\-]?{Digit}+
REALNUM {INTNUM}\.{Digit}+
ID {Alpha}{AlphaDigit}* 

%%

{INTNUM} { 
  yylval.iVal = int.Parse(yytext); 
  return (int)Tokens.INUM; 
}

{REALNUM} { 
  //yylval.dVal = double.Parse(yytext, System.Globalization.CultureInfo("en-US")); 
  yylval.dVal = Convert.ToDouble(yytext, CultureInfo.GetCultureInfo("en-US"));
  return (int)Tokens.RNUM;
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID)
	yylval.sVal = yytext;
  return res;
}

":=" { return (int)Tokens.ASSIGN; }
"+=" { return (int)Tokens.ASSIGNPLUS; }
"-=" { return (int)Tokens.ASSIGNMINUS; }
"*=" { return (int)Tokens.ASSIGNMULT; }
"/=" { return (int)Tokens.ASSIGNDIVIDE; }
";" { return (int)Tokens.SEMICOLON; }
"+" { return (int)Tokens.PLUS; }
"-" { return (int)Tokens.MINUS; }
"*" { return (int)Tokens.MULT; }
"/" { return (int)Tokens.DIV; }
"(" { return (int)Tokens.LEFTBRACKET; }
")" { return (int)Tokens.RIGHTBRACKET; }
">" { return (int)Tokens.GREATER; }
"<" { return (int)Tokens.LESS; }
"=" { return (int)Tokens.EQUAL; }
"<>" { return (int)Tokens.NOTEQUAL; }
"," { return (int)Tokens.COMMA; }

[^ \r\n] {
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
    keywords.Add("begin",(int)Tokens.BEGIN);
    keywords.Add("end",(int)Tokens.END);
    keywords.Add("while",(int)Tokens.WHILE);
	keywords.Add("do",(int)Tokens.DO);
	keywords.Add("if",(int)Tokens.IF);
	keywords.Add("then",(int)Tokens.THEN);
	keywords.Add("else",(int)Tokens.ELSE);
	keywords.Add("repeat",(int)Tokens.REPEAT);
	keywords.Add("until",(int)Tokens.UNTIL);
  }
  public static int GetIDToken(string s)
  {
	var s1 = s.ToLower();
	if (keywords.ContainsKey(s1))
	  return keywords[s1];
	else
      return (int)Tokens.ID;
  }
  
}
