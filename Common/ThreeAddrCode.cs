using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    using Label = KeyValuePair<int, int>; // хранит номер блока и номер строки в этом блоке

    class Block: List<ThreeAddrCode.Line> {

        private List<HashSet<string>> defUseData = new List<HashSet<string>>();                

        public override string ToString()
        {
            const int indent = 1; // количество отступов
            var builder = new StringBuilder();            

            foreach (var line in this)
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
                    builder.Append("call " + line.left + ", " + line.second + "\n");
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

            return builder.ToString();
        }
        public void CalculateDefUseData()
        {
            defUseData.Clear();
            HashSet<string> currentlyAlive = new HashSet<string>();

            for (int i = this.Count - 1; i >= 0; --i)
            {
                
                switch(this[i].command)
                {
                    case "goto":                        
                            break;
                    case "call":
                            break;
                    case "param":
                        {
                            if (this[i].left == "")
                                throw new ArgumentException("Critical error! Empty LEFT operand for PARAM command in " + i + " line.");
                            if (!Char.IsDigit(this[i].left[0]))
                            {
                                if (!currentlyAlive.Contains(this[i].left))
                                    currentlyAlive.Add(this[i].left);
                            }
                            break;
                        }
                                            
                    case "if":
                        {
                            if (this[i].left == "")
                                throw new ArgumentException("Critical error! Empty LEFT operand for IF command in " + i + " line.");
                            if (!Char.IsDigit(this[i].left[0]))
                            {
                                if (!currentlyAlive.Contains(this[i].left))
                                    currentlyAlive.Add(this[i].left);
                            }
                            break;
                        }
                    default:
                        {
                            if (this[i].left != "")
                            {
                                if (currentlyAlive.Contains(this[i].left))
                                    currentlyAlive.Remove(this[i].left);
                            }

                            if (this[i].first != "")
                            {
                                if (!Char.IsDigit(this[i].first[0]))
                                {
                                    if (!currentlyAlive.Contains(this[i].first))
                                        currentlyAlive.Add(this[i].first);
                                }
                            }

                            if (this[i].second != "")
                            {
                                if (!Char.IsDigit(this[i].second[0]))
                                {
                                    if (!currentlyAlive.Contains(this[i].second))
                                        currentlyAlive.Add(this[i].second);
                                }
                            }
                            break;
                        }                
                }                                                

                defUseData.Add(new HashSet<string>(currentlyAlive.Clone()));
            }

            defUseData.Reverse();
        }

        public bool IsVariableAlive(string variable, int step)
        {
            return defUseData[step].Contains(variable);
        }

        public HashSet<string> GetAliveVariables(int step)
        {
            return defUseData[step];
        }
    };

    static class Extensions
    {
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> containerToClone) where T : ICloneable
        {
            return containerToClone.Select(item => (T)item.Clone());
        }

    }

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

            public override string ToString()
            {
                return String.Format("{0,10}|{1,10}|{2,10}|{3,10}|{4,10}", this.label, this.left, this.first, this.command, this.second);
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
            var builder = new StringBuilder();
            foreach (var block in blocks)
            {
                builder.Append(block.ToString());
                builder.Append("\n\n");
            }

            return builder.ToString();
        }
    }
}
