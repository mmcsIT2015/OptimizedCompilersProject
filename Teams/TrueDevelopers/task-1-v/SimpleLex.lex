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
  yylval.iVal = int.Parse(yytext);
  return (int)Tokens.INUM;
}

{REALNUM} {
  yylval.dVal = double.Parse(yytext);
  return (int)Tokens.RNUM;
}

{ID}  {
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID)
	yylval.sVal = yytext;
  return res;
}

"{" { return (int)Tokens.LBR; }
"}" { return (int)Tokens.RBR; }
"=" { return (int)Tokens.ASG; }
";" { return (int)Tokens.SM; }
"+" { return (int)Tokens.PL; }
"-" { return (int)Tokens.MN; }
"*" { return (int)Tokens.ML; }
"/" { return (int)Tokens.DV; }
"<" { return (int)Tokens.LT; }
">" { return (int)Tokens.GT; }
"==" { return (int)Tokens.EQ; }
"<=" { return (int)Tokens.LTE; }
">=" { return (int)Tokens.GTE; }
"(" { return (int)Tokens.LPR; }
")" { return (int)Tokens.RPR; }

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
    keywords.Add("if",(int)Tokens.IF);
    keywords.Add("else",(int)Tokens.ELS);
    keywords.Add("do",(int)Tokens.DO);
    keywords.Add("while",(int)Tokens.WHL);
  }
  public static int GetIDToken(string s)
  {
	if (keywords.ContainsKey(s.ToLower()))
	  return keywords[s];
	else
      return (int)Tokens.ID;
  }

}
