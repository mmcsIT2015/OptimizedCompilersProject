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
    ///     public Optimizer(ThreeAddrCode input) {
    ///         Code = input;
    ///     }
    ///     
    ///     public override void Optimize() {
    ///         // optimize
    ///     }
    /// };
    /// </summary>
    class IOptimizer
    {
        public ThreeAddrCode Code { get; set; }

        public virtual void Optimize(params Object[] values) { }

        /// <summary>
        /// Ф-я нужна для будущего класса, который будет заниматься применением набора оптимизаций.
        /// </summary>
        public virtual void Assign(ThreeAddrCode code)
        {
            Code = code;
        }
    }
}
