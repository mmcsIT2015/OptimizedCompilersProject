using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    using Block = List<ThreeAddrCode.Line>;
    using Label = KeyValuePair<int, int>; //хранит номер блока и номер строки в этом блоке

    class ThreeAddrCode
    {
        public struct Line
        {
            string label;
            string lhs; //операнд из левой части выражения (присваивания)
            string command; //команда

            string first; //первый операнд из  правой части выражения
            string second; //второй операнд из  правой части выражения
        }

        Dictionary<string, Label> labels; // содержит список меток и адресом этих методк в blocks
        List<Block> blocks; //содержит массив с блоками
    }
}
