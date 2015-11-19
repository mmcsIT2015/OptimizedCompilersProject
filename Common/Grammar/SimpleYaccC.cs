// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.3.6
// Machine:  DESKTOP-9C48I3Q
// DateTime: 20.11.2015 0:13:37
// UserName: alexey
// Input file <SimpleYacc_C.y>

// options: no-lines gplex

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using QUT.Gppg;
using ProgramTree;

namespace SimpleParserC
{
public enum Tokens {
    error=1,EOF=2,ASSIGN=3,SEMICOLON=4,PLUS=5,MINUS=6,
    MUL=7,DIV=8,LBRACKET=9,RBRACKET=10,BEGIN=11,END=12,
    IF=13,ELSE=14,WHILE=15,DO=16,LESS=17,GREAT=18,
    EQUAL=19,INEQUAL=20,LSHIFT=21,COUT=22,COMMA=23,NOT=24,
    INUM=25,RNUM=26,ID=27};

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
			public FunctionNode funVal;
			public FunctionNodeSt funStVal;
			public List<ExprNode> paramVal;
        }
// Abstract base class for GPLEX scanners
public abstract class ScanBase : AbstractScanner<ValueType,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

public class Parser: ShiftReduceParser<ValueType, LexLocation>
{
  // Verbatim content from SimpleYacc_C.y
    public BlockNode root;
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
  // End verbatim content from SimpleYacc_C.y

#pragma warning disable 649
  private static Dictionary<int, string> aliasses;
#pragma warning restore 649
  private static Rule[] rules = new Rule[43];
  private static State[] states = new State[84];
  private static string[] nonTerms = new string[] {
      "expr", "ident", "T", "F", "S", "U", "assign", "statement", "do_while", 
      "while", "if", "stlist", "block", "cout", "funcall", "funcallst", "params", 
      "progr", "$accept", };

