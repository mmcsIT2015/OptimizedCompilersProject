using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    /// <summary>
    /// Пример:
    /// ===
    /// class Optimizer: IOptimizer {
    ///     private ThreeAddrCode code;
    ///     
    ///     public Optimizer(ThreeAddrCode input) {
    ///         code = input;
    ///     }
    ///     
    ///     public override void Optimize() {
    ///         // optimize
    ///     }
    /// };
    /// </summary>
    public interface IOptimizer
    {
        void Optimize(params Object[] values);
    }
}
