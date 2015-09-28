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

            // Конструктор для строк вида x =  y `op` z
            public Line(string dst, string lhs, string cmd, string rhs) 
            {
                label = "";
                left = dst;
                first = lhs;
                command = cmd;
                second = rhs;
            }

            // Конструктор для строк вида x = `op` y, т.е. левый операнд - отсутствует
            public Line(string dst, string cmd, string rhs)
            {
                label = "";
                left = dst;
                first = "";
                command = cmd;
                second = rhs;
            }

            public bool IsEmpty()
            {
                return left == "" && first == "" && command == "" && second == "";
            }

            public static Line CreateEmpty()
            {
                return new Line("", "", "");
            }
        }

        public Dictionary<string, Label> labels; // содержит список меток и адресом этих меток в blocks
        public List<Block> blocks; // содержит массив с блоками

        public ThreeAddrCode()
        {
            blocks = new List<Block>() { new Block() };
            labels = new Dictionary<string, Label>();
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

        public Label GetLastPosition()
        {
            return new Label(blocks.Count() - 1, blocks.Last().Count() - 1);
        }

        public Line GetLine(Label label)
        {
            return blocks[label.Key][label.Value];
        }

        public Line GetLine(int block, int line)
        {
            return blocks[block][line];
        }

        public override string ToString()
        {
            const int indent = 1; // количество отступов
            var builder = new StringBuilder();
            foreach (var block in blocks)
            {
                foreach (var line in block)
                {
                    if (line.label.Length > 0) builder.Append(line.label + ":");
                    builder.Append('\t', indent);

                    if (line.command == "if")
                    {
                        builder.Append("if " + line.left + " goto " + line.first + "\n");
                    }
                    else if (line.command == "goto")
                    {
                        builder.Append("goto " + line.left + "\n");
                    }
                    else if (line.command == "param")
                    {
                        builder.Append("param " + line.left + "\n");
                    }
                    else if (line.command == "call")
                    {
                        builder.Append("call " + line.left + ", " + line .second + "\n");
                    }
                    else
                    {
                        if (line.IsEmpty())
                        {
                            builder.Append("<empty statement>\n");
                        }
                        else
                        {
                            builder.Append(line.left + " = " + line.first + " ");
                            builder.Append((line.command == "" ? "" : line.command + " ") + line.second + "\n");
                        }
                    }
                }

                builder.Replace("  ", " ");
                builder.Append("\n");
            }

            return builder.ToString();
        }
    }
}
