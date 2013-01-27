using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using smallshogi.search;

namespace smallshogi
{
    using Bits = System.UInt32;

	public class PNSearch : Search
	{
		static Func<Node, int> PN = n => n.pn;
		static Func<Node, int> DN = n => n.dn;

		public Hashtable transposition = new Hashtable();
		public int nodeCount = 0;

		public Node root;
        int timeLimit, timeSpent;
        bool graph;

		public PNSearch (bool graph)
		{
            this.graph = graph;
		}

        public PNSearch(bool graph, int timeLimit)
        {
            this.graph = graph;
            this.timeLimit = timeLimit;
        }

		public void Prove (Game g)
		{
			root = new Node(g.startingPos, 1);
			root.Evaluate(g);
            transposition[root] = root;
            Node lastExpanded = null;
            int lastpn = 0, lastdn = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();
			while (root.pn != 0 && root.dn != 0) {
				var mpn = MostProving(root);

                if (System.Object.ReferenceEquals(mpn, lastExpanded) && lastpn == mpn.pn && lastdn == mpn.dn)
                {
                    sw.Stop();
                    timeSpent = (int)sw.ElapsedMilliseconds;
                    throw new Exception("Loop in finding most proving");
                }

                if (graph)
                {
                    mpn.Expand(g, transposition);
                    mpn.StartUpdate();
                }
                else
                {
                    mpn.ExpandTree(g, ref  nodeCount);
                    mpn.StartUpdateTree();
                }
                lastExpanded = mpn;
                lastpn = mpn.pn;
                lastdn = mpn.dn;

                if (sw.ElapsedMilliseconds > timeLimit * 60000)
                {
                    sw.Stop();
                    timeSpent = (int)sw.ElapsedMilliseconds;
                    throw new Exception("Time limit exceeded");
                }
			}
            sw.Stop();
            timeSpent = (int) sw.ElapsedMilliseconds;
		}

		public Node MostProving (Node v)
		{
			Node.InitiateVisiting ();
			while (v.children != null) {
				if (v.c == 1)
					v = LeftmostNode (v, PN);
				else
					v = LeftmostNode (v, DN);
				if(v.IsVisited ()) {
					v.pn *= 2;
					v.dn *= 2;
					break;
				}
			}
			return v;
		}

		public Node LeftmostNode (Node v, Func<Node, int> f)
		{
			v.SetVisit ();
			foreach (var child in v.children)
				if (f(child) == f(v) && !child.IsVisited ())
					return child;
			return v;
		}

        public void SetTimeLimit(int minutes)
        {
            timeLimit = minutes;
        }

        public int Value()
        {
            return root.pn==0?1:root.dn==0?-1:0;
        }

        public int TimeSpent()
        {
            return timeSpent;
        }

        public int NodeCount()
        {
            if (graph)
                return transposition.Count;
            else
                return nodeCount;
        }

        public List<Bits[]> BestGame()
        {
            var chain = root.GetLongestGame();
            List<Bits[]> result = new List<Bits[]>();
            foreach (var node in chain)
                result.Add(node.position);
            return result;
        }

        public String Name()
        {
            if (graph)
                return "PNG";
            else
                return "PNT";
        }
	}
}

