using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace iCompiler
{
    public class Block : List<Line.Line>
    {
        private List<HashSet<string>> mDefUseData;

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var line in this)
            {
                if (line.HasLabel()) builder.Append(line.label + ":");
                builder.Append('\t', 1);
                builder.Append(line.ToString() + "\n");
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
                if (this[i] is Line.FunctionParam)
                {
                    var line = this[i] as Line.FunctionParam;
                    if (line.param == "")
                    {
                        throw new ArgumentException("Critical error! Empty `param` operand for `FunctionParam` command in " + i + " line.");
                    }

                    double temp;
                    if ((!double.TryParse(line.param, out temp)) && (line.param != "endl"))
                    {
                        if (!currentlyAlive.Contains(line.param))
                        {
                            currentlyAlive.Add(line.param);
                        }
                    }
                }
                else if (this[i] is Line.ConditionalJump)
                {
                    var line = this[i] as Line.ConditionalJump;
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
                else if (this[i] is Line.BinaryExpr)
                {
                    var line = this[i] as Line.BinaryExpr;

                    if (line.left != "" && currentlyAlive.Contains(line.left))
                    {
                        currentlyAlive.Remove(line.left);
                    }

                    if (line.first != "" && !line.FirstParamIsNumber() && !currentlyAlive.Contains(line.first))
                    {
                        currentlyAlive.Add(line.first);
                    }

                    if (line.second != "" && !line.SecondParamIsNumber() && !currentlyAlive.Contains(line.second))
                    {
                        currentlyAlive.Add(line.second);
                    }
                }
                else if (this[i] is Line.Identity)
                {
                    var line = this[i] as Line.Identity;
                    if (currentlyAlive.Contains(line.left))
                        currentlyAlive.Remove(line.left);

                    if (!line.RightIsNumber())
                        currentlyAlive.Add(line.right);
                }
                else if (this[i] is Line.UnaryExpr)
                {
                    var line = this[i] as Line.UnaryExpr;
                    if (currentlyAlive.Contains(line.left))
                        currentlyAlive.Remove(line.left);

                    if (!line.ArgIsNumber())
                        currentlyAlive.Add(line.argument);
                        
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

        public void ReplaceLines(Line.Line what, Line.Line who, bool replaceLabels = false)
        {
            for (int i = 0; i < Count; ++i)
            {
                if (this[i] == what)
                {
                    this[i] = who;
                    if (replaceLabels && what.label.Length > 0)
                    {
                        who.label = what.label;
                    }
                    return;
                }
            }
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
