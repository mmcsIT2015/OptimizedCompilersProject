// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.3.6
// Machine:  ANTON-NOTEBOOK
// DateTime: 21.09.2015 17:50:16
// UserName: ?????
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
    error=1,EOF=2,BEGIN=3,END=4,CYCLE=5,ASSIGN=6,
    SEMICOLON=7,PLUS=8,MINUS=9,MULT=10,DIV=11,LEFTBRACKET=12,
    RIGHTBRACKET=13,GREATER=14,LESS=15,EQUAL=16,NOTEQUAL=17,IF=18,
    THEN=19,ELSE=20,INUM=21,RNUM=22,ID=23};

public struct ValueType
{ 
			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
			public PredicateBinaryNode prVal;
			public IfNode ifVal;
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
// ��� ���������� ����������� � ����� GPPGParser, �������������� ����� ������, ������������ �������� gppg
    public BlockNode root; // �������� ���� ��������������� ������ 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
  // End verbatim content from SimpleYacc.y

#pragma warning disable 649
  private static Dictionary<int, string> aliasses;
#pragma warning restore 649
  private static Rule[] rules = new Rule[30];
  private static State[] states = new State[53];
  private static string[] nonTerms = new string[] {
      "expr", "ident", "t", "f", "assign", "statement", "cycle", "stlist", "block", 
      "prexpr", "if", "progr", "$accept", };

