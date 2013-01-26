using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	public class PNSearch
	{
		static Func<Node, int> PN = n => n.pn;
		static Func<Node, int> DN = n => n.dn;

		public Hashtable transposition = new Hashtable();
		public int nodeCount = 0;

		public Node root;

		public PNSearch ()
		{
		}

		public int Prove (Game g)
		{
			root = new Node(g.startingPos, 1);
			root.Evaluate(g);
			int count = 0;
			while (root.pn != 0 && root.dn != 0) {
				var mpn = MostProving(root);
                Node.InitiateVisiting();
                mpn.Expand(g, transposition);
				Node.InitiateVisiting();
				mpn.Update ();
				if(count % 100 == 0) {
					Node.InitiateVisiting ();
					Console.WriteLine(root.pn + " and " + root.dn);
					Console.WriteLine("Transposition: " + transposition.Count);
				}
				if(count >= 1000000)
					break;
 				count++;
			}
			Console.WriteLine(root.pn + " and " + root.dn);
			return root.pn;
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
	}
}

