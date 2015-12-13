using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iCompiler
{
    // TODO
    // Нужен пример кода, который этой оптимизацией оптимизируется.

    /// <summary>
    /// Класс удаляет "мертвый" код из трехадресного кода в пределах одного базового блока
    /// Пример использования:
    /// ====
    ///     Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
    ///     codeGenerator.Visit(parser.root);
    ///     var code = codeGenerator.CreateCode();

    ///     DeadCodeElimination deadCodeElimination = new DeadCodeElimination(code);
    ///     deadCodeElimination.Optimize();

    ///     Console.WriteLine(code);
    /// </summary>
    public class DeadCodeElimination : IOptimizer
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="threeAddrCode">Трехадресный код</param>
        public DeadCodeElimination(ThreeAddrCode threeAddrCode)
        {
            Code = threeAddrCode;
        }

        protected virtual Dictionary<string, bool> PrepareVitalityVars(Block block)
        {
            return new Dictionary<string, bool>();
        }

        private Block DCEAlgorithm(Dictionary<string, bool> idLife, Block block)
        {
            // idLife - ассоциативный массив "переменная - живучесть"

            int listSize = block.Count; //количество строк в блоке
            List<int> removeIndexList = new List<int>(); //лист удаляемых номеров строк кода ББл

            // цикл по строкам кода ББл
            for (int i = listSize - 1; i >= 0; --i)
            {
                //if (block[i].IsNot<Line.Identity>() && block[i].IsNot<Line.BinaryExpr>() && block[i].IsNot<Line.UnaryExpr>()) continue;

                if (block[i].Is<Line.Identity>())
                {
                    var temp = block[i] as Line.Identity;

                    if (temp.left == temp.right)
                    {
                        //idLife[line.left] = false;//переменная в левой части "мертвая"
                        removeIndexList.Add(i); //добавляем номер текущей строки в лист для удаления
                    }
                    else
                    {
                        idLife[temp.right] = true;

                        //если для переменной в левой части есть значение "живучести"
                        bool isAlive;
                        if (idLife.TryGetValue(temp.left, out isAlive))
                        {
                            //если переменная в левой части "живая"
                            if (isAlive) idLife[temp.left] = false; //делаем ее "мертвой"
                            else
                            {
                                removeIndexList.Add(i); //добавляем номер текущей строки в лист для удаления
                            }
                        }
                        else
                        {
                            idLife[temp.left] = false; //делаем ее "мертвой"
                        }
                    }

                }
                else if (block[i].Is<Line.BinaryExpr>())
                {
                    var line = block[i] as Line.BinaryExpr;

                    idLife[line.first] = true; //первый операнд правой части "живой"
                    idLife[line.second] = true; //второй операнд правой части "живой"

                    //если для переменной в левой части есть значение "живучести"
                    bool isAlive;
                    if (idLife.TryGetValue(line.left, out isAlive))
                    {
                        //если переменная в левой части "живая"
                        if (isAlive) idLife[line.left] = false; //делаем ее "мертвой"
                        else removeIndexList.Add(i); //добавляем номер текущей строки в лист для удаления
                    }
                    else
                    {
                        idLife[line.left] = false; //делаем ее "мертвой"
                    }
                }
                else if (block[i].Is<Line.UnaryExpr>())
                {
                    var line = block[i] as Line.UnaryExpr;

                    if (line.left != line.argument)
                        idLife[line.argument] = true;

                    //если для переменной в левой части есть значение "живучести"
                    bool isAlive;
                    if (idLife.TryGetValue(line.left, out isAlive))
                    {
                        //если переменная в левой части "живая"
                        if (isAlive) idLife[line.left] = false; //делаем ее "мертвой"
                        else removeIndexList.Add(i); //добавляем номер текущей строки в лист для удаления
                    }
                    else
                    {
                        idLife[line.left] = false; //делаем ее "мертвой"
                    }

                }
            }

            //удаление строк "мертвого" кода
            for (int i = 0; i < removeIndexList.Count; i++)
            {
                block.RemoveAt(removeIndexList[i]);
            }

            return block as Block; //возвращаем измененный блок
        }

        public override void Optimize(params Object[] values)
        {
            for (int i = 0; i < Code.blocks.Count; ++i)
            {
                var idLife = PrepareVitalityVars(Code.blocks[i]);
                Code.blocks[i] = DCEAlgorithm(idLife, Code.blocks[i]);
            }
        }
    }
}
