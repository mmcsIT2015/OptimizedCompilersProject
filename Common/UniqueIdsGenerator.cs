using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    class UniqueIdsGenerator
    {
        private string prefix;
        private int counter = 0;

        public UniqueIdsGenerator(string prefix)
        {
            this.prefix = prefix;
        }

        public string Get()
        {
            return prefix + (counter++).ToString();
        }
    }
}
