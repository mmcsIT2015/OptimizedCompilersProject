using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    interface ISemilattice<T>
    {
        IEnumerable<T> Join(IEnumerable<T> lhs, IEnumerable<T> rhs); // оператор сбора
        IEnumerable<T> Top(); // T - верхний элемент
    }
}
