<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SimpleLang
{
    using Prefix = String;

    class UniqueIdsGenerator
    {
        private static UniqueIdsGenerator mInstance;                

        // Счетчики для генерации удобочитаемых переменных
        private Dictionary<Prefix, int> mCounters = new Dictionary<Prefix, int>();

        private UniqueIdsGenerator()
        {
            
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

        public string Get(String prefix)
        {
            if (!mCounters.ContainsKey(prefix)) mCounters.Add(prefix, 0);
            return "@" + prefix + (mCounters[prefix]++).ToString();
        }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SimpleLang
{
    using Prefix = String;

    class UniqueIdsGenerator
    {
        private static UniqueIdsGenerator mInstance;                

        // Счетчики для генерации удобочитаемых переменных
        private Dictionary<Prefix, int> mCounters = new Dictionary<Prefix, int>();

        private UniqueIdsGenerator()
        {
            
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

        public string Get(String prefix)
        {
            if (!mCounters.ContainsKey(prefix)) mCounters.Add(prefix, 0);
            return "@" + prefix + (mCounters[prefix]++).ToString();
        }
    }
}
>>>>>>> 5ed01454fc05e4df8cd497e3fbd6fc8385e6541a
