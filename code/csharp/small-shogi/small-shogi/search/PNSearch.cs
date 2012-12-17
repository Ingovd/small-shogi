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
				Node.InitiateVisiting ();
				var mpn = MostProving(root);
				mpn.Expand (g);
				Node.InitiateVisiting ();
				mpn.StartUpdate ();
				if(count % 10000 == 0) {
					int size = root.Size();
					Console.WriteLine(root.pn + " and " + root.dn);
					Console.WriteLine("Transposition: " + Node.transposition.Count);
					Console.WriteLine("Count:         " + size);
				}
 				count++;
			}
			Console.WriteLine(root.pn + " and " + root.dn);
			return root.pn;
		}

		public Node MostProving (Node v)
		{
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

	public class Node
	{
		public BitBoard[] position;
		public List<Node> children;
		public List<Node> parents = new List<Node> ();
		public byte c;
		public int pn, dn, marker;
		static int visitMarker;

		public static Hashtable transposition = new Hashtable ();

		public Node (BitBoard[] position, byte colour)
		{
			this.position = position;
			this.c = colour;
		}

		public Node (Ply p, Node parent, byte colour)
		{
			this.position = p.apply (parent.position);
			c = colour;
		}

		public static void InitiateVisiting ()
		{
			visitMarker++;
		}

		public void SetVisit ()
		{
			this.marker = Node.visitMarker;
		}

		public bool IsVisited ()
		{
			return this.marker == Node.visitMarker;
		}

		public void Draw ()
		{
			pn = Int32.MaxValue;
			dn = 0;
			Node.InitiateVisiting ();
			children = null;
			foreach (var parent in parents)
				parent.Update (null);
		}

		public void StartUpdate ()
		{
			UpdateNumbers ();

			// Update parents
			foreach (var parent in parents)
				parent.Update (this);
		}

		public void Update (Node origin)
		{
			// Tree test, draw detected if true
			if (origin != null && this.Equals (origin)) {
				origin.Draw ();
				return;
			}

			// DAG/DCG test, dont visit a node more than once
			if(IsVisited ())
				return;
			SetVisit ();

			UpdateNumbers ();

			// Clear some memory maybe
			//if(pn == 0 || dn == 0)
			//	children = null;

			// Update parents
			foreach (var parent in parents)
				parent.Update (origin);
		}

		public void UpdateNumbers ()
		{
			if (c == 1) {
				int min = Int32.MaxValue;
				int sum = 0;
				foreach (var child in children) {
					min = Math.Min (child.pn, min);
					if(sum + child.dn >= sum)
						sum += child.dn;
					else
						sum = Int32.MaxValue;
				}
				pn = min;
				dn = sum;
			} else {
				int min = Int32.MaxValue;
				int sum = 0;
				foreach (var child in children) {
					min = Math.Min (child.dn, min);
					if(sum + child.pn >= sum)
						sum += child.pn;
					else
						sum = Int32.MaxValue;
				}
				dn = min;
				pn = sum;
			}
		}

		public void Evaluate (Game g)
		{
			int pos = g.gamePosition (position, c);
			if (pos > 0) {
				pn = -1*(pos - 2) * Int32.MaxValue;
				dn =    (pos - 1) * Int32.MaxValue;
			} else {
				pn = 1;
				dn = 1;
			}
		}

		public void Expand (Game g)
		{
			//if(children != null)
			//	return;
			children = new List<Node> ();
			var plies = g.children (position, c);
			foreach (var ply in plies) {
				var s = new Node (ply, this, (byte)(c ^ 1));
				s.Evaluate(g);
				if(!transposition.ContainsKey(s))
					transposition[s] = s;
				/*if(transposition.ContainsKey(s))
					s = (Node) transposition[s];
				else {
					s.Evaluate (g);
					transposition[s] = s;
				}*/
				s.parents.Add(this);
				children.Add (s);
			}
		}

		public int Size ()
		{
			var size = 1;
			if(children != null)
				foreach(var child in children)
					size += child.Size ();
			return size;
		}

		public void Show (Game g)
		{
			Console.WriteLine(g.prettyPrint(position));
			if(children != null)
				foreach(var child in children)
					child.Show(g);
		}

		public override int GetHashCode ()
		{
			var hash = 982451653;
			foreach (BitBoard b in position)
				hash = 997 * hash + (int)b.bits;
			hash ^= c * 2147483647;
			return hash;
		}

		public override bool Equals (Object obj)
		{
			if (obj == null)
				return false;
			//if(System.Object.ReferenceEquals(this, obj))
			//   return true;
			Node node = obj as Node;
			if (node == null)
				return false;
			return Equals (node);
		}

		public bool Equals (Node other)
		{
			if (c != other.c)
				return false;
			if (position.Length != other.position.Length)
				return false;
			for (int i = 0; i < position.Length; ++i)
				if (!position [i].Equals (other.position [i]))
					return false;
			return true;
		}
	}
}

