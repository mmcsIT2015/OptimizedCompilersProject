using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public interface ITransferFunction<T>
    {
        IEnumerable<T> Map(IEnumerable<T> x);
        ITransferFunction<T> Map(ITransferFunction<T> f1); // композиция
    }
}
