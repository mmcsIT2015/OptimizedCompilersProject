using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    class IterativeAlgo<AData, TrFunc>
        where AData : IEqualityComparer<AData>
        where TrFunc : ITransferFunction<AData>
    {
        public Dictionary<Block, IEnumerable<AData>> In { get; set; }
        public Dictionary<Block, IEnumerable<AData>> Out { get; set; }

        public ISemilattice<AData> Semilattice; // полурешетка
        // TODO
        public Dictionary<Block, TrFunc> TransferFuncs = new Dictionary<Block, TrFunc>(); // передаточные функции (для блоков если AData = Block)

        public IterativeAlgo(ISemilattice<AData> semilattice)
        {
            Semilattice = semilattice;
        }

        public void Run(ThreeAddrCode code)
        {
            var graph = code.graph; // граф управления потоком
            var blocks = code.blocks;

            foreach (var block in blocks)
            {
                In.Add(block, new HashSet<AData>());
                Out.Add(block, new HashSet<AData>(Semilattice.Top())); // OUT[B] = T
            }
            Out[graph.EntryPoint()] = new HashSet<AData>(); // OUT[вход] = 0

            bool hasChanges = false;
            do
            {
                var old = new Dictionary<Block, IEnumerable<AData>>(Out);

                foreach (var block in blocks)
                {
                    var temp = new HashSet<AData>();
                    foreach (var p in graph.InEdges(block))
                    {
                        temp = Semilattice.Join(temp, Out[p]) as HashSet<AData>;
                    }

                    In[block] = temp;
                    Out[block] = TransferFuncs[block].Map(In[block]);
                }

                hasChanges = false;
                foreach (var block in blocks) {
                    if (!old[block].Equals(Out[block]))
                    {
                        hasChanges = true;
                        return;
                    }
                }
            } while (hasChanges);
        }
    }
}
