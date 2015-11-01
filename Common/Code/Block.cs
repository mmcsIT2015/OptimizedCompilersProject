using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SimpleLang
{
    class Block : List<Line.Line>
    {
        private List<HashSet<string>> mDefUseData;

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var line in this)
            {
                if (line.HasLabel()) builder.Append(line.label + ":");
                builder.Append('\t', 1);

                if (line is Line.СonditionalJump)
                {
                    var jump = line as Line.СonditionalJump;
                    builder.Append("if " + jump.condition + " goto " + jump.target + "\n");
                }
                else if (line is Line.GoTo)
                {
                    builder.Append("goto " + (line as Line.GoTo).target + "\n");
                }
                else if (line is Line.FunctionParam)
                {
                    builder.Append("param " + (line as Line.FunctionParam).param + "\n");
                }
                else if (line is Line.FunctionCall)
                {
                    var call = line as Line.FunctionCall;
                    if (call.IsVoid()) builder.Append("call " + call.name + ", " + call.parameters + "\n");
                    else builder.Append(call.destination + " = call " + call.name + ", " + call.parameters + "\n");
                }
                else
                {
                    if (line.IsEmpty())
                    {
                        builder.Append("<empty statement>\n");
                    }
                    else
                    {
                        var binary = line as Line.Operation;

                        builder.Append(binary.left + " = " + binary.first + " ");
                        builder.Append((binary.IsIdentity() ? "" : binary.operation.ToString() + " ") + binary.second + "\n");
                    }
                }
            }

            builder.Replace("  ", " ");

            return builder.ToString();
        }

        /// <summary>
        /// Должна быть вызвана перед любыми вызовами IsVariableAlive, GetAliveVariables.
        /// Если блок был изменен, нужно вызвать еще раз, в противном случае возвращаемые данные бессмысленны.
        /// </summary>
        public void CalculateDefUseData()
        {
            mDefUseData = new List<HashSet<string>>();
            HashSet<string> currentlyAlive = new HashSet<string>();

            for (int i = this.Count - 1; i >= 0; --i)
            {
                if (this[i] is Line.FunctionCall) continue;
                else if (this[i] is Line.FunctionParam)
                {
                    var line = this[i] as Line.FunctionParam;
                    if (line.param == "")
                    {
                        throw new ArgumentException("Critical error! Empty `param` operand for `FunctionParam` command in " + i + " line.");
                    }

                    double temp;
                    if (!double.TryParse(line.param, out temp))
                    {
                        if (!currentlyAlive.Contains(line.param))
                        {
                            currentlyAlive.Add(line.param);
                        }
                    }
                }
                else if (this[i] is Line.СonditionalJump)
                {
                    var line = this[i] as Line.СonditionalJump;
                    if (line.condition == "")
                    {
                        throw new ArgumentException("Critical error! Empty `condition` operand for `СonditionalJump` command in " + i + " line.");
                    }

                    double temp;
                    if (!double.TryParse(line.condition, out temp))
                    {
                        if (!currentlyAlive.Contains(line.condition))
                        {
                            currentlyAlive.Add(line.condition);
                        }
                    }
                }
                else if (this[i] is Line.GoTo) continue;
                else
                {
                    var line = this[i] as Line.Operation;

                    Debug.Assert(line.left != "");
                    if (currentlyAlive.Contains(line.left))
                    {
                        currentlyAlive.Remove(line.left);
                    }

                    Debug.Assert(line.first != "");
                    if (!line.FirstParamIsNumber() && !currentlyAlive.Contains(line.first))
                    {
                        currentlyAlive.Add(line.first);
                    }

                    Debug.Assert(line.second != "");
                    if (!line.SecondParamIsNumber() && !currentlyAlive.Contains(line.second))
                    {
                        currentlyAlive.Add(line.second);
                    }
                }

                mDefUseData.Add(new HashSet<string>(currentlyAlive.Clone()));
            }

            mDefUseData.Reverse();
        }

        /// <summary>
        /// Проверяет, жива ли переменная на некотором шаге
        /// </summary>
        /// <param name="variable">Имя переменной</param>
        /// <param name="step">Номер шага</param>
        /// <returns>Истина, если жива</returns>
        public bool IsVariableAlive(string variable, int step)
        {
            return mDefUseData[step].Contains(variable);
        }

        /// <summary>
        /// Возвращает множество живых переменных для заданного шага
        /// </summary>
        /// <param name="step">Номер шага</param>
        /// <returns>HashSet, содержащий все живые переменные в строковом виде</string></returns>
        public HashSet<string> GetAliveVariables(int step)
        {
            return mDefUseData[step];
        }
    };

    static class Extensions
    {
        /// <summary>
        /// Deep copy for IEnumerable containers, which is not implemented in the standard library by default
        /// </summary>
        /// <typeparam name="T">Type of data within container</typeparam>
        /// <param name="containerToClone">Container to copy</param>
        /// <returns>New container, fully copied</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> containerToClone) where T : ICloneable
        {
            return containerToClone.Select(item => (T)item.Clone());
        }

    }
}
