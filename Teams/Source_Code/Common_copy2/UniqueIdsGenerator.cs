using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SimpleLang
{
    class UniqueIdsGenerator
    {
        private static UniqueIdsGenerator mInstance;                

        //нужно для того, чтобы каждый раз при запуске программы
        //генерировалась одна и та же последовательность строк
        //полезно для дебага
        private const int mSeed = 1;

        //внутренний генератор случайных чисел
        private Random mRandomGenerator;

        //алфавит символов для генерации строк
        private string mAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private HashSet<string> mUsed = new HashSet<string>();

        /// <summary>
        /// Да, он приватный. Так надо.
        /// </summary>
        private UniqueIdsGenerator()
        {
            mRandomGenerator = new Random(mSeed);
        }

        /// <summary>
        /// Функция получения ссылки на генератор
        /// </summary>
        /// <returns>Генератор для использования, создается, если его еще нет</returns>
        public static UniqueIdsGenerator Instance()
        {
            if (mInstance == null) mInstance = new UniqueIdsGenerator();

            return mInstance;
        }

        /// <summary>
        /// Возвращает случайную строку
        /// </summary>
        /// <returns>Строка</returns>
        public string Get(int length)
        {
            if (length > 64)
            {
                // На всякий случай, меру надо знать в длине идентификаторов...
                throw new ArgumentException("Too long name ID: " + length + ". Maximum length - 64");
            }

            int attempt = 0;
            StringBuilder builder;
            double limit = Math.Pow(mAlphabet.Length, length);
            do
            {
                // @ - Такого символа точно быть в программе не могло
                builder = new StringBuilder("@");
                for (int i = 0; i < length; ++i)
                {
                    builder.Append(mAlphabet[mRandomGenerator.Next(mAlphabet.Length)]);
                }
            }
            while ((++attempt < limit) && mUsed.Contains(builder.ToString()));

            if (attempt >= limit)
            {
                throw new Exception("No free IDs with length " + length);
            }

            mUsed.Add(builder.ToString());
            return builder.ToString();
        }
    }
}
