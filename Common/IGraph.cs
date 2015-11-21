using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public interface IGraph<T>
    {
        IEnumerable<T> OutEdges(T block);
        IEnumerable<T> InEdges(T block);
    }
}
