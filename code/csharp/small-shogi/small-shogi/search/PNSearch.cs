using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	public class PNSearch
	{
		static Func<Node, int> PN = n => n.pn;
		static Func<Node, int> DN = n => n.dn;

		public PNSearch ()
		{
		}

		public int Search (Node root, Game g)
		{
			root.Evaluate(g);
			int count = 0;
			while (root.pn != 0 && root.dn != 0) {
				var mpn = MostProving(root);
                Node.InitiateVisiting();
                mpn.Update(null);
				//if(root.DetectDraw())
				//	break;
				if(count % 100 == 0) {
					Node.InitiateVisiting ();
					int size = root.Size();
					Console.WriteLine(root.pn + " and " + root.dn);
					Console.WriteLine("Transposition: " + Node.transposition.Count);
					//Console.WriteLine("Count:         " + size);
					//Console.WriteLine("Draw:          " + (root.DetectDraw() ? "True" : "False"));
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
			while (v.children != null && !v.IsVisited ())
				if (v.c == 1)
					v = LeftmostNode (v, PN);
				else
					v = LeftmostNode (v, DN);
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

