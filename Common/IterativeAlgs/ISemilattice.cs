using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    interface ISemilattice<T>
    {
        IEnumerable<T> Join(IEnumerable<T> lhs, IEnumerable<T> rhs); // оператор сбора
        IEnumerable<T> Top(); // T - верхний элемент
    }
}
