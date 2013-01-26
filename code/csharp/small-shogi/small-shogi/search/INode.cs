using System;
using System.Collections.Generic;

namespace smallshogi
{
	public class INode
	{
		// Board position
		public BitBoard[] position;
		// Children nodes
		public List<INode> children;
		// Player to move
		public int c;
		// Game value
		public int value;
		// Proof number
		public int pn;
		// Flag if this node is already queued
		public bool queued;
		public static Dictionary<INode, INode> transposition = new Dictionary<INode, INode> ();
		public static Stack<INode> stackMothaFucka = new Stack<INode> ();
		public static Queue<INode> queue = new Queue<INode> ();

		static int da, ea;

		public INode (BitBoard[] position, int c)
		{
			this.position = position;
			this.c = c;

			// Default game value is -2: unknown
			value = -2;
			// Default proof number is nonzero en smaller than Maxvalue
			pn = 1;
			children = null;
		}

		public static void BreadthFirst (INode root, Game g)
		{
			transposition.Add (root, root);
			root.Enqueue ();
			INode node = null;
			while (queue.Count != 0) {
				node = Dequeue ();
				if (node.Equals (root)) {
					Console.WriteLine(transposition.Count);
					if (root.pn != 1)
						return;
				}
				if (transposition.Count > 100000)
					return;
				if (node.pn == 1)
					node.Solve (g);
			}
		}

		public static INode Dequeue ()
		{
			da++;
			var node = queue.Dequeue ();
			node.queued = false;
			return node;
		}

		public void Enqueue ()
		{
			ea++;
			if (!queued) 
				queued = true;
				queue.Enqueue (this);
			
		}
		
		public void Expand (Game g)
		{
			//Console.WriteLine(g.prettyPrint(position));
			// Generate and expand children
			foreach (var p in g.children(position, c)) {
				var newPosition = p.apply (position);
				var newINode = new INode (newPosition, c ^ 1);

				INode tabled = null;
				if (transposition.TryGetValue (newINode, out tabled)) {
					children.Add (tabled);
				} else {
					newINode.value = g.gamePosition (newINode.position, c ^ 1);
					children.Add (newINode);
					transposition.Add (newINode, newINode);
				}
			}
		}

		public int Solve (Game g)
		{
			//Console.WriteLine (transposition.Count);
			//Console.WriteLine (queue.Count);
			// If this node is a terminal position return
			if (value >= 0) {
				pn = (value - 1) * Int32.MaxValue;
				//Console.WriteLine("Terminal position with " + (c == 0 ? "white" : "black") + " to move: " + pn);
				//Console.WriteLine(g.prettyPrint(position));
				return pn;
			}
			// If the children haven't been calculated yet expand this node
			if (children == null) {
				this.children = new List<INode> ();
				Expand (g);
			}

			// Black is maximising (1) white is minimising (-1)
			int sign = (2 * c) - 1;
			int e = Int32.MaxValue * (c ^ 1);
			//Console.WriteLine("Current e: " + e);
			foreach (var child in children) {
				//Console.WriteLine("Child pn:  " + child.pn);
				e = sign * Math.Max (sign * child.pn, sign * e);
				//Console.WriteLine("New e:     " + e);
			}
			if (e != 1) {
				foreach (var child in children)
					transposition.Remove(child);
				children = null;
				pn = e;
				return pn;
			}

			// Enqueue unsolved children node for solving
			foreach (var child in children) {
				if (child.pn == 1) {
					child.Enqueue ();
				}
			}

			// Enqeueu this node for solving
			this.Enqueue ();
			return pn;
		}

		public int Height ()
		{
			var maxHeight = 0;
			if (children != null)
				foreach (var child in children)
					maxHeight = Math.Max (maxHeight, child.Height ());
			return maxHeight + 1;
		}

		public int Size ()
		{
			var size = 1;
			if (children != null)
				foreach (var child in children)
					size += child.Size ();
			return size;
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
			INode node = obj as INode;
			if (node == null)
				return false;
			return Equals (node);
		}

		public bool Equals (INode other)
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