  static Parser() {
    states[0] = new State(new int[]{11,4},new int[]{-18,1,-13,3});
    states[1] = new State(new int[]{2,2});
    states[2] = new State(-1);
    states[3] = new State(-2);
    states[4] = new State(new int[]{27,21,11,4,16,53,15,60,22,69,13,73},new int[]{-12,5,-8,83,-7,8,-2,10,-13,50,-9,51,-10,59,-14,65,-11,72,-16,80,-15,82});
    states[5] = new State(new int[]{12,6,27,21,11,4,16,53,15,60,22,69,13,73},new int[]{-8,7,-7,8,-2,10,-13,50,-9,51,-10,59,-14,65,-11,72,-16,80,-15,82});
    states[6] = new State(-40);
    states[7] = new State(-4);
    states[8] = new State(new int[]{4,9});
    states[9] = new State(-9);
    states[10] = new State(new int[]{3,11});
    states[11] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-1,12,-5,48,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[12] = new State(new int[]{17,13,18,27,19,37,20,46,4,-19});
    states[13] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-5,14,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[14] = new State(new int[]{5,15,6,29,17,-21,18,-21,19,-21,20,-21,4,-21,10,-21,23,-21,21,-21});
    states[15] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-3,16,-6,40,-4,19,-2,20,-15,41});
    states[16] = new State(new int[]{7,17,8,31,5,-26,6,-26,17,-26,18,-26,19,-26,20,-26,4,-26,10,-26,23,-26,21,-26});
    states[17] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-6,18,-4,19,-2,20,-15,41});
    states[18] = new State(-29);
    states[19] = new State(-31);
    states[20] = new State(-34);
    states[21] = new State(new int[]{9,22,3,-18,7,-18,8,-18,5,-18,6,-18,17,-18,18,-18,19,-18,20,-18,4,-18,10,-18,23,-18,21,-18});
    states[22] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-17,23,-1,49,-5,48,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[23] = new State(new int[]{10,24,23,25});
    states[24] = new State(-17);
    states[25] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-1,26,-5,48,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[26] = new State(new int[]{17,13,18,27,19,37,20,46,10,-8,23,-8});
    states[27] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-5,28,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[28] = new State(new int[]{5,15,6,29,17,-22,18,-22,19,-22,20,-22,4,-22,10,-22,23,-22,21,-22});
    states[29] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-3,30,-6,40,-4,19,-2,20,-15,41});
    states[30] = new State(new int[]{7,17,8,31,5,-27,6,-27,17,-27,18,-27,19,-27,20,-27,4,-27,10,-27,23,-27,21,-27});
    states[31] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-6,32,-4,19,-2,20,-15,41});
    states[32] = new State(-30);
    states[33] = new State(-35);
    states[34] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-1,35,-5,48,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[35] = new State(new int[]{10,36,17,13,18,27,19,37,20,46});
    states[36] = new State(-36);
    states[37] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-5,38,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[38] = new State(new int[]{5,15,6,29,17,-23,18,-23,19,-23,20,-23,4,-23,10,-23,23,-23,21,-23});
    states[39] = new State(new int[]{7,17,8,31,5,-25,6,-25,17,-25,18,-25,19,-25,20,-25,4,-25,10,-25,23,-25,21,-25});
    states[40] = new State(-28);
    states[41] = new State(-37);
    states[42] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-6,43,-4,19,-2,20,-15,41});
    states[43] = new State(-32);
    states[44] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-6,45,-4,19,-2,20,-15,41});
    states[45] = new State(-33);
    states[46] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-5,47,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[47] = new State(new int[]{5,15,6,29,17,-24,18,-24,19,-24,20,-24,4,-24,10,-24,23,-24,21,-24});
    states[48] = new State(new int[]{5,15,6,29,17,-20,18,-20,19,-20,20,-20,4,-20,10,-20,23,-20,21,-20});
    states[49] = new State(new int[]{17,13,18,27,19,37,20,46,10,-7,23,-7});
    states[50] = new State(-10);
    states[51] = new State(new int[]{4,52});
    states[52] = new State(-11);
    states[53] = new State(new int[]{27,21,11,4,16,53,15,60,22,69,13,73},new int[]{-8,54,-7,8,-2,10,-13,50,-9,51,-10,59,-14,65,-11,72,-16,80,-15,82});
    states[54] = new State(new int[]{15,55});
    states[55] = new State(new int[]{9,56});
    states[56] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-1,57,-5,48,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[57] = new State(new int[]{10,58,17,13,18,27,19,37,20,46});
    states[58] = new State(-41);
    states[59] = new State(-12);
    states[60] = new State(new int[]{9,61});
    states[61] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-1,62,-5,48,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[62] = new State(new int[]{10,63,17,13,18,27,19,37,20,46});
    states[63] = new State(new int[]{27,21,11,4,16,53,15,60,22,69,13,73},new int[]{-8,64,-7,8,-2,10,-13,50,-9,51,-10,59,-14,65,-11,72,-16,80,-15,82});
    states[64] = new State(-42);
    states[65] = new State(new int[]{4,66,21,67});
    states[66] = new State(-13);
    states[67] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-1,68,-5,48,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[68] = new State(new int[]{17,13,18,27,19,37,20,46,4,-6,21,-6});
    states[69] = new State(new int[]{21,70});
    states[70] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-1,71,-5,48,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[71] = new State(new int[]{17,13,18,27,19,37,20,46,4,-5,21,-5});
    states[72] = new State(-14);
    states[73] = new State(new int[]{9,74});
    states[74] = new State(new int[]{27,21,25,33,9,34,6,42,24,44},new int[]{-1,75,-5,48,-3,39,-6,40,-4,19,-2,20,-15,41});
    states[75] = new State(new int[]{10,76,17,13,18,27,19,37,20,46});
    states[76] = new State(new int[]{27,21,11,4,16,53,15,60,22,69,13,73},new int[]{-8,77,-7,8,-2,10,-13,50,-9,51,-10,59,-14,65,-11,72,-16,80,-15,82});
    states[77] = new State(new int[]{14,78,12,-38,27,-38,11,-38,16,-38,15,-38,22,-38,13,-38});
    states[78] = new State(new int[]{27,21,11,4,16,53,15,60,22,69,13,73},new int[]{-8,79,-7,8,-2,10,-13,50,-9,51,-10,59,-14,65,-11,72,-16,80,-15,82});
    states[79] = new State(-39);
    states[80] = new State(new int[]{4,81});
    states[81] = new State(-15);
    states[82] = new State(-16);
    states[83] = new State(-3);

    rules[1] = new Rule(-19, new int[]{-18,2});
    rules[2] = new Rule(-18, new int[]{-13});
    rules[3] = new Rule(-12, new int[]{-8});
    rules[4] = new Rule(-12, new int[]{-12,-8});
    rules[5] = new Rule(-14, new int[]{22,21,-1});
    rules[6] = new Rule(-14, new int[]{-14,21,-1});
    rules[7] = new Rule(-17, new int[]{-1});
    rules[8] = new Rule(-17, new int[]{-17,23,-1});
    rules[9] = new Rule(-8, new int[]{-7,4});
    rules[10] = new Rule(-8, new int[]{-13});
    rules[11] = new Rule(-8, new int[]{-9,4});
    rules[12] = new Rule(-8, new int[]{-10});
    rules[13] = new Rule(-8, new int[]{-14,4});
    rules[14] = new Rule(-8, new int[]{-11});
    rules[15] = new Rule(-8, new int[]{-16,4});
    rules[16] = new Rule(-16, new int[]{-15});
    rules[17] = new Rule(-15, new int[]{27,9,-17,10});
    rules[18] = new Rule(-2, new int[]{27});
    rules[19] = new Rule(-7, new int[]{-2,3,-1});
    rules[20] = new Rule(-1, new int[]{-5});
    rules[21] = new Rule(-1, new int[]{-1,17,-5});
    rules[22] = new Rule(-1, new int[]{-1,18,-5});
    rules[23] = new Rule(-1, new int[]{-1,19,-5});
    rules[24] = new Rule(-1, new int[]{-1,20,-5});
    rules[25] = new Rule(-5, new int[]{-3});
    rules[26] = new Rule(-5, new int[]{-5,5,-3});
    rules[27] = new Rule(-5, new int[]{-5,6,-3});
    rules[28] = new Rule(-3, new int[]{-6});
    rules[29] = new Rule(-3, new int[]{-3,7,-6});
    rules[30] = new Rule(-3, new int[]{-3,8,-6});
    rules[31] = new Rule(-6, new int[]{-4});
    rules[32] = new Rule(-6, new int[]{6,-6});
    rules[33] = new Rule(-6, new int[]{24,-6});
    rules[34] = new Rule(-4, new int[]{-2});
    rules[35] = new Rule(-4, new int[]{25});
    rules[36] = new Rule(-4, new int[]{9,-1,10});
    rules[37] = new Rule(-4, new int[]{-15});
    rules[38] = new Rule(-11, new int[]{13,9,-1,10,-8});
    rules[39] = new Rule(-11, new int[]{13,9,-1,10,-8,14,-8});
    rules[40] = new Rule(-13, new int[]{11,-12,12});
    rules[41] = new Rule(-9, new int[]{16,-8,15,9,-1,10});
    rules[42] = new Rule(-10, new int[]{15,9,-1,10,-8});
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
      case 7: // params -> expr
{ CurrentSemanticValue.paramVal = new List<ExprNode>(); CurrentSemanticValue.paramVal.Add(ValueStack[ValueStack.Depth-1].eVal); }
        break;
      case 8: // params -> params, COMMA, expr
{
         ValueStack[ValueStack.Depth-3].paramVal.Add(ValueStack[ValueStack.Depth-1].eVal);
			CurrentSemanticValue.paramVal = ValueStack[ValueStack.Depth-3].paramVal;
		}
        break;
      case 9: // statement -> assign, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].stVal; }
        break;
      case 10: // statement -> block
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].blVal; }
        break;
      case 11: // statement -> do_while, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].stVal; }
        break;
      case 12: // statement -> while
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal; }
        break;
      case 13: // statement -> cout, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].ioVal; }
        break;
      case 14: // statement -> if
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal; }
        break;
      case 15: // statement -> funcallst, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].funStVal; }
        break;
      case 16: // funcallst -> funcall
{ CurrentSemanticValue.funStVal = new FunctionNodeSt(); CurrentSemanticValue.funStVal.Function = ValueStack[ValueStack.Depth-1].funVal; }
        break;
      case 17: // funcall -> ID, LBRACKET, params, RBRACKET
{
			CurrentSemanticValue.funVal = new FunctionNode(ValueStack[ValueStack.Depth-4].sVal);
			CurrentSemanticValue.funVal.Parameters = ValueStack[ValueStack.Depth-2].paramVal;
		}
        break;
      case 18: // ident -> ID
{ CurrentSemanticValue.eVal = new IdNode(ValueStack[ValueStack.Depth-1].sVal); }
        break;
      case 19: // assign -> ident, ASSIGN, expr
{ CurrentSemanticValue.stVal = new AssignNode(ValueStack[ValueStack.Depth-3].eVal as IdNode, ValueStack[ValueStack.Depth-1].eVal); }
        break;
      case 20: // expr -> S
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 21: // expr -> expr, LESS, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryOperation.Less); }
        break;
      case 22: // expr -> expr, GREAT, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryOperation.Greater); }
        break;
      case 23: // expr -> expr, EQUAL, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryOperation.Equal); }
        break;
      case 24: // expr -> expr, INEQUAL, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryOperation.NotEqual); }
        break;
      case 25: // S -> T
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 26: // S -> S, PLUS, T
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryOperation.Plus); }
        break;
      case 27: // S -> S, MINUS, T
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryOperation.Minus); }
        break;
      case 28: // T -> U
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 29: // T -> T, MUL, U
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryOperation.Mult); }
        break;
      case 30: // T -> T, DIV, U
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, BinaryOperation.Div); }
        break;
      case 31: // U -> F
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 32: // U -> MINUS, U
{ CurrentSemanticValue.eVal = new UnaryNode(ValueStack[ValueStack.Depth-1].eVal, UnaryOperation.Minus); }
        break;
      case 33: // U -> NOT, U
{ CurrentSemanticValue.eVal = new UnaryNode(ValueStack[ValueStack.Depth-1].eVal, UnaryOperation.Not); }
        break;
      case 34: // F -> ident
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal as IdNode; }
        break;
      case 35: // F -> INUM
{ CurrentSemanticValue.eVal = new IntNumNode(ValueStack[ValueStack.Depth-1].iVal); }
        break;
      case 36: // F -> LBRACKET, expr, RBRACKET
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-2].eVal; }
        break;
      case 37: // F -> funcall
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].funVal; }
        break;
      case 38: // if -> IF, LBRACKET, expr, RBRACKET, statement
{ CurrentSemanticValue.stVal = new IfNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].stVal); }
        break;
      case 39: // if -> IF, LBRACKET, expr, RBRACKET, statement, ELSE, statement
{ CurrentSemanticValue.stVal = new IfNode(ValueStack[ValueStack.Depth-5].eVal, ValueStack[ValueStack.Depth-3].stVal, ValueStack[ValueStack.Depth-1].stVal); }
        break;
      case 40: // block -> BEGIN, stlist, END
{ CurrentSemanticValue.blVal = ValueStack[ValueStack.Depth-2].blVal; }
        break;
      case 41: // do_while -> DO, statement, WHILE, LBRACKET, expr, RBRACKET
{ CurrentSemanticValue.stVal = new DoWhileNode(ValueStack[ValueStack.Depth-2].eVal, ValueStack[ValueStack.Depth-5].stVal); }
        break;
      case 42: // while -> WHILE, LBRACKET, expr, RBRACKET, statement
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
