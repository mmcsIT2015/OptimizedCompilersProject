using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iCompiler
{
    public class TransferFunction<T> : ITransferFunction<T>
    {
        private IEnumerable<T> mGen;
        private IEnumerable<T> mKill;

        public TransferFunction(IEnumerable<T> gen, IEnumerable<T> kill)
        {
            mGen = gen;
            mKill = kill;
        }

        /// <summary>
        /// Т.к. перегрузить круглые скобки нельзя, вызов этой функции будет аналогом вызова f(x)
        /// </summary>
        public IEnumerable<T> Map(IEnumerable<T> x)
        {
            return mGen.Union(x.Except(mKill));
        }

        /// <summary>
        /// Композиция двух передаточных функций: (`this` . f1)
        /// </summary>
        public ITransferFunction<T> Map(ITransferFunction<T> f1)
        {
            Debug.Assert(f1 is TransferFunction<T>);

            var g12 = mGen.Union((f1 as TransferFunction<T>).mGen.Except(mKill)) as IEnumerable<T>;
            var k12 = mKill.Union((f1 as TransferFunction<T>).mKill) as IEnumerable<T>;
            return new TransferFunction<T>(g12, k12);
        }

        /// <summary>
        /// Возвращает композицию последовательности передаточных функций.
        /// Функции передаются в порядке Fn, F_{n-1}, ..., F1, 
        /// в результате будет возвращена функция (Fn . F_{n-1} . ... . F1)(x).
        /// Пример использования:
        /// var f1 = new TransferFunction<string>(new List<string>() { "d1", "d2", "d3" }, new List<string>() { "d1", "d4", "d5" });
        /// var f2 = new TransferFunction<string>(new List<string>() { "d4", "d5", "d3" }, new List<string>() { "d1", "d2", "d3" });
        /// var f3 = new TransferFunction<string>(new List<string>() { "d4", "d7", "d3" }, new List<string>() { "d2", "d3" });
        /// var f4 = new TransferFunction<string>(new List<string>() { "d4", "d5" }, new List<string>() { "d1", "d2" });
        /// var f = TransferFunction.Сomposition(f1, f2, f3, f4); // f(x) = (f1 . f2 . f3 . f4)(x)
        /// </summary>
        public static TransferFunction<T> Сomposition(params TransferFunction<T>[] funcs)
        {
            if (funcs.Length < 2)
            {
                throw new ArgumentException("funcs.Length < 2!");
            }

            var kill = new HashSet<T>();
            foreach (var func in funcs)
            {
                kill = kill.Union(func.mKill) as HashSet<T>;
            }

            var gen = new HashSet<T>();
            for (int i = 0; i < funcs.Length; ++i)
            {
                var chunk = new HashSet<T>(funcs[i].mGen);
                for (int j = i - 1; j >= 0; --j)
                {
                    chunk = chunk.Except(funcs[j].mKill) as HashSet<T>;
                }
                Console.WriteLine();

                gen = gen.Union(chunk) as HashSet<T>;
            }

            return new TransferFunction<T>(gen, kill);
        }

        public static TransferFunction<T> SuperPosition(params TransferFunction<T>[] funcs)
        {
            if (funcs.Length < 2)
            {
                throw new ArgumentException("funcs.Length < 2!");
            }

            TransferFunction<T> tf = funcs[funcs.Length - 1];

            for (int i = funcs.Length - 2; i >= 0; --i)
            {
                tf = funcs[i].Map(tf) as TransferFunction<T>;
            }

            return tf;
        }
    }

    public class TransferFunctionForDraggingConstants : ITransferFunction<ConstNACInfo>
    {
        private IEnumerable<ConstNACInfo> mGen;

        public TransferFunctionForDraggingConstants(IEnumerable<ConstNACInfo> gen)
        {
            mGen = gen;
        }

        /// <summary>
        /// Т.к. перегрузить круглые скобки нельзя, вызов этой функции будет аналогом вызова f(x)
        /// </summary>
        public IEnumerable<ConstNACInfo> Map(IEnumerable<ConstNACInfo> x)
        {
            var newGen = new HashSet<ConstNACInfo>(mGen);
            foreach (ConstNACInfo c in x)
            {
                var tmp = mGen.Count(el => el.VarName == c.VarName);

                foreach (var kk in mGen)
                    Debug.Assert(mGen.Count(el => el.VarName == kk.VarName) <= 1);
                if (tmp > 0)
                    continue;
                else
                    newGen.Add(new ConstNACInfo(c));
            }
            return newGen;
        }

        /// <summary>
        /// Композиция двух передаточных функций: (`this` . f1)
        /// </summary>
        public ITransferFunction<ConstNACInfo> Map(ITransferFunction<ConstNACInfo> f1)
        {
            Debug.Assert(f1 is TransferFunctionForDraggingConstants);

            return new TransferFunctionForDraggingConstants(this.Map(mGen));
        }
    }
}
