using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
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

        private void RunAlgorithm(ThreeAddrCode code)
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
                    //if (code.GetBlockId(block) == code.GetBlockId(graph.EntryPoint())) continue;
                    var edges = graph.InEdges(block);

                    IEnumerable<AData> newIn;
                    if (edges.Count() > 0)
                    {
                        newIn = new HashSet<AData>(Out[edges.First()]);
                        for (int i = 1; i < edges.Count(); ++i)
                        {
                            int index = code.GetBlockId(edges.ElementAt(i));
                            newIn = Semilattice.Join(newIn, Out[edges.ElementAt(i)]);
                        }
                    }
                    else
                    {
                        newIn = new HashSet<AData>();
                    }
                   
                    In[block] = new HashSet<AData>(newIn);
                    Out[block] = new HashSet<AData>(TransferFuncs[block].Map(In[block]));
                }

                hasChanges = false;
                foreach (var block in blocks)
                {
                    if (old[block].Count() != Out[block].Count())
                    {
                        hasChanges = true;
                        break;
                    }

                    var l1 = old[block].ToList();
                    var l2 = Out[block].ToList();

                    hasChanges = !l1.SequenceEqual(l2);

                    if (hasChanges) break;
                }
            } while (hasChanges);
        }

        public void Run(ThreeAddrCode code)
        {
            RunAlgorithm(code);
        }

        public void RunOnReverseGraph(ThreeAddrCode code)
        {
            code.graph.ReverseMode = true;
            RunAlgorithm(code);

            var temp = new Dictionary<Block, IEnumerable<AData>>(In);
            In = Out;
            Out = temp;
            code.graph.ReverseMode = false;
        }
    }
}
