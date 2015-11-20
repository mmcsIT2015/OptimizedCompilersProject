using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Compiler
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

    ///     DeadCodeElimination deadCodeElimination = new DeadCodeElimination(code/*, 1*/);
    ///     deadCodeElimination.Optimize();

    ///     Console.WriteLine(code);
    /// </summary>
    public class DeadCodeElimination : IOptimizer
    {
        private int mBlockNumber;

        /// <summary>
        ///
        /// </summary>
        /// <param name="threeAddrCode">Трехадресный код</param>
        /// <param name="blockNumber">Номер нужного нам базового блока, нумерация с 1; Если -1 - то для всех блоков</param>
        public DeadCodeElimination(ThreeAddrCode threeAddrCode, int blockNumber = -1)
        {
            Code = threeAddrCode;
            mBlockNumber = blockNumber;
        }

        private Block DCEAlgorithm(ThreeAddrCode threeAddrCode, int blockNumber)
        {
            var block = threeAddrCode.blocks[blockNumber - 1]; //берем блок из листа блоков трехадресного кода
            var dotLine = block;
            int listSize = dotLine.Count; //количество строк в блоке
            Dictionary<string, bool> idLife = new Dictionary<string, bool>(); //ассоциативный массив "переменная - живучесть"
            List<int> removeIndexList = new List<int>(); //лист удаляемых номеров строк кода ББл


            //цикл по строкам кода ББл
            for (int i = listSize - 1; i >= 0; --i)
            {
                //if (dotLine[i].IsNot<Line.Identity>() && dotLine[i].IsNot<Line.BinaryExpr>() && dotLine[i].IsNot<Line.UnaryExpr>()) continue;
                
                if (dotLine[i].Is<Line.Identity>())
                {
                    var temp = dotLine[i] as Line.Identity;
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
                            else removeIndexList.Add(i); //добавляем номер текущей строки в лист для удаления
                        }
                        else
                        {
                            idLife[temp.left] = false; //делаем ее "мертвой"
                        }
                    }

                }
                else if (dotLine[i].Is<Line.BinaryExpr>())
                {
                    var line = dotLine[i] as Line.BinaryExpr;

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
                else if (dotLine[i].Is<Line.UnaryExpr>())
                {
                    var line = dotLine[i] as Line.UnaryExpr;

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
                dotLine.RemoveAt(removeIndexList[i]);
            }

            return dotLine as Block; //возвращаем измененный блок
        }

        public override void Optimize(params Object[] values)
        {
            if (mBlockNumber < 0)
            {
                for (int i = 0; i < Code.blocks.Count; ++i)
                {
                    Code.blocks[i] = DCEAlgorithm(Code, i + 1);
                }
            }
            else
            {
                Debug.Assert(mBlockNumber >= 1 && mBlockNumber <= Code.blocks.Count);
                Code.blocks[mBlockNumber - 1] = DCEAlgorithm(Code, mBlockNumber);
            }
        }
    }
}
