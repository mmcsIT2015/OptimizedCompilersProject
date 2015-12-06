using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler
{
    class GraphEdges<T>
    {
        private Dictionary<T, int> Numbers = new Dictionary<T, int>();
        private Dictionary<T, List<T>> Data = new Dictionary<T, List<T>>();
        private Dictionary<T, IEnumerable<T>> Dom = new Dictionary<T, IEnumerable<T>>();

        public GraphEdges(SpanningTree<T> spanningTree, Dictionary<T, IEnumerable<T>> Dom)
        {
            this.Data = spanningTree.Data;
            this.Numbers = spanningTree.Numbers;
            this.Dom = Dom;
        }

        private IEnumerable<DomGraph.ValPair<T>> RetreatingEdges()
        {
            List<DomGraph.ValPair<T>> listRetreat = new List<DomGraph.ValPair<T>>();

            foreach (T Tkey in Data.Keys)
                for (int i = 0; i < Data[Tkey].Count; ++i)
                    if (Numbers[Tkey] > Numbers[Data[Tkey][i]])
                        listRetreat.Add(new DomGraph.ValPair<T>(Tkey, Data[Tkey][i]));

            return listRetreat;

        }

        private IEnumerable<DomGraph.ValPair<T>> ReversedEdges(Dictionary<T, IEnumerable<T>> Dom)
        {
            List<DomGraph.ValPair<T>> listEdges = new List<DomGraph.ValPair<T>>();

            foreach (T a in Dom.Keys)
                foreach (T b in Dom[a])
                    //if a -> b
                    if (Data[a].Contains(b))
                        listEdges.Add(new DomGraph.ValPair<T>(a, b));

            return listEdges;

        }

        public bool IsGraphGiven()
        {
            List<DomGraph.ValPair<T>> listReversed = ReversedEdges(this.Dom) as List<DomGraph.ValPair<T>>;
            List<DomGraph.ValPair<T>> listRetreat = RetreatingEdges() as List<DomGraph.ValPair<T>>;

            if (listReversed.Count == listRetreat.Count)
            {
                for (int i = 0; i < listReversed.Count; ++i)
                    if (!(listReversed[i].valBegin.Equals(listRetreat[i].valEnd) && listReversed[i].valBegin.Equals(listRetreat[i].valBegin)))
                        return false;
            }
            else
                return false;

            return true;

        }
    }
}