  static Parser() {
    states[0] = new State(new int[]{3,4},new int[]{-12,1,-9,3});
    states[1] = new State(new int[]{2,2});
    states[2] = new State(-1);
    states[3] = new State(-2);
    states[4] = new State(new int[]{23,18,3,4,5,41,18,45},new int[]{-8,5,-6,52,-5,9,-2,10,-9,39,-7,40,-11,44});
    states[5] = new State(new int[]{4,6,7,7});
    states[6] = new State(-26);
    states[7] = new State(new int[]{23,18,3,4,5,41,18,45},new int[]{-6,8,-5,9,-2,10,-9,39,-7,40,-11,44});
    states[8] = new State(-4);
    states[9] = new State(-5);
    states[10] = new State(new int[]{6,11});
    states[11] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-1,12,-10,38,-3,29,-4,28,-2,17});
    states[12] = new State(new int[]{8,13,9,24,14,30,15,32,16,34,17,36,4,-10,7,-10,20,-10});
    states[13] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-3,14,-4,28,-2,17});
    states[14] = new State(new int[]{10,15,11,26,8,-17,9,-17,14,-17,15,-17,16,-17,17,-17,4,-17,7,-17,20,-17,13,-17,19,-17,23,-17,3,-17,5,-17,18,-17});
    states[15] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-4,16,-2,17});
    states[16] = new State(-20);
    states[17] = new State(-22);
    states[18] = new State(-9);
    states[19] = new State(-23);
    states[20] = new State(-24);
    states[21] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-1,22,-3,29,-4,28,-2,17});
    states[22] = new State(new int[]{13,23,8,13,9,24});
    states[23] = new State(-25);
    states[24] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-3,25,-4,28,-2,17});
    states[25] = new State(new int[]{10,15,11,26,8,-18,9,-18,14,-18,15,-18,16,-18,17,-18,4,-18,7,-18,20,-18,13,-18,19,-18,23,-18,3,-18,5,-18,18,-18});
    states[26] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-4,27,-2,17});
    states[27] = new State(-21);
    states[28] = new State(-19);
    states[29] = new State(new int[]{10,15,11,26,8,-16,9,-16,14,-16,15,-16,16,-16,17,-16,4,-16,7,-16,20,-16,13,-16,19,-16,23,-16,3,-16,5,-16,18,-16});
    states[30] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-1,31,-3,29,-4,28,-2,17});
    states[31] = new State(new int[]{8,13,9,24,4,-12,7,-12,20,-12,19,-12});
    states[32] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-1,33,-3,29,-4,28,-2,17});
    states[33] = new State(new int[]{8,13,9,24,4,-13,7,-13,20,-13,19,-13});
    states[34] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-1,35,-3,29,-4,28,-2,17});
    states[35] = new State(new int[]{8,13,9,24,4,-14,7,-14,20,-14,19,-14});
    states[36] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-1,37,-3,29,-4,28,-2,17});
    states[37] = new State(new int[]{8,13,9,24,4,-15,7,-15,20,-15,19,-15});
    states[38] = new State(-11);
    states[39] = new State(-6);
    states[40] = new State(-7);
    states[41] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-1,42,-3,29,-4,28,-2,17});
    states[42] = new State(new int[]{8,13,9,24,23,18,3,4,5,41,18,45},new int[]{-6,43,-5,9,-2,10,-9,39,-7,40,-11,44});
    states[43] = new State(-27);
    states[44] = new State(-8);
    states[45] = new State(new int[]{23,18,21,19,22,20,12,21},new int[]{-10,46,-1,51,-3,29,-4,28,-2,17});
    states[46] = new State(new int[]{19,47});
    states[47] = new State(new int[]{23,18,3,4,5,41,18,45},new int[]{-6,48,-5,9,-2,10,-9,39,-7,40,-11,44});
    states[48] = new State(new int[]{20,49,4,-28,7,-28});
    states[49] = new State(new int[]{23,18,3,4,5,41,18,45},new int[]{-6,50,-5,9,-2,10,-9,39,-7,40,-11,44});
    states[50] = new State(-29);
    states[51] = new State(new int[]{14,30,8,13,9,24,15,32,16,34,17,36});
    states[52] = new State(-3);

    rules[1] = new Rule(-13, new int[]{-12,2});
    rules[2] = new Rule(-12, new int[]{-9});
    rules[3] = new Rule(-8, new int[]{-6});
    rules[4] = new Rule(-8, new int[]{-8,7,-6});
    rules[5] = new Rule(-6, new int[]{-5});
    rules[6] = new Rule(-6, new int[]{-9});
    rules[7] = new Rule(-6, new int[]{-7});
    rules[8] = new Rule(-6, new int[]{-11});
    rules[9] = new Rule(-2, new int[]{23});
    rules[10] = new Rule(-5, new int[]{-2,6,-1});
    rules[11] = new Rule(-5, new int[]{-2,6,-10});
    rules[12] = new Rule(-10, new int[]{-1,14,-1});
    rules[13] = new Rule(-10, new int[]{-1,15,-1});
    rules[14] = new Rule(-10, new int[]{-1,16,-1});
    rules[15] = new Rule(-10, new int[]{-1,17,-1});
    rules[16] = new Rule(-1, new int[]{-3});
    rules[17] = new Rule(-1, new int[]{-1,8,-3});
    rules[18] = new Rule(-1, new int[]{-1,9,-3});
    rules[19] = new Rule(-3, new int[]{-4});
    rules[20] = new Rule(-3, new int[]{-3,10,-4});
    rules[21] = new Rule(-3, new int[]{-3,11,-4});
    rules[22] = new Rule(-4, new int[]{-2});
    rules[23] = new Rule(-4, new int[]{21});
    rules[24] = new Rule(-4, new int[]{22});
    rules[25] = new Rule(-4, new int[]{12,-1,13});
    rules[26] = new Rule(-9, new int[]{3,-8,4});
    rules[27] = new Rule(-7, new int[]{5,-1,-6});
    rules[28] = new Rule(-11, new int[]{18,-10,19,-6});
    rules[29] = new Rule(-11, new int[]{18,-10,19,-6,20,-6});
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
{ 
				CurrentSemanticValue.blVal = new BlockNode(ValueStack[ValueStack.Depth-1].stVal); 
			}
        break;
      case 4: // stlist -> stlist, SEMICOLON, statement
{ 
				ValueStack[ValueStack.Depth-3].blVal.Add(ValueStack[ValueStack.Depth-1].stVal); 
				CurrentSemanticValue.blVal = ValueStack[ValueStack.Depth-3].blVal; 
			}
        break;
      case 5: // statement -> assign
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal; }
        break;
      case 6: // statement -> block
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].blVal; }
        break;
      case 7: // statement -> cycle
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal; }
        break;
      case 8: // statement -> if
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].ifVal; }
        break;
      case 9: // ident -> ID
{ CurrentSemanticValue.eVal = new IdNode(ValueStack[ValueStack.Depth-1].sVal); }
        break;
      case 10: // assign -> ident, ASSIGN, expr
{ CurrentSemanticValue.stVal = new AssignNode(ValueStack[ValueStack.Depth-3].eVal as IdNode, ValueStack[ValueStack.Depth-1].eVal); }
        break;
      case 11: // assign -> ident, ASSIGN, prexpr
{ CurrentSemanticValue.stVal = new AssignNode(ValueStack[ValueStack.Depth-3].eVal as IdNode, ValueStack[ValueStack.Depth-1].prVal); }
        break;
      case 12: // prexpr -> expr, GREATER, expr
{ CurrentSemanticValue.prVal = new PredicateBinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, PredicateOperationType.Greater); }
        break;
      case 13: // prexpr -> expr, LESS, expr
{ CurrentSemanticValue.prVal = new PredicateBinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, PredicateOperationType.Less); }
        break;
      case 14: // prexpr -> expr, EQUAL, expr
{ CurrentSemanticValue.prVal = new PredicateBinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, PredicateOperationType.Equal); }
        break;
      case 15: // prexpr -> expr, NOTEQUAL, expr
{ CurrentSemanticValue.prVal = new PredicateBinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, PredicateOperationType.Notequal); }
        break;
      case 16: // expr -> t
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 17: // expr -> expr, PLUS, t
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, OperationType.Plus); }
        break;
      case 18: // expr -> expr, MINUS, t
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, OperationType.Minus); }
        break;
      case 19: // t -> f
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 20: // t -> t, MULT, f
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, OperationType.Mult); }
        break;
      case 21: // t -> t, DIV, f
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, OperationType.Div); }
        break;
      case 22: // f -> ident
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal as IdNode; }
        break;
      case 23: // f -> INUM
{ CurrentSemanticValue.eVal = new IntNumNode(ValueStack[ValueStack.Depth-1].iVal); }
        break;
      case 24: // f -> RNUM
{ CurrentSemanticValue.eVal = new RealNumNode(ValueStack[ValueStack.Depth-1].dVal); }
        break;
      case 25: // f -> LEFTBRACKET, expr, RIGHTBRACKET
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-2].eVal; }
        break;
      case 26: // block -> BEGIN, stlist, END
{ CurrentSemanticValue.blVal = ValueStack[ValueStack.Depth-2].blVal; }
        break;
      case 27: // cycle -> CYCLE, expr, statement
{ CurrentSemanticValue.stVal = new CycleNode(ValueStack[ValueStack.Depth-2].eVal, ValueStack[ValueStack.Depth-1].stVal); }
        break;
      case 28: // if -> IF, prexpr, THEN, statement
{ CurrentSemanticValue.ifVal = new IfNode(ValueStack[ValueStack.Depth-3].prVal, ValueStack[ValueStack.Depth-1].stVal); }
        break;
      case 29: // if -> IF, prexpr, THEN, statement, ELSE, statement
{ CurrentSemanticValue.ifVal = new IfNode(ValueStack[ValueStack.Depth-5].prVal, ValueStack[ValueStack.Depth-3].stVal, ValueStack[ValueStack.Depth-1].stVal); }
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
