// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.3.6
// Machine:  DESKTOP-U94NV7T
// DateTime: 02.10.2015 18:00:41
// UserName: jedik
// Input file <SimpleYacc.y>

// options: no-lines gplex

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using QUT.Gppg;
using ProgramTree;

namespace SimpleParser
{
public enum Tokens {
    error=1,EOF=2,ASSIGN=3,SEMICOLON=4,PLUS=5,MINUS=6,
    MUL=7,DIV=8,LBRACKET=9,RBRACKET=10,BEGIN=11,END=12,
    IF=13,ELSE=14,WHILE=15,DO=16,LESS=17,GREAT=18,
    EQUAL=19,INEQUAL=20,LSHIFT=21,COUT=22,INUM=23,RNUM=24,
    ID=25};

public struct ValueType
{
			public double dVal;
			public int iVal;
			public string sVal;
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
			public CoutNode ioVal;
        }
// Abstract base class for GPLEX scanners
public abstract class ScanBase : AbstractScanner<ValueType,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

public class Parser: ShiftReduceParser<ValueType, LexLocation>
{
  // Verbatim content from SimpleYacc.y
    public BlockNode root;
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
  // End verbatim content from SimpleYacc.y

#pragma warning disable 649
  private static Dictionary<int, string> aliasses;
#pragma warning restore 649
  private static Rule[] rules = new Rule[34];
  private static State[] states = new State[69];
  private static string[] nonTerms = new string[] {
      "expr", "ident", "T", "F", "S", "assign", "statement", "do_while", "while", 
      "if", "stlist", "block", "cout", "progr", "$accept", };

