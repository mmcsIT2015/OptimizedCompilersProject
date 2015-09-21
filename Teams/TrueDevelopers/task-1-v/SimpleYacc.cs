// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.3.6
// Machine:  PVLADPC
// DateTime: 21.09.2015 23:19:31
// UserName: VladislavPyslaru
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
    error=1,EOF=2,LBR=3,RBR=4,ASG=5,SM=6,
    DO=7,WHL=8,IF=9,ELS=10,PL=11,MN=12,
    ML=13,DV=14,LT=15,GT=16,EQ=17,LTE=18,
    GTE=19,LPR=20,RPR=21,INUM=22,RNUM=23,ID=24};

public struct ValueType
{ 
			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
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
    public BlockNode root; // Корневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
  // End verbatim content from SimpleYacc.y

#pragma warning disable 649
  private static Dictionary<int, string> aliasses;
#pragma warning restore 649
  private static Rule[] rules = new Rule[36];
  private static State[] states = new State[67];
  private static string[] nonTerms = new string[] {
      "ident", "expr", "Q", "S", "E", "T", "F", "statement", "if", "ifelse", 
      "while", "dowhile", "stlist", "block", "progr", "$accept", };

  static Parser() {
    states[0] = new State(new int[]{3,4},new int[]{-15,1,-14,3});
    states[1] = new State(new int[]{2,2});
    states[2] = new State(-1);
    states[3] = new State(-2);
    states[4] = new State(new int[]{24,19,22,20,20,21,3,4,9,46,8,53,7,60},new int[]{-13,5,-8,66,-2,8,-3,10,-4,11,-5,38,-6,31,-7,30,-1,39,-14,42,-9,43,-10,51,-11,52,-12,58});
    states[5] = new State(new int[]{4,6,24,19,22,20,20,21,3,4,9,46,8,53,7,60},new int[]{-8,7,-2,8,-3,10,-4,11,-5,38,-6,31,-7,30,-1,39,-14,42,-9,43,-10,51,-11,52,-12,58});
    states[6] = new State(-11);
    states[7] = new State(-4);
    states[8] = new State(new int[]{6,9});
    states[9] = new State(-5);
    states[10] = new State(-18);
    states[11] = new State(new int[]{17,12,16,24,15,32,19,34,18,36,6,-19,21,-19});
    states[12] = new State(new int[]{24,19,22,20,20,21},new int[]{-5,13,-6,31,-7,30,-1,18});
    states[13] = new State(new int[]{11,14,12,26,17,-22,16,-22,15,-22,19,-22,18,-22,6,-22,21,-22});
    states[14] = new State(new int[]{24,19,22,20,20,21},new int[]{-6,15,-7,30,-1,18});
    states[15] = new State(new int[]{13,16,14,28,11,-28,12,-28,17,-28,16,-28,15,-28,19,-28,18,-28,6,-28,21,-28});
    states[16] = new State(new int[]{24,19,22,20,20,21},new int[]{-7,17,-1,18});
    states[17] = new State(-31);
    states[18] = new State(-33);
    states[19] = new State(-17);
    states[20] = new State(-34);
    states[21] = new State(new int[]{24,19,22,20,20,21},new int[]{-4,22,-5,38,-6,31,-7,30,-1,18});
    states[22] = new State(new int[]{21,23,17,12,16,24,15,32,19,34,18,36});
    states[23] = new State(-35);
    states[24] = new State(new int[]{24,19,22,20,20,21},new int[]{-5,25,-6,31,-7,30,-1,18});
    states[25] = new State(new int[]{11,14,12,26,17,-23,16,-23,15,-23,19,-23,18,-23,6,-23,21,-23});
    states[26] = new State(new int[]{24,19,22,20,20,21},new int[]{-6,27,-7,30,-1,18});
    states[27] = new State(new int[]{13,16,14,28,11,-29,12,-29,17,-29,16,-29,15,-29,19,-29,18,-29,6,-29,21,-29});
    states[28] = new State(new int[]{24,19,22,20,20,21},new int[]{-7,29,-1,18});
    states[29] = new State(-32);
    states[30] = new State(-30);
    states[31] = new State(new int[]{13,16,14,28,11,-27,12,-27,17,-27,16,-27,15,-27,19,-27,18,-27,6,-27,21,-27});
    states[32] = new State(new int[]{24,19,22,20,20,21},new int[]{-5,33,-6,31,-7,30,-1,18});
    states[33] = new State(new int[]{11,14,12,26,17,-24,16,-24,15,-24,19,-24,18,-24,6,-24,21,-24});
    states[34] = new State(new int[]{24,19,22,20,20,21},new int[]{-5,35,-6,31,-7,30,-1,18});
    states[35] = new State(new int[]{11,14,12,26,17,-25,16,-25,15,-25,19,-25,18,-25,6,-25,21,-25});
    states[36] = new State(new int[]{24,19,22,20,20,21},new int[]{-5,37,-6,31,-7,30,-1,18});
    states[37] = new State(new int[]{11,14,12,26,17,-26,16,-26,15,-26,19,-26,18,-26,6,-26,21,-26});
    states[38] = new State(new int[]{11,14,12,26,17,-21,16,-21,15,-21,19,-21,18,-21,6,-21,21,-21});
    states[39] = new State(new int[]{5,40,13,-33,14,-33,11,-33,12,-33,17,-33,16,-33,15,-33,19,-33,18,-33,6,-33,21,-33});
    states[40] = new State(new int[]{24,19,22,20,20,21},new int[]{-4,41,-5,38,-6,31,-7,30,-1,18});
    states[41] = new State(new int[]{17,12,16,24,15,32,19,34,18,36,6,-20,21,-20});
    states[42] = new State(-6);
    states[43] = new State(new int[]{10,44,4,-7,24,-7,22,-7,20,-7,3,-7,9,-7,8,-7,7,-7});
    states[44] = new State(new int[]{24,19,22,20,20,21,3,4,9,46,8,53,7,60},new int[]{-8,45,-2,8,-3,10,-4,11,-5,38,-6,31,-7,30,-1,39,-14,42,-9,43,-10,51,-11,52,-12,58});
    states[45] = new State(-14);
    states[46] = new State(new int[]{20,47});
    states[47] = new State(new int[]{24,19,22,20,20,21},new int[]{-2,48,-3,10,-4,11,-5,38,-6,31,-7,30,-1,39});
    states[48] = new State(new int[]{21,49});
    states[49] = new State(new int[]{24,19,22,20,20,21,3,4,9,46,8,53,7,60},new int[]{-8,50,-2,8,-3,10,-4,11,-5,38,-6,31,-7,30,-1,39,-14,42,-9,43,-10,51,-11,52,-12,58});
    states[50] = new State(-12);
    states[51] = new State(-8);
    states[52] = new State(-9);
    states[53] = new State(new int[]{20,54});
    states[54] = new State(new int[]{24,19,22,20,20,21},new int[]{-2,55,-3,10,-4,11,-5,38,-6,31,-7,30,-1,39});
    states[55] = new State(new int[]{21,56});
    states[56] = new State(new int[]{24,19,22,20,20,21,3,4,9,46,8,53,7,60},new int[]{-8,57,-2,8,-3,10,-4,11,-5,38,-6,31,-7,30,-1,39,-14,42,-9,43,-10,51,-11,52,-12,58});
    states[57] = new State(-15);
    states[58] = new State(new int[]{6,59});
    states[59] = new State(-10);
    states[60] = new State(new int[]{24,19,22,20,20,21,3,4,9,46,8,53,7,60},new int[]{-8,61,-2,8,-3,10,-4,11,-5,38,-6,31,-7,30,-1,39,-14,42,-9,43,-10,51,-11,52,-12,58});
    states[61] = new State(new int[]{8,62});
    states[62] = new State(new int[]{20,63});
    states[63] = new State(new int[]{24,19,22,20,20,21},new int[]{-2,64,-3,10,-4,11,-5,38,-6,31,-7,30,-1,39});
    states[64] = new State(new int[]{21,65});
    states[65] = new State(-16);
    states[66] = new State(-3);

    rules[1] = new Rule(-16, new int[]{-15,2});
    rules[2] = new Rule(-15, new int[]{-14});
    rules[3] = new Rule(-13, new int[]{-8});
    rules[4] = new Rule(-13, new int[]{-13,-8});
    rules[5] = new Rule(-8, new int[]{-2,6});
    rules[6] = new Rule(-8, new int[]{-14});
    rules[7] = new Rule(-8, new int[]{-9});
    rules[8] = new Rule(-8, new int[]{-10});
    rules[9] = new Rule(-8, new int[]{-11});
    rules[10] = new Rule(-8, new int[]{-12,6});
    rules[11] = new Rule(-14, new int[]{3,-13,4});
    rules[12] = new Rule(-9, new int[]{9,20,-2,21,-8});
    rules[13] = new Rule(-10, new int[]{-9});
    rules[14] = new Rule(-10, new int[]{-9,10,-8});
    rules[15] = new Rule(-11, new int[]{8,20,-2,21,-8});
    rules[16] = new Rule(-12, new int[]{7,-8,8,20,-2,21});
    rules[17] = new Rule(-1, new int[]{24});
    rules[18] = new Rule(-2, new int[]{-3});
    rules[19] = new Rule(-3, new int[]{-4});
    rules[20] = new Rule(-3, new int[]{-1,5,-4});
    rules[21] = new Rule(-4, new int[]{-5});
    rules[22] = new Rule(-4, new int[]{-4,17,-5});
    rules[23] = new Rule(-4, new int[]{-4,16,-5});
    rules[24] = new Rule(-4, new int[]{-4,15,-5});
    rules[25] = new Rule(-4, new int[]{-4,19,-5});
    rules[26] = new Rule(-4, new int[]{-4,18,-5});
    rules[27] = new Rule(-5, new int[]{-6});
    rules[28] = new Rule(-5, new int[]{-5,11,-6});
    rules[29] = new Rule(-5, new int[]{-5,12,-6});
    rules[30] = new Rule(-6, new int[]{-7});
    rules[31] = new Rule(-6, new int[]{-6,13,-7});
    rules[32] = new Rule(-6, new int[]{-6,14,-7});
    rules[33] = new Rule(-7, new int[]{-1});
    rules[34] = new Rule(-7, new int[]{22});
    rules[35] = new Rule(-7, new int[]{20,-4,21});
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
      case 4: // stlist -> stlist, statement
{ 
                ValueStack[ValueStack.Depth-2].blVal.Add(ValueStack[ValueStack.Depth-1].stVal); 
                CurrentSemanticValue.blVal = ValueStack[ValueStack.Depth-2].blVal; 
              }
        break;
      case 17: // ident -> ID
{ CurrentSemanticValue.eVal = new IdNode(ValueStack[ValueStack.Depth-1].sVal); }
        break;
      case 33: // F -> ident
{ CurrentSemanticValue.eVal = ValueStack[ValueStack.Depth-1].eVal as IdNode; }
        break;
      case 34: // F -> INUM
{ CurrentSemanticValue.eVal = new IntNumNode(ValueStack[ValueStack.Depth-1].iVal); }
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
