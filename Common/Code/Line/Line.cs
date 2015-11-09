﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Line
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
}
