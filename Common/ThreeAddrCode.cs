using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    using Block = List<ThreeAddrCode.Line>;
    using Label = KeyValuePair<int, int>; // хранит номер блока и номер строки в этом блоке

    class ThreeAddrCode
    {
        public class Line
        {
            public string label;
            public string left; // операнд из левой части выражения (присваивания)
            public string command; // команда
            public string first; // первый операнд из  правой части выражения
            public string second; // второй операнд из  правой части выражения

            private static int LineUID = 0;
            public static string GetNextLabel()
            {
                return "L" + (LineUID).ToString();
            }

            // Конструктор для строк вида x =  y `op` z
            public Line(string dst, string lhs, string cmd, string rhs) 
            {
                label = "L" + (LineUID++).ToString();
                left = dst;
                first = lhs;
                command = cmd;
                second = rhs;
            }

            // Конструктор для строк вида x = `op` y, т.е. левый операнд - отсутствует
            public Line(string dst, string cmd, string rhs)
            {
                label = "L" + (LineUID++).ToString();
                left = dst;
                first = "";
                command = cmd;
                second = rhs;
            }
        }

        public Dictionary<string, Label> labels; // содержит список меток и адресом этих меток в blocks
        public List<Block> blocks; // содержит массив с блоками

        public ThreeAddrCode()
        {
            blocks = new List<Block>() {new Block() };
            labels = new Dictionary<string, Label>();
        }

        private static int tempaVarUID = 0;
        public static string GetTempVariable()
        {
            // TODO variables must have such names
            return "t" + (tempaVarUID++).ToString();
        }

        public void NewBlock()
        {
            // смысла в пустых блоках нет - поэтому, если мы пытаемся добавить еще один очередной пустой, ничего не делаем
            if (blocks.Last().Count > 0)
            {
                blocks.Add(new Block());
            }
        }

        public Line AddLine(Line line)
        {
            blocks.Last().Add(line);
            return line;
        }

        public Label GetLastLabel()
        {
            return new Label(blocks.Count() - 1, blocks.Last().Count() - 1);
        }

        public Line GetLine(Label label)
        {
            return blocks[label.Key][label.Value];
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var block in blocks)
            {
                foreach (var line in block)
                {
                    if (line.label.Length > 0) builder.Append(line.label + ": ");

                    if (line.command != "if" && line.command != "goto")
                    {
                        builder.Append(line.left + " = ");
                        builder.Append(line.first + " " + line.command + " " + line.second + "\n");
                    }
                    else if (line.command == "if")
                    {
                        builder.Append("if " + line.left + " goto " + line.first + "\n");
                    } else if (line.command == "goto")
                    {
                        builder.Append("goto " + line.left + "\n");
                    }
                }

                builder.Append("\n");
            }

            return builder.ToString();
        }
    }
}
