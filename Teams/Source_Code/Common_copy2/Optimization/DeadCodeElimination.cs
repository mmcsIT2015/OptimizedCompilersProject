using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    /// <summary>
    /// Класс удаляет "мертвый" код из трехадресного кода в пределах одного базового блока
    /// Пример использования:
    /// ====
    ///     Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
    ///     codeGenerator.Visit(parser.root);
    ///     
    ///     BaseBlocksPartition baseBlocksPartition = new BaseBlocksPartition(codeGenerator.Code);
    ///
    ///     DeadCodeElimination deadCodeElimination = new DeadCodeElimination(codeGenerator.Code/*, 1*/);
    ///     deadCodeElimination.Optimize();
    ///     
    ///     Console.WriteLine(codeGenerator.Code);
    /// </summary>
    class DeadCodeElimination: IOptimizer
    {
        private int mBlockNumber;
        private ThreeAddrCode mCode;

        /// <summary>
        ///
        /// </summary>
        /// <param name="threeAddrCode">Трехадресный код</param>
        /// <param name="blockNumber">Номер нужного нам базового блока, нумерация с 1; Если -1 - то для всех блоков</param>
        public DeadCodeElimination(ThreeAddrCode threeAddrCode, int blockNumber = -1)
        {
            mCode = threeAddrCode;
            mBlockNumber = blockNumber;
        }

        private Block DCEAlgorithm(ThreeAddrCode threeAddrCode, int blockNumber)
        {
            var block = threeAddrCode.blocks[blockNumber - 1]; //берем блок из листа блоков трехадресного кода
            List<ThreeAddrCode.Line> listThreeAddrCodeDotLine = block;
            int listSize = listThreeAddrCodeDotLine.Count; //количество строк в блоке

            //цикл с конца блока до начала
            for (int i = listSize - 1; i >= 0; i--)
            {
                ThreeAddrCode.Line threeAddrCodeDotLineI = listThreeAddrCodeDotLine[i]; //берем i-ю строку
                bool isAlive1, isAlive2; //"живучесть"
                isAlive1 = threeAddrCodeDotLineI.first != "";
                isAlive2 = threeAddrCodeDotLineI.second != "";
                //цикл с i-го элемента до начала блока
                for (int j = i - 1; j >= 0; j--)
                {
                    ThreeAddrCode.Line threeAddrCodeDotLineJ = listThreeAddrCodeDotLine[j];//берем j-ю строку
                    //если первый операнд i-ой строки равен левому значению j-й строки
                    if (threeAddrCodeDotLineI.first == threeAddrCodeDotLineJ.left)
                    {
                        //если переменная уже "мертва", то
                        if (isAlive1 == false)
                        {
                            i--;
                            listThreeAddrCodeDotLine.RemoveAt(j); //удаляем j-ю строку с этой переменной
                        }
                        //иначе
                        else
                            isAlive1 = false; //делаем ее "мертвой"
                    }

                    //если второй операнд i-ой строки равен левому значению j-й строки
                    if (threeAddrCodeDotLineI.second == threeAddrCodeDotLineJ.left)
                    {
                        //если переменная уже "мертва", то
                        if (isAlive2 == false)
                        {
                            i--;
                            listThreeAddrCodeDotLine.RemoveAt(j); //удаляем j-ю строку с этой переменной
                        }
                        //иначе
                        else
                            isAlive2 = false; //делаем ее "мертвой"
                    }

                    //если левое значение i-ой строки равно левому значению j-й строки
                    if (threeAddrCodeDotLineI.left == threeAddrCodeDotLineJ.left)
                    {
                        i--;
                        listThreeAddrCodeDotLine.RemoveAt(j); //удаляем j-ю строку с этой переменной
                    }
                }
            }

            return listThreeAddrCodeDotLine as Block; //возвращаем измененный базовый блок
        }

        public void Optimize(params Object[] values)
        {
            if (mBlockNumber < 0)
            {
                for (int i = 0; i < mCode.blocks.Count; ++i)
                {
                    mCode.blocks[i] = DCEAlgorithm(mCode, i + 1);
                }
            }
            else
            {
                Debug.Assert(mBlockNumber >= 1 && mBlockNumber <= mCode.blocks.Count);
                mCode.blocks[mBlockNumber - 1] = DCEAlgorithm(mCode, mBlockNumber);
            }
        }
    }
}
