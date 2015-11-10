using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    /// <summary>
    /// Класс изменяет входящий threeAddrCode - на выходе этот код будет разбит на блоки.
    /// Также в трехадресном коде инициализируется граф переходов между блоками.
    /// (!)
    /// Вызывать вручную разбиение на блоки не требуется - оно происходит при конструировании трехадресного кода из List<Line.Line>.
    /// </summary>
    static class BaseBlocksPartition
    {
        public static void Partition(ThreeAddrCode threeAddrCode)
        {
            if (threeAddrCode.blocks.Count() != 1) return;

            List<Block> blocks = new List<Block>();
            var currentBlock = new Block();
            foreach (var line in threeAddrCode.blocks[0])
            {
                if (line.HasLabel() && currentBlock.Count() > 0)
                {
                    blocks.Add(currentBlock);
                    currentBlock = new Block();
                }
                currentBlock.Add(line);
                if (line is Line.GoTo) // Line.ConditionalJump является GoTo
                {
                    blocks.Add(currentBlock);
                    currentBlock = new Block();
                }
            }

            if (currentBlock.Count() > 0) blocks.Add(currentBlock);

            threeAddrCode.blocks = blocks;
        }
    }
}
