// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.3.6
// Machine:  JEDIKNIGHT-PC
// DateTime: 10.12.2015 18:53:34
// UserName: jediknight
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
    EQUAL=19,INEQUAL=20,LSHIFT=21,COUT=22,COMMA=23,COLON=24,
    NOT=25,LESSEQUAL=26,GREATEREQUAL=27,GOTO=28,INUM=29,RNUM=30,
    ID=31,STRING_L=32,TYPE=33};

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
			public string typeVal;
			public VarDeclListNode listDeclVal;
		  public List<VarDeclNode> listDeclRawVal;
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
  private static Rule[] rules = new Rule[59];
  private static State[] states = new State[108];
  private static string[] nonTerms = new string[] {
      "expr", "ident", "T", "F", "S", "U", "assign", "statement", "st", "do_while", 
      "while", "if", "goto", "stlist", "block", "cout", "funcall", "funcallst", 
      "params", "decl_type_list", "decl_list", "progr", "$accept", };

  static Parser() {
    states[0] = new State(new int[]{11,4},new int[]{-22,1,-15,3});
    states[1] = new State(new int[]{2,2});
    states[2] = new State(-1);
    states[3] = new State(-2);
    states[4] = new State(new int[]{31,24,11,4,16,62,15,70,22,79,13,83,33,95,28,105,4,-21},new int[]{-14,5,-8,107,-2,8,-9,68,-7,11,-15,59,-10,60,-11,69,-16,75,-12,82,-18,90,-17,92,-20,93,-13,103});
    states[5] = new State(new int[]{12,6,31,24,11,4,16,62,15,70,22,79,13,83,33,95,28,105,4,-21},new int[]{-8,7,-2,8,-9,68,-7,11,-15,59,-10,60,-11,69,-16,75,-12,82,-18,90,-17,92,-20,93,-13,103});
    states[6] = new State(-56);
    states[7] = new State(-4);
    states[8] = new State(new int[]{24,9,3,14});
    states[9] = new State(new int[]{31,24,11,4,16,62,15,70,22,79,13,83,33,95,28,105,4,-21},new int[]{-9,10,-7,11,-2,13,-15,59,-10,60,-11,69,-16,75,-12,82,-18,90,-17,92,-20,93,-13,103});
    states[10] = new State(-9);
    states[11] = new State(new int[]{4,12});
    states[12] = new State(-11);
    states[13] = new State(new int[]{3,14});
    states[14] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-1,15,-5,57,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[15] = new State(new int[]{17,16,18,30,19,42,20,51,26,53,27,55,4,-31,23,-31});
    states[16] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-5,17,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[17] = new State(new int[]{5,18,6,32,17,-33,18,-33,19,-33,20,-33,26,-33,27,-33,4,-33,23,-33,10,-33,21,-33});
    states[18] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-3,19,-6,45,-4,22,-2,23,-17,46});
    states[19] = new State(new int[]{7,20,8,34,5,-40,6,-40,17,-40,18,-40,19,-40,20,-40,26,-40,27,-40,4,-40,23,-40,10,-40,21,-40});
    states[20] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-6,21,-4,22,-2,23,-17,46});
    states[21] = new State(-43);
    states[22] = new State(-45);
    states[23] = new State(-48);
    states[24] = new State(new int[]{9,25,24,-30,3,-30,7,-30,8,-30,5,-30,6,-30,17,-30,18,-30,19,-30,20,-30,26,-30,27,-30,4,-30,23,-30,10,-30,21,-30});
    states[25] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-19,26,-1,58,-5,57,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[26] = new State(new int[]{10,27,23,28});
    states[27] = new State(-29);
    states[28] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-1,29,-5,57,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[29] = new State(new int[]{17,16,18,30,19,42,20,51,26,53,27,55,10,-8,23,-8});
    states[30] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-5,31,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[31] = new State(new int[]{5,18,6,32,17,-34,18,-34,19,-34,20,-34,26,-34,27,-34,4,-34,23,-34,10,-34,21,-34});
    states[32] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-3,33,-6,45,-4,22,-2,23,-17,46});
    states[33] = new State(new int[]{7,20,8,34,5,-41,6,-41,17,-41,18,-41,19,-41,20,-41,26,-41,27,-41,4,-41,23,-41,10,-41,21,-41});
    states[34] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-6,35,-4,22,-2,23,-17,46});
    states[35] = new State(-44);
    states[36] = new State(-49);
    states[37] = new State(-50);
    states[38] = new State(-51);
    states[39] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-1,40,-5,57,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[40] = new State(new int[]{10,41,17,16,18,30,19,42,20,51,26,53,27,55});
    states[41] = new State(-52);
    states[42] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-5,43,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[43] = new State(new int[]{5,18,6,32,17,-35,18,-35,19,-35,20,-35,26,-35,27,-35,4,-35,23,-35,10,-35,21,-35});
    states[44] = new State(new int[]{7,20,8,34,5,-39,6,-39,17,-39,18,-39,19,-39,20,-39,26,-39,27,-39,4,-39,23,-39,10,-39,21,-39});
    states[45] = new State(-42);
    states[46] = new State(-53);
    states[47] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-6,48,-4,22,-2,23,-17,46});
    states[48] = new State(-46);
    states[49] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-6,50,-4,22,-2,23,-17,46});
    states[50] = new State(-47);
    states[51] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-5,52,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[52] = new State(new int[]{5,18,6,32,17,-36,18,-36,19,-36,20,-36,26,-36,27,-36,4,-36,23,-36,10,-36,21,-36});
    states[53] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-5,54,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[54] = new State(new int[]{5,18,6,32,17,-37,18,-37,19,-37,20,-37,26,-37,27,-37,4,-37,23,-37,10,-37,21,-37});
    states[55] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-5,56,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[56] = new State(new int[]{5,18,6,32,17,-38,18,-38,19,-38,20,-38,26,-38,27,-38,4,-38,23,-38,10,-38,21,-38});
    states[57] = new State(new int[]{5,18,6,32,17,-32,18,-32,19,-32,20,-32,26,-32,27,-32,4,-32,23,-32,10,-32,21,-32});
    states[58] = new State(new int[]{17,16,18,30,19,42,20,51,26,53,27,55,10,-7,23,-7});
    states[59] = new State(-12);
    states[60] = new State(new int[]{4,61});
    states[61] = new State(-13);
    states[62] = new State(new int[]{31,24,11,4,16,62,15,70,22,79,13,83,33,95,28,105,4,-21},new int[]{-8,63,-2,8,-9,68,-7,11,-15,59,-10,60,-11,69,-16,75,-12,82,-18,90,-17,92,-20,93,-13,103});
    states[63] = new State(new int[]{15,64});
    states[64] = new State(new int[]{9,65});
    states[65] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-1,66,-5,57,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[66] = new State(new int[]{10,67,17,16,18,30,19,42,20,51,26,53,27,55});
    states[67] = new State(-57);
    states[68] = new State(-10);
    states[69] = new State(-14);
    states[70] = new State(new int[]{9,71});
    states[71] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-1,72,-5,57,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[72] = new State(new int[]{10,73,17,16,18,30,19,42,20,51,26,53,27,55});
    states[73] = new State(new int[]{31,24,11,4,16,62,15,70,22,79,13,83,33,95,28,105,4,-21},new int[]{-8,74,-2,8,-9,68,-7,11,-15,59,-10,60,-11,69,-16,75,-12,82,-18,90,-17,92,-20,93,-13,103});
    states[74] = new State(-58);
    states[75] = new State(new int[]{4,76,21,77});
    states[76] = new State(-15);
    states[77] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-1,78,-5,57,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[78] = new State(new int[]{17,16,18,30,19,42,20,51,26,53,27,55,4,-6,21,-6});
    states[79] = new State(new int[]{21,80});
    states[80] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-1,81,-5,57,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[81] = new State(new int[]{17,16,18,30,19,42,20,51,26,53,27,55,4,-5,21,-5});
    states[82] = new State(-16);
    states[83] = new State(new int[]{9,84});
    states[84] = new State(new int[]{31,24,29,36,30,37,32,38,9,39,6,47,25,49},new int[]{-1,85,-5,57,-3,44,-6,45,-4,22,-2,23,-17,46});
    states[85] = new State(new int[]{10,86,17,16,18,30,19,42,20,51,26,53,27,55});
    states[86] = new State(new int[]{31,24,11,4,16,62,15,70,22,79,13,83,33,95,28,105,4,-21},new int[]{-8,87,-2,8,-9,68,-7,11,-15,59,-10,60,-11,69,-16,75,-12,82,-18,90,-17,92,-20,93,-13,103});
    states[87] = new State(new int[]{14,88,12,-54,31,-54,11,-54,16,-54,15,-54,22,-54,13,-54,33,-54,28,-54,4,-54});
    states[88] = new State(new int[]{31,24,11,4,16,62,15,70,22,79,13,83,33,95,28,105,4,-21},new int[]{-8,89,-2,8,-9,68,-7,11,-15,59,-10,60,-11,69,-16,75,-12,82,-18,90,-17,92,-20,93,-13,103});
    states[89] = new State(-55);
    states[90] = new State(new int[]{4,91});
    states[91] = new State(-17);
    states[92] = new State(-28);
    states[93] = new State(new int[]{4,94});
    states[94] = new State(-18);
    states[95] = new State(new int[]{31,100,23,-23,4,-23},new int[]{-21,96,-2,101,-7,102});
    states[96] = new State(new int[]{23,97,4,-22});
    states[97] = new State(new int[]{31,100},new int[]{-2,98,-7,99});
    states[98] = new State(new int[]{3,14,23,-26,4,-26});
    states[99] = new State(-27);
    states[100] = new State(-30);
    states[101] = new State(new int[]{3,14,23,-24,4,-24});
    states[102] = new State(-25);
    states[103] = new State(new int[]{4,104});
    states[104] = new State(-19);
    states[105] = new State(new int[]{31,100},new int[]{-2,106});
    states[106] = new State(-20);
    states[107] = new State(-3);

    rules[1] = new Rule(-23, new int[]{-22,2});
    rules[2] = new Rule(-22, new int[]{-15});
    rules[3] = new Rule(-14, new int[]{-8});
    rules[4] = new Rule(-14, new int[]{-14,-8});
    rules[5] = new Rule(-16, new int[]{22,21,-1});
    rules[6] = new Rule(-16, new int[]{-16,21,-1});
    rules[7] = new Rule(-19, new int[]{-1});
    rules[8] = new Rule(-19, new int[]{-19,23,-1});
    rules[9] = new Rule(-8, new int[]{-2,24,-9});
    rules[10] = new Rule(-8, new int[]{-9});
    rules[11] = new Rule(-9, new int[]{-7,4});
    rules[12] = new Rule(-9, new int[]{-15});
    rules[13] = new Rule(-9, new int[]{-10,4});
    rules[14] = new Rule(-9, new int[]{-11});
    rules[15] = new Rule(-9, new int[]{-16,4});
    rules[16] = new Rule(-9, new int[]{-12});
    rules[17] = new Rule(-9, new int[]{-18,4});
    rules[18] = new Rule(-9, new int[]{-20,4});
    rules[19] = new Rule(-9, new int[]{-13,4});
    rules[20] = new Rule(-13, new int[]{28,-2});
    rules[21] = new Rule(-20, new int[]{});
    rules[22] = new Rule(-20, new int[]{33,-21});
    rules[23] = new Rule(-21, new int[]{});
    rules[24] = new Rule(-21, new int[]{-2});
    rules[25] = new Rule(-21, new int[]{-7});
    rules[26] = new Rule(-21, new int[]{-21,23,-2});
    rules[27] = new Rule(-21, new int[]{-21,23,-7});
    rules[28] = new Rule(-18, new int[]{-17});
    rules[29] = new Rule(-17, new int[]{31,9,-19,10});
    rules[30] = new Rule(-2, new int[]{31});
    rules[31] = new Rule(-7, new int[]{-2,3,-1});
    rules[32] = new Rule(-1, new int[]{-5});
    rules[33] = new Rule(-1, new int[]{-1,17,-5});
    rules[34] = new Rule(-1, new int[]{-1,18,-5});
    rules[35] = new Rule(-1, new int[]{-1,19,-5});
    rules[36] = new Rule(-1, new int[]{-1,20,-5});
    rules[37] = new Rule(-1, new int[]{-1,26,-5});
    rules[38] = new Rule(-1, new int[]{-1,27,-5});
    rules[39] = new Rule(-5, new int[]{-3});
    rules[40] = new Rule(-5, new int[]{-5,5,-3});
    rules[41] = new Rule(-5, new int[]{-5,6,-3});
    rules[42] = new Rule(-3, new int[]{-6});
    rules[43] = new Rule(-3, new int[]{-3,7,-6});
    rules[44] = new Rule(-3, new int[]{-3,8,-6});
    rules[45] = new Rule(-6, new int[]{-4});
    rules[46] = new Rule(-6, new int[]{6,-6});
    rules[47] = new Rule(-6, new int[]{25,-6});
    rules[48] = new Rule(-4, new int[]{-2});
    rules[49] = new Rule(-4, new int[]{29});
    rules[50] = new Rule(-4, new int[]{30});
    rules[51] = new Rule(-4, new int[]{32});
    rules[52] = new Rule(-4, new int[]{9,-1,10});
    rules[53] = new Rule(-4, new int[]{-17});
    rules[54] = new Rule(-12, new int[]{13,9,-1,10,-8});
    rules[55] = new Rule(-12, new int[]{13,9,-1,10,-8,14,-8});
    rules[56] = new Rule(-15, new int[]{11,-14,12});
    rules[57] = new Rule(-10, new int[]{16,-8,15,9,-1,10});
    rules[58] = new Rule(-11, new int[]{15,9,-1,10,-8});
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
      case 9: // statement -> ident, COLON, st
{ ValueStack[ValueStack.Depth-1].stVal.AddLabel(ValueStack[ValueStack.Depth-3].eVal as IdNode); CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal;}
        break;
      case 10: // statement -> st
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal; }
        break;
      case 11: // st -> assign, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].stVal; }
        break;
      case 12: // st -> block
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].blVal; }
        break;
      case 13: // st -> do_while, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].stVal; }
        break;
      case 14: // st -> while
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal; }
        break;
      case 15: // st -> cout, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].ioVal; }
        break;
      case 16: // st -> if
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-1].stVal; }
        break;
      case 17: // st -> funcallst, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].funStVal; }
        break;
      case 18: // st -> decl_type_list, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].listDeclVal; }
        break;
      case 19: // st -> goto, SEMICOLON
{ CurrentSemanticValue.stVal = ValueStack[ValueStack.Depth-2].stVal; }
        break;
      case 20: // goto -> GOTO, ident
{ CurrentSemanticValue.stVal = new GotoNode(ValueStack[ValueStack.Depth-1].eVal as IdNode); }
        break;
      case 22: // decl_type_list -> TYPE, decl_list
{ 
		SimpleVarType type;
		if (ValueStack[ValueStack.Depth-2].typeVal == "int")
			type = SimpleVarType.Int;
		else if (ValueStack[ValueStack.Depth-2].typeVal == "float")
			type = SimpleVarType.Float;
		else if (ValueStack[ValueStack.Depth-2].typeVal == "string")
			type = SimpleVarType.Str;
		else if (ValueStack[ValueStack.Depth-2].typeVal == "bool")
			type = SimpleVarType.Bool;
		else type = SimpleVarType.Unknown;
		CurrentSemanticValue.listDeclVal = new VarDeclListNode(type); 
		CurrentSemanticValue.listDeclVal.VariablesList = ValueStack[ValueStack.Depth-1].listDeclRawVal; 
	}
        break;
      case 24: // decl_list -> ident
{ CurrentSemanticValue.listDeclRawVal = new List<VarDeclNode>(); CurrentSemanticValue.listDeclRawVal.Add(new VarDeclNode(ValueStack[ValueStack.Depth-1].eVal as IdNode)); }
        break;
      case 25: // decl_list -> assign
{ CurrentSemanticValue.listDeclRawVal = new List<VarDeclNode>(); CurrentSemanticValue.listDeclRawVal.Add(new VarDeclNode(ValueStack[ValueStack.Depth-1].stVal as AssignNode)); }
        break;
      case 26: // decl_list -> decl_list, COMMA, ident
{ ValueStack[ValueStack.Depth-3].listDeclRawVal.Add(new VarDeclNode(ValueStack[ValueStack.Depth-1].eVal as IdNode)); CurrentSemanticValue.listDeclRawVal = ValueStack[ValueStack.Depth-3].listDeclRawVal; }
        break;
      case 27: // decl_list -> decl_list, COMMA, assign
{ ValueStack[ValueStack.Depth-3].listDeclRawVal.Add(new VarDeclNode(ValueStack[ValueStack.Depth-1].stVal as AssignNode)); CurrentSemanticValue.listDeclRawVal = ValueStack[ValueStack.Depth-3].listDeclRawVal; }
        break;
      case 28: // funcallst -> funcall
{ CurrentSemanticValue.funStVal = new FunctionNodeSt(); CurrentSemanticValue.funStVal.Function = ValueStack[ValueStack.Depth-1].funVal; }
        break;
      case 29: // funcall -> ID, LBRACKET, params, RBRACKET
{
		CurrentSemanticValue.funVal = new FunctionNode(ValueStack[ValueStack.Depth-4].sVal);
		CurrentSemanticValue.funVal.Parameters = ValueStack[ValueStack.Depth-2].paramVal;
	}
        break;
      case 30: // ident -> ID
{ CurrentSemanticValue.eVal = new IdNode(ValueStack[ValueStack.Depth-1].sVal); }
        break;
      case 31: // assign -> ident, ASSIGN, expr
{ CurrentSemanticValue.stVal = new AssignNode(ValueStack[ValueStack.Depth-3].eVal as IdNode, ValueStack[ValueStack.Depth-1].eVal); }
        break;
      case 32: // expr -> S
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 33: // expr -> expr, LESS, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.Less); }
        break;
      case 34: // expr -> expr, GREAT, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.Greater); }
        break;
      case 35: // expr -> expr, EQUAL, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.Equal); }
        break;
      case 36: // expr -> expr, INEQUAL, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.NotEqual); }
        break;
      case 37: // expr -> expr, LESSEQUAL, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.LessEqual); }
        break;
      case 38: // expr -> expr, GREATEREQUAL, S
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.GreaterEqual); }
        break;
      case 39: // S -> T
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 40: // S -> S, PLUS, T
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.Plus); }
        break;
      case 41: // S -> S, MINUS, T
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.Minus); }
        break;
      case 42: // T -> U
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 43: // T -> T, MUL, U
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.Mult); }
        break;
      case 44: // T -> T, DIV, U
{ CurrentSemanticValue.eVal = new BinaryNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].eVal, Operator.Div); }
        break;
      case 45: // U -> F
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal; }
        break;
      case 46: // U -> MINUS, U
{ CurrentSemanticValue.eVal = new UnaryNode(ValueStack[ValueStack.Depth-1].eVal, Operator.Minus); }
        break;
      case 47: // U -> NOT, U
{ CurrentSemanticValue.eVal = new UnaryNode(ValueStack[ValueStack.Depth-1].eVal, Operator.Not); }
        break;
      case 48: // F -> ident
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal as IdNode; }
        break;
      case 49: // F -> INUM
{ CurrentSemanticValue.eVal = new IntNumNode(ValueStack[ValueStack.Depth-1].iVal); }
        break;
      case 50: // F -> RNUM
{ CurrentSemanticValue.eVal = new FloatNumNode(ValueStack[ValueStack.Depth-1].dVal); }
        break;
      case 51: // F -> STRING_L
{ CurrentSemanticValue.eVal = new StringLiteralNode(ValueStack[ValueStack.Depth-1].sVal); }
        break;
      case 52: // F -> LBRACKET, expr, RBRACKET
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-2].eVal; }
        break;
      case 53: // F -> funcall
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].funVal; }
        break;
      case 54: // if -> IF, LBRACKET, expr, RBRACKET, statement
{ CurrentSemanticValue.stVal = new IfNode(ValueStack[ValueStack.Depth-3].eVal, ValueStack[ValueStack.Depth-1].stVal); }
        break;
      case 55: // if -> IF, LBRACKET, expr, RBRACKET, statement, ELSE, statement
{ CurrentSemanticValue.stVal = new IfNode(ValueStack[ValueStack.Depth-5].eVal, ValueStack[ValueStack.Depth-3].stVal, ValueStack[ValueStack.Depth-1].stVal); }
        break;
      case 56: // block -> BEGIN, stlist, END
{ CurrentSemanticValue.blVal = ValueStack[ValueStack.Depth-2].blVal; }
        break;
      case 57: // do_while -> DO, statement, WHILE, LBRACKET, expr, RBRACKET
{ CurrentSemanticValue.stVal = new DoWhileNode(ValueStack[ValueStack.Depth-2].eVal, ValueStack[ValueStack.Depth-5].stVal); }
        break;
      case 58: // while -> WHILE, LBRACKET, expr, RBRACKET, statement
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
