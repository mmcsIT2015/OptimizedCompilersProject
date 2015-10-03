using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    class UniqueIdsGenerator
    {
        private static UniqueIdsGenerator inst;                

        //нужно для того, чтобы каждый раз при запуске программы
        //генерировалась одна и та же последовательность строк
        //полезно для дебага
        private const int seed = 1;

        //внутренний генератор случайных чисел
        private Random randomGenerator;

        //алфавит символов для генерации строк
        private string alp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Да, он приватный. Так надо.
        /// </summary>
        private UniqueIdsGenerator()
        {
            randomGenerator = new Random(seed);
        }

        /// <summary>
        /// Функция получения ссылки на генератор
        /// </summary>
        /// <returns>Генератор для использования, создается, если его еще нет</returns>
        public static UniqueIdsGenerator Instance()
        {            
            if (inst == null)
                inst = new UniqueIdsGenerator();
            return inst;
        }

        /// <summary>
        /// Возвращает случайную строку
        /// </summary>
        /// <returns>Строка</returns>
        public string Get(int length)
        {
            StringBuilder sb = new StringBuilder("tmp_");
            for (int i = 0; i < length; ++i)
                sb.Append(alp[randomGenerator.Next(alp.Length)]);
            return sb.ToString();
        }
    }
}
