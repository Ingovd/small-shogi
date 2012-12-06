using System;
using System.Collections.Generic;

namespace smallshogi
{
	public class INode
	{
		public BitBoard[] position;
		public List<INode> children;
		public int c;
		public int value;
		public int pn;
		public static Dictionary<INode, INode> transposition = new Dictionary<INode, INode> ();
		public static Stack<INode> stackMothaFucka = new Stack<INode> ();
		public static Queue<INode> queue = new Queue<INode> ();

		public INode (BitBoard[] position, int c)
		{
			this.position = position;
			this.c = c;
			value = -1;
			pn = 1;
			children = new List<INode> ();
		}

		public static void ExpandRoot (INode root, Game g)
		{
			stackMothaFucka.Push (root);
			INode node = null;
			while ((node = stackMothaFucka.Pop ()) != null)
				node.Expand (g);
		}

		public static void BreadthFirst (INode root, Game g)
		{
			transposition.Add (root, root);
			queue.Enqueue (root);
			INode node = null;
			while ((node = queue.Dequeue ()) != null) {
				if(node.Equals(root)) {
					Console.WriteLine("Root get!");
					if(root.pn != 1)
						return;
				}
				node.Solve2 (g);
			}
		}

		public void Expand (Game g)
		{
			Console.WriteLine(g.prettyPrint(position));
			Console.WriteLine(stackMothaFucka.Count);
			// Generate and expand children
			foreach (var p in g.children(position, c)) {
				var newPosition = p.apply (position);
				var newINode = new INode (newPosition, c ^ 1);

				// Stop expanding if this position has already occured
				if (transposition.ContainsKey (newINode)) {
					var tabledINode = transposition [newINode];
					if (!tabledINode.position.Equals(newINode))
						System.Console.WriteLine ("Warning: we have a collision!");
					continue;
				}

				// New position, add to transposition, children and stack
				transposition.Add (newINode, newINode);
				children.Add (newINode);

				// Don't push if this position is terminal
				if (g.gamePosition (newINode.position, c ^ 1) >= 0)
					continue;
				stackMothaFucka.Push (newINode);
			}
		}

		public void Expand2 (Game g)
		{
			// Generate and expand children
			foreach (var p in g.children(position, c)) {
				var newPosition = p.apply (position);
				var newINode = new INode (newPosition, c ^ 1);
				children.Add (newINode);

				// Stop expanding if this position has already occured
				if (transposition.ContainsKey (newINode)) {
					/*INode tabledINode = null;
					transposition.TryGetValue(newINode, out tabledINode);
					if (!tabledINode.Equals(newINode))
						System.Console.WriteLine ("Warning: we have a collision!");*/
					continue;
				}

				// New position, add to transposition, children and stack
				transposition.Add (newINode, newINode);
			}
		}

		public int Solve (Game g, int c)
		{
			int unit = c ^ 1;
			int pos = g.gamePosition (position, c);
			if (pos < 0) {
				if (c == 0) {
					foreach (var child in children)
						unit = unit & child.Solve (g, c ^ 1);
				} else {
					foreach (var child in children)
						unit = unit | child.Solve (g, c ^ 1);
				}
				value = unit;
			} else {
				if (pos == 1)
					value = 0;
				else
					value = 1;
			}
			return value;
		}

		public int Solve2 (Game g)
		{
			Console.WriteLine (queue.Count);

			if (value < 0) {
				value = g.gamePosition (position, c);
				if (value >= 0) {
					pn = (value - 1) * Int32.MaxValue;
					return pn;
				}
			}

			if (children.Count == 0)
				Expand2(g);


			// Black is maximising (1) white is minimising (-1)
			int sign = (2 * c) - 1;
			int e = Int32.MaxValue * (c ^ 1);
			foreach (var child in children) {
				var tchild = transposition[child];
				//Console.WriteLine(tchild.pn);
				e = sign * Math.Max (sign * tchild.pn, sign * e);
			}
			if (e == Int32.MaxValue * c) {
				pn = e;
				Console.WriteLine(pn);
				return pn;
			}
			foreach (var child in children) {
				var tchild = transposition[child];
				if (tchild.pn == 1) {
					queue.Enqueue(tchild);
				}
			}
			queue.Enqueue(this);
			return pn;
		}

		public void show (Game g)
		{
			if (position [2 * Game.l].bits > 3 || position [2 * Game.l + 1].bits > 3)
				Console.WriteLine (g.prettyPrint (position));
			foreach (var child in children)
				child.show (g);
		}

		public int Height ()
		{
			var maxHeight = 0;
			foreach (var child in children)
				maxHeight = Math.Max (maxHeight, child.Height ());
			return maxHeight + 1;
		}

		public int Size ()
		{
			var size = 1;
			foreach (var child in children)
				size += child.Size ();
			return size;
		}

		public override int GetHashCode ()
		{
			var hash = 982451653;
			foreach (BitBoard b in position)
				hash = 997 * hash + (int)b.bits;
			hash ^= c*2147483647;
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

		public bool Equals (INode other) {
			if(c != other.c)
				return false;
			if(position.Length != other.position.Length)
				return false;
			for (int i = 0; i < position.Length; ++i)
				if(!position[i].Equals(other.position[i]))
					return false;
			return true;
		}
	}
}

