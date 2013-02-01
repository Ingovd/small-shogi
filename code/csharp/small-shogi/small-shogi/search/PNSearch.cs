using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using smallshogi.search;

namespace smallshogi
{
    using Bits = System.UInt16;

	public class PNSearch : Search
	{
		public Hashtable transposition = new Hashtable ();
		public int nodeCount = 0;
		public Node root;
		int timeLimit, timeSpent;
        int collisionCount = 0;
		bool graph;
		public bool firstRun = true;
		PNSearch second = null;

		public PNSearch (bool graph)
		{
			this.graph = graph;
		}

		public PNSearch (bool graph, int timeLimit)
		{
			this.graph = graph;
			this.timeLimit = timeLimit;
		}

		public void Prove (Game g)
		{
			root = new Node (g.startingPos, 1);
			root.Evaluate (g);
			transposition [root] = root;
			nodeCount++;
			Node lastExpanded = null;
			int lastpn = 0, lastdn = 0;

            int count = 0;

			Stopwatch sw = new Stopwatch ();
			sw.Start ();
			while (root.pn != 0 && root.dn != 0) {
				var mpn = MostProving (root);

				if (graph) {
					if (System.Object.ReferenceEquals (mpn, lastExpanded) && lastpn == mpn.pn && lastdn == mpn.dn) {
						sw.Stop ();
						timeSpent = (int)sw.ElapsedMilliseconds;
						throw new Exception ("Loop in finding most proving");
					}
					mpn.Expand (g, transposition);
					mpn.StartUpdate ();

					lastExpanded = mpn;
					lastpn = mpn.pn;
					lastdn = mpn.dn;
				} else {
					mpn.ExpandTree (g, ref  nodeCount, ref collisionCount, transposition);
					mpn.StartUpdateTree (firstRun);

                    if (count % 10000 == 0)
                    {
                        Console.WriteLine("PN:         {0}\nDN:         {1}", root.pn, root.dn);
                        Console.WriteLine("Collisions: " + collisionCount);
                        Console.WriteLine("Nodes:      " + nodeCount);
                    }

                    count++;
				}

				if (sw.ElapsedMilliseconds > timeLimit * 60000) {
					sw.Stop ();
					timeSpent = (int)sw.ElapsedMilliseconds;
					throw new Exception ("Time limit exceeded");
				}
			}

			sw.Stop ();
			timeSpent = (int)sw.ElapsedMilliseconds;

			// If this is the first run of a graph search where black loses, run again to check for draw
			if (!graph && firstRun && root.dn == 0) {
				second = new PNSearch (false, timeLimit);
				second.firstRun = false;
				second.Prove (g);
			}
		}

		public Node MostProving (Node v)
		{
			Node.InitiateVisiting ();
			while (v.children != null) {
				v = LeftmostNode (v);
				if (v.IsVisited ()) {
					//v.pn *= 2;
					//v.dn *= 2;
					break;
				}
			}
			return v;
		}

		public Node LeftmostNode (Node v)
		{
			v.SetVisit ();
			foreach (var child in v.children)
				if (v.c == 1) {
					if (child.pn == v.pn && !child.IsVisited ())
						return child;
				} else {
					if (child.dn == v.dn && !child.IsVisited ())
						return child;
				}
			return v;
		}

		public void SetTimeLimit (int minutes)
		{
			timeLimit = minutes;
		}

		public int Value ()
		{
			if (second == null)
				return root.pn == 0 ? 1 : root.dn == 0 ? -1 : 0;
			else
				return root.pn == 0 ? 1 : (root.dn == 0 && second.root.dn == 0) ? -1 : 0;
		}

		public int TimeSpent ()
		{
			if (second == null)
				return timeSpent;
			else
				return timeSpent + second.timeSpent;
		}

		public int NodeCount ()
		{
			if (graph)
			if (second == null)
				return transposition.Count;
			else
				return transposition.Count + second.transposition.Count;
			else
				return nodeCount;
		}

		public List<Bits[]> BestGame ()
		{
			var chain = root.GetLongestGame ();
			List<Bits[]> result = new List<Bits[]> ();
			foreach (var node in chain)
				result.Add (node.position);
			return result;
		}

		public String Name ()
		{
			if (graph)
				return "PNG";
			else
				return "PNT";
		}
	}
}

