using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    // Пока так
    using Set = List<string>;

    class DataFlow
    {
        public class TransferFunction {
            private Set mGen = new Set();
            private Set mKill = new Set();

            public TransferFunction(Set gen, Set kill)
            {
                mGen = gen;
                mKill = kill;
            }

            /// <summary>
            /// Т.к. перегрузить круглые скобки нельзя, вызов этой функции будет аналогом вызова f(x)
            /// </summary>
            public Set Map(Set x)
            {
                return mGen.Union(x.Except(mKill)) as Set;
            }

            /// <summary>
            /// Композиция двух передаточных функций: (`this` . f1)
            /// </summary>
            public TransferFunction Map(TransferFunction f1)
            {
                var g12 = mGen.Union(f1.mGen.Except(mKill)) as Set;
                var k12 = mKill.Union(f1.mKill) as Set;
                return new TransferFunction(g12, k12);
            }

            /// <summary>
            /// Возвращает композицию последовательности передаточных функций.
            /// Функции передаются в порядке Fn, F_{n-1}, ..., F1, 
            /// в результате будет возвращена функция (Fn . F_{n-1} . ... . F1)(x).
            /// Пример использования:
            /// var f1 = new DataFlow.TransferFunction(new List<string>() { "d1", "d2", "d3" }, new List<string>() { "d1", "d4", "d5" });
            /// var f2 = new DataFlow.TransferFunction(new List<string>() { "d4", "d5", "d3" }, new List<string>() { "d1", "d2", "d3" });
            /// var f3 = new DataFlow.TransferFunction(new List<string>() { "d4", "d7", "d3" }, new List<string>() { "d2", "d3" });
            /// var f4 = new DataFlow.TransferFunction(new List<string>() { "d4", "d5" }, new List<string>() { "d1", "d2" });
            /// var f = DataFlow.TransferFunction.Сomposition(f1, f2, f3, f4); // f(x) = (f1 . f2 . f3 . f4)(x)
            /// </summary>
            public static TransferFunction Сomposition(params TransferFunction[] funcs)
            {
                if (funcs.Length < 2)
                {
                    throw new ArgumentException("funcs.Length < 2!");
                }

                IEnumerable<string> kill = new Set();
                foreach (var func in funcs)
                {
                    kill = kill.Union(func.mKill);
                }

                IEnumerable<string> gen = new Set();
                for (int i = 0; i < funcs.Length; ++i)
                {
                    IEnumerable<string> chunk = new Set(funcs[i].mGen);
                    for (int j = i - 1; j >= 0; --j)
                    {
                        chunk = chunk.Except(funcs[j].mKill);
                    }
                    Console.WriteLine();

                    gen = gen.Union(chunk);
                }

                return new TransferFunction(gen as Set, kill as Set);
            }

            /// <summary>
            /// Функция для команды DoubleK.
            /// </summary>
            public static TransferFunction StupidСomposition(params TransferFunction[] funcs)
            {
                // TODO
                // Скорее всего, нужно использовать `TransferFunction Map(TransferFunction f1)`
                return new TransferFunction(new Set(), new Set());
            }
        }
    }
}