  static Parser() {
    states[0] = new State(new int[]{11,4},new int[]{-14,1,-12,3});
    states[1] = new State(new int[]{2,2});
    states[2] = new State(-1);
    states[3] = new State(-2);
    states[4] = new State(new int[]{25,20,11,4,16,41,15,48,22,57,13,61},new int[]{-11,5,-7,68,-6,8,-2,10,-12,38,-8,39,-9,47,-13,53,-10,60});
    states[5] = new State(new int[]{12,6,25,20,11,4,16,41,15,48,22,57,13,61},new int[]{-7,7,-6,8,-2,10,-12,38,-8,39,-9,47,-13,53,-10,60});
    states[6] = new State(-31);
    states[7] = new State(-4);
    states[8] = new State(new int[]{4,9});
    states[9] = new State(-7);
    states[10] = new State(new int[]{3,11});
    states[11] = new State(new int[]{25,20,23,21,9,22},new int[]{-1,12,-5,37,-3,32,-4,31,-2,19});
    states[12] = new State(new int[]{17,13,18,25,19,33,20,35,4,-14});
    states[13] = new State(new int[]{25,20,23,21,9,22},new int[]{-5,14,-3,32,-4,31,-2,19});
    states[14] = new State(new int[]{5,15,6,27,17,-16,18,-16,19,-16,20,-16,4,-16,10,-16,21,-16});
    states[15] = new State(new int[]{25,20,23,21,9,22},new int[]{-3,16,-4,31,-2,19});
    states[16] = new State(new int[]{7,17,8,29,5,-21,6,-21,17,-21,18,-21,19,-21,20,-21,4,-21,10,-21,21,-21});
    states[17] = new State(new int[]{25,20,23,21,9,22},new int[]{-4,18,-2,19});
    states[18] = new State(-24);
    states[19] = new State(-26);
    states[20] = new State(-13);
    states[21] = new State(-27);
    states[22] = new State(new int[]{25,20,23,21,9,22},new int[]{-1,23,-5,37,-3,32,-4,31,-2,19});
    states[23] = new State(new int[]{10,24,17,13,18,25,19,33,20,35});
    states[24] = new State(-28);
    states[25] = new State(new int[]{25,20,23,21,9,22},new int[]{-5,26,-3,32,-4,31,-2,19});
    states[26] = new State(new int[]{5,15,6,27,17,-17,18,-17,19,-17,20,-17,4,-17,10,-17,21,-17});
    states[27] = new State(new int[]{25,20,23,21,9,22},new int[]{-3,28,-4,31,-2,19});
    states[28] = new State(new int[]{7,17,8,29,5,-22,6,-22,17,-22,18,-22,19,-22,20,-22,4,-22,10,-22,21,-22});
    states[29] = new State(new int[]{25,20,23,21,9,22},new int[]{-4,30,-2,19});
    states[30] = new State(-25);
    states[31] = new State(-23);
    states[32] = new State(new int[]{7,17,8,29,5,-20,6,-20,17,-20,18,-20,19,-20,20,-20,4,-20,10,-20,21,-20});
    states[33] = new State(new int[]{25,20,23,21,9,22},new int[]{-5,34,-3,32,-4,31,-2,19});
    states[34] = new State(new int[]{5,15,6,27,17,-18,18,-18,19,-18,20,-18,4,-18,10,-18,21,-18});
    states[35] = new State(new int[]{25,20,23,21,9,22},new int[]{-5,36,-3,32,-4,31,-2,19});
    states[36] = new State(new int[]{5,15,6,27,17,-19,18,-19,19,-19,20,-19,4,-19,10,-19,21,-19});
    states[37] = new State(new int[]{5,15,6,27,17,-15,18,-15,19,-15,20,-15,4,-15,10,-15,21,-15});
    states[38] = new State(-8);
    states[39] = new State(new int[]{4,40});
    states[40] = new State(-9);
    states[41] = new State(new int[]{25,20,11,4,16,41,15,48,22,57,13,61},new int[]{-7,42,-6,8,-2,10,-12,38,-8,39,-9,47,-13,53,-10,60});
    states[42] = new State(new int[]{15,43});
    states[43] = new State(new int[]{9,44});
    states[44] = new State(new int[]{25,20,23,21,9,22},new int[]{-1,45,-5,37,-3,32,-4,31,-2,19});
    states[45] = new State(new int[]{10,46,17,13,18,25,19,33,20,35});
    states[46] = new State(-32);
    states[47] = new State(-10);
    states[48] = new State(new int[]{9,49});
    states[49] = new State(new int[]{25,20,23,21,9,22},new int[]{-1,50,-5,37,-3,32,-4,31,-2,19});
    states[50] = new State(new int[]{10,51,17,13,18,25,19,33,20,35});
    states[51] = new State(new int[]{25,20,11,4,16,41,15,48,22,57,13,61},new int[]{-7,52,-6,8,-2,10,-12,38,-8,39,-9,47,-13,53,-10,60});
    states[52] = new State(-33);
    states[53] = new State(new int[]{4,54,21,55});
    states[54] = new State(-11);
    states[55] = new State(new int[]{25,20,23,21,9,22},new int[]{-1,56,-5,37,-3,32,-4,31,-2,19});
    states[56] = new State(new int[]{17,13,18,25,19,33,20,35,4,-6,21,-6});
    states[57] = new State(new int[]{21,58});
    states[58] = new State(new int[]{25,20,23,21,9,22},new int[]{-1,59,-5,37,-3,32,-4,31,-2,19});
    states[59] = new State(new int[]{17,13,18,25,19,33,20,35,4,-5,21,-5});
    states[60] = new State(-12);
    states[61] = new State(new int[]{9,62});
    states[62] = new State(new int[]{25,20,23,21,9,22},new int[]{-1,63,-5,37,-3,32,-4,31,-2,19});
    states[63] = new State(new int[]{10,64,17,13,18,25,19,33,20,35});
    states[64] = new State(new int[]{25,20,11,4,16,41,15,48,22,57,13,61},new int[]{-7,65,-6,8,-2,10,-12,38,-8,39,-9,47,-13,53,-10,60});
    states[65] = new State(new int[]{14,66,12,-29,25,-29,11,-29,16,-29,15,-29,22,-29,13,-29});
    states[66] = new State(new int[]{25,20,11,4,16,41,15,48,22,57,13,61},new int[]{-7,67,-6,8,-2,10,-12,38,-8,39,-9,47,-13,53,-10,60});
    states[67] = new State(-30);
    states[68] = new State(-3);

    rules[1] = new Rule(-15, new int[]{-14,2});
    rules[2] = new Rule(-14, new int[]{-12});
    rules[3] = new Rule(-11, new int[]{-7});
    rules[4] = new Rule(-11, new int[]{-11,-7});
    rules[5] = new Rule(-13, new int[]{22,21,-1});
    rules[6] = new Rule(-13, new int[]{-13,21,-1});
    rules[7] = new Rule(-7, new int[]{-6,4});
    rules[8] = new Rule(-7, new int[]{-12});
    rules[9] = new Rule(-7, new int[]{-8,4});
    rules[10] = new Rule(-7, new int[]{-9});
    rules[11] = new Rule(-7, new int[]{-13,4});
    rules[12] = new Rule(-7, new int[]{-10});
    rules[13] = new Rule(-2, new int[]{25});
    rules[14] = new Rule(-6, new int[]{-2,3,-1});
    rules[15] = new Rule(-1, new int[]{-5});
    rules[16] = new Rule(-1, new int[]{-1,17,-5});
    rules[17] = new Rule(-1, new int[]{-1,18,-5});
    rules[18] = new Rule(-1, new int[]{-1,19,-5});
    rules[19] = new Rule(-1, new int[]{-1,20,-5});
    rules[20] = new Rule(-5, new int[]{-3});
    rules[21] = new Rule(-5, new int[]{-5,5,-3});
    rules[22] = new Rule(-5, new int[]{-5,6,-3});
    rules[23] = new Rule(-3, new int[]{-4});
    rules[24] = new Rule(-3, new int[]{-3,7,-4});
    rules[25] = new Rule(-3, new int[]{-3,8,-4});
    rules[26] = new Rule(-4, new int[]{-2});
    rules[27] = new Rule(-4, new int[]{23});
    rules[28] = new Rule(-4, new int[]{9,-1,10});
    rules[29] = new Rule(-10, new int[]{13,9,-1,10,-7});
    rules[30] = new Rule(-10, new int[]{13,9,-1,10,-7,14,-7});
    rules[31] = new Rule(-12, new int[]{11,-11,12});
    rules[32] = new Rule(-8, new int[]{16,-7,15,9,-1,10});
    rules[33] = new Rule(-9, new int[]{15,9,-1,10,-7});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Tokens.error, (int)Tokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
    switch (action)
    {
      case 2: // progr -> block
{ root = ValueStack[ValueStack.Depth-1].blVal; }
        break;
      case 3: // stlist -> statement
{ CurrentSemanticValue.blVal = new BlockNode(ValueStack[ValueStack.Depth-1].stVal);	}
        break;
      case 4: // stlist -> stlist, statement
{
				ValueStack[ValueStack.Depth-2].blVal.Add(ValueStack[ValueStack.Depth-1].stVal);
				CurrentSemanticValue.blVal = ValueStack[ValueStack.Depth-2].blVal;
			}
        break;
      case 5: // cout -> COUT, LSHIFT, expr
{ CurrentSemanticValue.ioVal = new CoutNode(ValueStack[ValueStack.Depth-1].eVal); }
        break;
      case 6: // cout -> cout, LSHIFT, expr
{
    		ValueStack[ValueStack.Depth-3].ioVal.Add(ValueStack[ValueStack.Depth-1].eVal);
    		CurrentSemanticValue.ioVal = ValueStack[ValueStack.Depth-3].ioVal;
    	}
        break;
      case 7: // statement -> assign, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].stVal; }
        break;
      case 8: // statement -> block
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].blVal; }
        break;
      case 9: // statement -> do_while, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].stVal; }
        break;
      case 10: // statement -> while
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal; }
        break;
      case 11: // statement -> cout, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].ioVal; }
        break;
      case 12: // statement -> if
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal; }
        break;
      case 13: // ident -> ID
{ CurrentSemanticValue.eVal = new IdNode(ValueStack[ValueStack.Depth-1].sVal); }
        break;
      case 14: // assign -> ident, ASSIGN, expr
{ CurrentSemanticValue.stVal = new AssignNode(ValueStack[ValueStack.Depth-3].eVal as IdNode, ValueStack[ValueStack.Depth-1].eVal); }
        break;
      case 15: // expr -> S
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 16: // expr -> expr, LESS, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryType.Less); }
        break;
      case 17: // expr -> expr, GREAT, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryType.More); }
        break;
      case 18: // expr -> expr, EQUAL, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryType.Equal); }
        break;
      case 19: // expr -> expr, INEQUAL, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryType.NotEqual); }
        break;
      case 20: // S -> T
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 21: // S -> S, PLUS, T
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryType.Plus); }
        break;
      case 22: // S -> S, MINUS, T
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryType.Minus); }
        break;
      case 23: // T -> F
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 24: // T -> T, MUL, F
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryType.Mult); }
        break;
      case 25: // T -> T, DIV, F
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryType.Div); }
        break;
      case 26: // F -> ident
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal as IdNode; }
        break;
      case 27: // F -> INUM
{ CurrentSemanticValue.eVal = new IntNumNode(ValueStack[ValueStack.Depth-1].iVal); }
        break;
      case 28: // F -> LBRACKET, expr, RBRACKET
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-2].eVal; }
        break;
      case 29: // if -> IF, LBRACKET, expr, RBRACKET, statement
{ CurrentSemanticValue.stVal = new IfNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].stVal); }
        break;
      case 30: // if -> IF, LBRACKET, expr, RBRACKET, statement, ELSE, statement
{ CurrentSemanticValue.stVal = new IfNode(ValueStack[ValueStack.Depth-5].eVal, ValueStack[ValueStack.Depth-3].stVal, ValueStack[ValueStack.Depth-1].stVal); }
        break;
      case 31: // block -> BEGIN, stlist, END
{ CurrentSemanticValue.blVal = ValueStack[ValueStack.Depth-2].blVal; }
        break;
      case 32: // do_while -> DO, statement, WHILE, LBRACKET, expr, RBRACKET
{ CurrentSemanticValue.stVal = new DoWhileNode(ValueStack[ValueStack.Depth-2].eVal, ValueStack[ValueStack.Depth-5].stVal); }
        break;
      case 33: // while -> WHILE, LBRACKET, expr, RBRACKET, statement
{ CurrentSemanticValue.stVal = new WhileNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].stVal); }
        break;
    }
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliasses != null && aliasses.ContainsKey(terminal))
        return aliasses[terminal];
    else if (((Tokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Tokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

}
}
