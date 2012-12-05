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
		public static Dictionary<INode, INode> transposition = new Dictionary<INode, INode> ();
		public static Stack<INode> stackMothaFucka = new Stack<INode> ();

		public INode (BitBoard[] position, int c)
		{
			this.position = position;
			this.c = c;
			children = new List<INode> ();
		}

		public static void ExpandRoot (INode root, Game g)
		{
			stackMothaFucka.Push (root);
			INode node = null;
			while ((node = stackMothaFucka.Pop ()) != null)
				node.Expand (g);
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

