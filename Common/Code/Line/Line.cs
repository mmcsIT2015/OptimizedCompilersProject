﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using System.Diagnostics;

namespace Compiler.Line
{
    public class Line
    {
        public string label = "";

        public virtual bool IsEmpty() // является ли строка пустым оператором
        {
            return true;
        }

        public virtual bool HasLabel() // есть ли у строки метка
        {  
            return label != "";
        }

        // Меняет цель (для GoTo и If) на `forWhat`, если она равна `check`
        public virtual void ChangeTargetIfEqual(string check, string forWhat)
        {

        }

        public bool Is<T>() where T : Line
        {
            return (this is T);
        }

        public bool IsNot<T>() where T : Line
        {
            return !(this is T);
        }

        public override string ToString()
        {
            Debug.Assert(false, "[Abstract line to string conversion]");
            return "";
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
        public override string ToString()
        {
            return "<empty statement>\n";
        }
    }
}
