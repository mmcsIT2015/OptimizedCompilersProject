using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCompiler
{
    /// <summary>
    /// ---
    ///     var seq = new AscendingSequenceOfRegions();
    ///     seq.MakeSequence(code);
    ///     seq.PrintSequence();
    ///     
    /// </summary>
    public class AscendingSequenceOfRegions
    {
        private ThreeAddrCode mCode;
        public List<Region> regions;

        public void MakeSequence(ThreeAddrCode code)
        {
            mCode = code;
            Region first = Region.RegionsDetermination(code);

            List<Region> acc = new List<Region>();
            Dictionary<Region, bool> used = new Dictionary<Region, bool>();

            List<Region> regions = new List<Region>();
            List<Region> initial = new List<Region>();
            regions.Add(first);
            initial.Add(first);
            while (regions.Last().NextRegions.Count > 0)
            {
                var last = regions.Last();
                foreach (var r in last.NextRegions)
                {
                    initial.Add(r);
                    regions.Add(r);
                }
            }

            for (; ; )
            {
                bool found = false;
                var added = new List<Region>();
                var removed = new List<Region>();
                for (int i = 0; i < regions.Count; ++i)
                {
                    if (regions[i] is CycleRegion)
                    {
                        found = true;
                        acc.Add(regions[i]);
                        removed.Add(regions[i]);

                        added.Add((regions[i] as CycleRegion).CycleBody.Head);
                        while (added.Last().NextRegions.Count > 0)
                        {
                            var last = added.Last();
                            foreach (var r in last.NextRegions) added.Add(r);
                        }
                    }
                }

                foreach (var r in removed) regions.Remove(r);
                foreach (var r in added) regions.Add(r);

                if (!found) break;
            }

            /* Первый уровень регионов - ББл, в топологическом порядке */
            acc.Reverse();

            var d = new Dictionary<Block, Region>();
            foreach (var r in regions)
            {
                d.Add((r as SimpleRegion).Block, r);
            }

            HashSet<Block> used2 = new HashSet<Block>();
            List<Block> stack = new List<Block>();
            stack.Add(code.graph.EntryPoint());
            used2.Add(code.graph.EntryPoint());
            int counter = 0;
            while (stack.Count > 0)
            {
                List<Block> temp = new List<Block>();
                foreach (var node in stack)
                {
                    acc.Insert(counter++, d[node]);
                    foreach (var e in code.graph.OutEdges(node))
                    {
                        if (!used2.Contains(e))
                        {
                            used2.Add(e);
                            temp.Add(e);
                        }
                    }
                }

                stack = temp;
            }

            this.regions = acc;
        }

        public void PrintSequence() {
            Debug.Assert(regions != null);

            int counter = 0;
            StringBuilder sb = new StringBuilder("Sequence\n");
            foreach (var r in regions)
            {
                if (r is SimpleRegion)
                {
                    sb.AppendLine(counter + ": " + '\t' + "Block " + mCode.blocks.IndexOf((r as SimpleRegion).Block).ToString());
                }
                else if (r is CycleRegion)
                {
                    sb.AppendLine(counter + ": " + '\t' + "Cycle {");
                    Region.TestR(mCode, sb, (r as CycleRegion).CycleBody.Head, '\t' + "\t");
                    sb.AppendLine('\t' + "}");
                }

                ++counter;
            }

            Console.WriteLine(sb);
        }
    }
}
