using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
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
    public class IOptimizer
    {
        public int NumberOfChanges { get; protected set; }

        public ThreeAddrCode Code { get; protected set; }

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
