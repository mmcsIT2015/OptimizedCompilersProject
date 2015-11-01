//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;

//namespace SimpleLang
//{
//    /// <summary>
//    /// Класс удаляет "мертвый" код из трехадресного кода в пределах одного базового блока
//    /// Пример использования:
//    /// ====
//    ///     Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
//    ///     codeGenerator.Visit(parser.root);
//    ///     
//    ///     BaseBlocksPartition baseBlocksPartition = new BaseBlocksPartition(codeGenerator.Code);
//    ///
//    ///     DeadCodeElimination deadCodeElimination = new DeadCodeElimination(codeGenerator.Code/*, 1*/);
//    ///     deadCodeElimination.Optimize();
//    ///     
//    ///     Console.WriteLine(codeGenerator.Code);
//    /// </summary>
//    class DeadCodeElimination : IOptimizer
//    {
//        private int mBlockNumber;

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="threeAddrCode">Трехадресный код</param>
//        /// <param name="blockNumber">Номер нужного нам базового блока, нумерация с 1; Если -1 - то для всех блоков</param>
//        public DeadCodeElimination(ThreeAddrCode threeAddrCode, int blockNumber = -1)
//        {
//            Code = threeAddrCode;
//            mBlockNumber = blockNumber;
//        }

//        private Block DCEAlgorithm(ThreeAddrCode threeAddrCode, int blockNumber)
//        {
//            var block = threeAddrCode.blocks[blockNumber - 1]; //берем блок из листа блоков трехадресного кода
//            List<ThreeAddrCode.Line> listThreeAddrCodeDotLine = block;
//            int listSize = listThreeAddrCodeDotLine.Count; //количество строк в блоке
//            Dictionary<string, bool> idLife = new Dictionary<string, bool>();//ассоциативный массив "переменная - живучесть"
//            List<int> removeIndexList = new List<int>();//лист удаляемых номеров строк кода ББл

//            bool isAlive;//временная переменная

//            //цикл по строкам кода ББл
//            for (int i = listSize - 1; i >= 0; --i)
//            {
//                ThreeAddrCode.Line threeAddrCodeDotLine = listThreeAddrCodeDotLine[i];//i-я строка

//                bool wonderful_flag = threeAddrCodeDotLine.left == threeAddrCodeDotLine.second && threeAddrCodeDotLine.first == "";//если строка вида "x = x"

//                if (!wonderful_flag)
//                {
//                    idLife[threeAddrCodeDotLine.first] = true;//первый операнд правой части "живой"
//                    idLife[threeAddrCodeDotLine.second] = true;//второй операнд правой части "живой"

//                    //если для переменной в левой части есть значение "живучести"
//                    if (idLife.TryGetValue(threeAddrCodeDotLine.left, out isAlive))
//                        //если переменная в левой части "живая"
//                        if (isAlive)
//                            idLife[threeAddrCodeDotLine.left] = false;//делаем ее "мертвой"
//                        //иначе
//                        else
//                            removeIndexList.Add(i);//добавляем номер текущей строки в лист для удаления
//                    //иначе
//                    else
//                        idLife[threeAddrCodeDotLine.left] = false;//делаем ее "мертвой"
//                }
//                else
//                {
//                    //idLife[threeAddrCodeDotLine.left] = false;//переменная в левой части "мертвая"
//                    removeIndexList.Add(i);//добавляем номер текущей строки в лист для удаления
//                }
//            }

//            //удаление строк "мертвого" кода
//            for (int i = 0; i < removeIndexList.Count; i++)
//                listThreeAddrCodeDotLine.RemoveAt(removeIndexList[i]);

//            return listThreeAddrCodeDotLine as Block;//возвращаем измененный блок

//        }

//        public override void Optimize(params Object[] values)
//        {
//            if (mBlockNumber < 0)
//            {
//                for (int i = 0; i < Code.blocks.Count; ++i)
//                {
//                    Code.blocks[i] = DCEAlgorithm(Code, i + 1);
//                }
//            }
//            else
//            {
//                Debug.Assert(mBlockNumber >= 1 && mBlockNumber <= Code.blocks.Count);
//                Code.blocks[mBlockNumber - 1] = DCEAlgorithm(Code, mBlockNumber);
//            }
//        }
//    }
//}
