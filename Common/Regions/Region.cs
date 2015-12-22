using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    /// <summary>
    /// Область
    /// </summary>
    public class Region
    {
        public List<Region> PrevRegions { get; set; }
        /// <summary>
        /// Список следующих областей
        /// </summary>
        public List<Region> NextRegions { get; set; }

        public Region()
        {
            NextRegions = new List<Region>();
            PrevRegions = new List<Region>();
        }

        private static void RollUpCycle(AllCyclesHierarchy<Block> cycles, Cycle<Block> cycle, Dictionary<Block, Region> block_to_region)
        {
            foreach (Cycle<Block> c in cycles.data[cycle])
                RollUpCycle(cycles, c, block_to_region);

            CycleRegion reg = new CycleRegion();
            reg.CycleBody = new CycleBodyRegion();
            reg.CycleBody.Head = block_to_region[cycle.N];
            for (int i = 0; i < cycle.OUTS.Count; ++i)
            {
                Region regBegin = block_to_region[cycle.OUTS[i].valBegin];
                Region regEnd = block_to_region[cycle.OUTS[i].valEnd];

                while (regBegin.NextRegions.Remove(regEnd))
                {
                    regEnd.PrevRegions.Remove(regBegin);

                    reg.NextRegions.Add(regEnd);
                    regEnd.PrevRegions.Add(reg);

                    reg.CycleBody.OutsEdges.Add(regBegin, regEnd);
                }
            }

            if (cycle is CycleUsual<Block>)
            {
                CycleUsual<Block> cu = cycle as CycleUsual<Block>;
                Region sr = block_to_region[cu.D];
                while (sr.NextRegions.Remove(reg.CycleBody.Head))
                {
                    reg.CycleBody.Head.PrevRegions.Remove(sr);

                    reg.ReversedRegions.Add(sr);
                }
            }
            else if (cycle is CycleSpecialCase<Block>)
            {
                CycleSpecialCase<Block> csc = cycle as CycleSpecialCase<Block>;
                Region sr1 = block_to_region[csc.D1];
                while (sr1.NextRegions.Remove(reg.CycleBody.Head))
                {
                    reg.CycleBody.Head.PrevRegions.Remove(sr1);

                    reg.ReversedRegions.Add(sr1);
                }
                Region sr2 = block_to_region[csc.D2];
                while (sr2.NextRegions.Remove(reg.CycleBody.Head))
                {
                    reg.CycleBody.Head.PrevRegions.Remove(sr2);

                    reg.ReversedRegions.Add(sr2);
                }
            }
            else
                throw new Exception("RollUpCycle: unexpected exception");

            for (int i = 0; i < reg.CycleBody.Head.PrevRegions.Count; ++i)
            {
                while (reg.CycleBody.Head.PrevRegions[i].NextRegions.Remove(reg.CycleBody.Head))
                {
                    reg.CycleBody.Head.PrevRegions[i].NextRegions.Add(reg);
                }
            }
            reg.CycleBody.Head.PrevRegions.Clear();

            Region r = reg.CycleBody.Head;
            while (!(r is SimpleRegion))
                if (r is CycleRegion)
                    r = (r as CycleRegion).CycleBody.Head;
                else
                    throw new Exception("RollUpCycle: unexpected exception 2");

            block_to_region[(r as SimpleRegion).Block] = reg;
        }

        public static Region RegionsDetermination(ThreeAddrCode code)
        {
            // Сбор всех данных
            ControlFlowGraph cfg = new ControlFlowGraph(code.blocks);
            List<DomGraph.ValPair<Block>> reversedEdges = new List<DomGraph.ValPair<Block>>(DomGraph.ReverseEdges(DomGraph.GenerateDomOut(code), cfg)); 
            DomTree domTree = new DomTree(code);
            AllCyclesWithSpecialCase<Block> cycles = new AllCyclesWithSpecialCase<Block>(code.blocks, code.graph, reversedEdges, domTree);
            AllCyclesHierarchy<Block> cycles2 = new AllCyclesHierarchy<Block>(cycles.cycles);

            // Определение областей
            Dictionary<Block, Region> block_to_region = new Dictionary<Block, Region>();
            // Простые области
            for (int i = 0; i < code.blocks.Count; ++i)
            {
                SimpleRegion reg = new SimpleRegion();
                reg.Block = code.blocks[i];
                block_to_region.Add(code.blocks[i], reg);
            }
            // Построение графа простых областей
            for (int i = 0; i < code.blocks.Count; ++i)
            {
                IEnumerable<Block> outs = cfg.OutEdges(code.blocks[i]);
                Region reg = block_to_region[code.blocks[i]];
                foreach (Block b in outs)
                {
                    Region reg2 = block_to_region[b];
                    reg.NextRegions.Add(reg2);
                    reg2.PrevRegions.Add(reg);
                }
            }

            for (int i = 0; i < cycles2.root.Count; ++i)
                RollUpCycle(cycles2, cycles2.root[i], block_to_region);

            return block_to_region[code.blocks[0]];
        }

        public static void TestR(ThreeAddrCode code, StringBuilder sb, Region reg, string delim = "")
        {
            if (reg is SimpleRegion)
            {
                sb.AppendLine(delim + "Block " + code.blocks.IndexOf((reg as SimpleRegion).Block).ToString());
            }
            else if (reg is CycleRegion)
            {
                sb.AppendLine(delim + "Cycle {");
                TestR(code, sb, (reg as CycleRegion).CycleBody.Head, delim + "\t");
                sb.AppendLine(delim + "}");
            }

            for (int i = 0; i < reg.NextRegions.Count; ++i)
                TestR(code, sb, reg.NextRegions[i], delim);
        }

        public static string Test(ThreeAddrCode code)
        {
            Region first = RegionsDetermination(code);
            StringBuilder sb = new StringBuilder();
            TestR(code, sb, first);
            return sb.ToString();
        }
    }

    /// <summary>
    /// Область: Базовый блок
    /// </summary>
    public class SimpleRegion : Region
    {
        /// <summary>
        /// Базовый блок
        /// </summary>
        public Block Block { get; set; }
        public SimpleRegion() : base() { }
        public SimpleRegion(Block block) : base() {
            Block = block;
        }
    }

    /// <summary>
    /// Область: Тело цикла
    /// </summary>
    public class CycleBodyRegion : Region
    {
        /// <summary>
        /// Область: Вход в цикл
        /// </summary>
        public Region Head { get; set; }
        /// <summary>
        /// Список выходных ребер
        /// </summary>
        public Dictionary<Region, Region> OutsEdges { get; set; }

        public CycleBodyRegion() : base()
        {
            OutsEdges = new Dictionary<Region, Region>();
        }
    }

    /// <summary>
    /// Область: Цикл
    /// </summary>
    public class CycleRegion : Region
    {
       /// <summary>
       /// Область: тело цикла
       /// </summary>
        public CycleBodyRegion CycleBody { get; set; }
        /// <summary>
        /// Список областей, из которых выходит обратное ребро
        /// </summary>
        public List<Region> ReversedRegions { get; set;  }

        public CycleRegion() : base()
        {
            ReversedRegions = new List<Region>();
        }
    }
}
