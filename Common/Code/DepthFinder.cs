using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    public class DepthFinder<T>
    {

        private static List<T> Data;


        //Использование
        //int depth = DepthFinder<Block>.FindDepth(code.blocks, code.graph);

        private static void FindLayer(T root, IGraph<T> graph, List<int> maxs, ref int depth)
        {
            Data.Add(root);
            depth++;
            foreach (var b in graph.OutEdges(root))
                if (!Data.Contains(b))
                {
                    maxs.Add(depth);
                    FindLayer(b, graph, maxs, ref depth);
                }
        }

        public static int FindDepth(IEnumerable<T> blocks, IGraph<T> graph)
        {
            Data = new List<T>();
            int depth = 0;
            List<int> maxs = new List<int>() {0};

            FindLayer(blocks.First(), graph, maxs, ref depth);          
            return maxs.Max();
        }




        

    }
}
