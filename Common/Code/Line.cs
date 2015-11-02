using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Line
{
    class Line
    {
        public string label = "";

        public virtual bool IsEmpty() {
            return true;
        }

        public virtual bool HasLabel() {
            return label != "";
        }

        // Меняет цель (для GoTo и If) на `forWhat`, если она равна `check`
        public virtual void ChangeTargetIfEqual(string check, string forWhat)
        {

        }
    }

    class NonEmptyLine : Line
    {
        public override bool IsEmpty()
        {
            return false;
        }
    }

    class EmptyLine : Line
    {

    }

    class GoTo : NonEmptyLine
    {
        public string target;

         public GoTo(string target) {
             this.target = target;
         }

         public override void ChangeTargetIfEqual(string check, string forWhat)
         {
             if (target == check) target = forWhat;
         }
    }


    class СonditionalJump : GoTo
    {
        public string condition;

        public СonditionalJump(string condition, string target):
            base(target)
        {
            this.condition = condition;
        }
    }

    class FunctionParam : NonEmptyLine
    {
        public string param;

        public FunctionParam(string param)
        {
            this.param = param;
        }
    }

    class FunctionCall : NonEmptyLine
    {
        public string name;
        public int parameters; //число параметров
        public string destination; //возвращаемый параметр (если он есть)

        public FunctionCall(string name, int nparams, string dest = null)
        {
            this.name = name;
            this.parameters = nparams;
            this.destination = dest;
        }

        public virtual bool IsVoid()
        {
            return destination == null || destination.Count() == 0;
        }
    }

    class Operation : NonEmptyLine
    {
        private static ISet<BinaryOperation> mBoolOps= new HashSet<BinaryOperation> {
            BinaryOperation.Equal,
            BinaryOperation.Less,
            BinaryOperation.LessEqual,
            BinaryOperation.Greater,
            BinaryOperation.GreaterEqual,
            BinaryOperation.NotEqual,
        };

        private static ISet<BinaryOperation> mArithmOps = new HashSet<BinaryOperation> {
            BinaryOperation.Plus,
            BinaryOperation.Minus,
            BinaryOperation.Div,
            BinaryOperation.Mult
        };

        public string left;
        public string first;
        public string second;
        public BinaryOperation operation;

        // Бинарная операция
        public Operation(string left, string first, BinaryOperation op, string second)
        {
            this.left = left;
            this.first = first;
            this.operation = op;
            this.second = second;
        }

        // Тождество
        public Operation(string left, string first)
        {
            this.left = left;
            this.first = first;

            this.second = "";
            this.operation = BinaryOperation.None;
        }

        public virtual bool IsUnary() //на будущее
        {
            return false;
        }

        public virtual bool IsIdentity()
        {
            return second == "" && operation == BinaryOperation.None;
        }

        public virtual bool IsBoolExpr()
        {
            return mBoolOps.Contains(operation);
        }

        public virtual bool IsArithmExpr()
        {
            return mArithmOps.Contains(operation);
        }

        public virtual bool FirstParamIsNumber()
        {
            double temp;
            return double.TryParse(first, out temp);
        }

        public virtual bool SecondParamIsNumber()
        {
            double temp;
            return double.TryParse(second, out temp);
        }

        public void ToIdentity(string value)
        {
            first = value;
            second = "";
            operation = BinaryOperation.None;
        }
    }
}
