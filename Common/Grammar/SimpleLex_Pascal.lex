%using CompilerExceptions;
%using QUT.Gppg;
%using SimpleParserPascal;
%using System.Linq;

%namespace SimpleScannerPascal

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
ID {Alpha}{AlphaDigit}* 
TYPEDESC integer|float|string|boolean

STRING_LITERAL \"([^\"]|\\.)*\"

%%

{INTNUM} { 
	bool isIntNum = int.TryParse(yytext, out yylval.iVal); 
	if (isIntNum) {
		return (int)Tokens.INUM; 
	}

	throw new SyntaxException(yytext + " isn't a value of type `INTNUM`");
}

{STRING_LITERAL} {
	return (int)Tokens.STRING_L;
}

{TYPEDESC}	{
	yylval.typeVal = yytext;
	return (int)Tokens.TYPE;		
}

{REALNUM} { 
	var style = System.Globalization.NumberStyles.Any;
    NumberFormatInfo nfi = new NumberFormatInfo();
    nfi.NumberDecimalSeparator = ".";
	bool isRealNum = double.TryParse(yytext, style, nfi, out yylval.dVal); 
	if (isRealNum) {
		return (int)Tokens.RNUM; 
	}
  
	throw new SyntaxException(yytext + " isn't a value of type `REALNUM`");
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID)
	yylval.sVal = yytext;
  return res;
}

":=" { return (int)Tokens.ASSIGN; }
";" { return (int)Tokens.SEMICOLON; }
"+" { return (int)Tokens.PLUS; }
"-" { return (int)Tokens.MINUS; }
"*" { return (int)Tokens.PROD; }
"/" { return (int)Tokens.DIV; }
"(" { return (int)Tokens.LB; }
")" { return (int)Tokens.RB; }
"," { return (int)Tokens.COMMA; }
"<" { return (int)Tokens.LESS; }
">" { return (int)Tokens.MORE; }
"<=" { return (int)Tokens.LESSEQUAL; }
">=" { return (int)Tokens.MOREEQUAL; }
"=" { return (int)Tokens.EQUAL; }
"<>" { return (int)Tokens.NOTEQUAL; }
"." { return (int)Tokens.POINT; }
":" { return (int)Tokens.COLON; }

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
  string errorMsg = string.Format("({0},{1}): Encountered {2}, but expected {3}.", yyline, yycol, args[0], string.Join(" OR ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
  string errorMsg = string.Format("({0},{1}): Unknown symbol: {2}", yyline, yycol, yytext);
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
	
	keywords.Add("if",(int)Tokens.IF);
	keywords.Add("else",(int)Tokens.ELSE);
	keywords.Add("then",(int)Tokens.THEN);
	
	keywords.Add("while",(int)Tokens.WHILE);
	keywords.Add("do",(int)Tokens.DO);
	
	keywords.Add("repeat",(int)Tokens.REPEAT);
	keywords.Add("until",(int)Tokens.UNTIL);
	
	keywords.Add("not",(int)Tokens.NOT);
	keywords.Add("goto", (int)Tokens.GOTO);
	keywords.Add("var", (int)Tokens.VAR);
	
  }
  public static int GetIDToken(string s)
  {
	if (keywords.ContainsKey(s.ToLower()))
	  return keywords[s];
	else
      return (int)Tokens.ID;
  }
  
}
