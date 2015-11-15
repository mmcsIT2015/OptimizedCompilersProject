using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class IterativeAlgo<AData, TrFunc>
        //where AData : IEqualityComparer<AData>
        where TrFunc : ITransferFunction<AData>
    {
        public Dictionary<Block, IEnumerable<AData>> In { get; set; }
        public Dictionary<Block, IEnumerable<AData>> Out { get; set; }

        public ISemilattice<AData> Semilattice; // полурешетка
        public Dictionary<Block, TrFunc> TransferFuncs; // передаточные функции (для блоков если AData = Block)

        public IterativeAlgo(ISemilattice<AData> semilattice, Dictionary<Block, TrFunc> transferFuncs)
        {
            In = new Dictionary<Block, IEnumerable<AData>>();
            Out = new Dictionary<Block, IEnumerable<AData>>();

            Semilattice = semilattice;
            TransferFuncs = transferFuncs;
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
                    var edges = graph.InEdges(block);
                    if (edges.Count() == 0) continue;

                    var temp = Out[edges.First()];
                    foreach (var p in edges.Skip(1))
                    {
                        temp = Semilattice.Join(temp, Out[p]);
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
