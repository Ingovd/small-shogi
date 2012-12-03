using System;
using System.Collections.Generic;

namespace smallshogi
{
	public class INode
	{
		BitBoard[] position;
		List<INode> children;
		public static Dictionary<int, BitBoard[]> transposition = new Dictionary<int, BitBoard[]> ();

		public INode (BitBoard[] position)
		{
			this.position = position;
			children = new List<INode> ();
		}

		public void Expand (Game g, int c)
		{
			transposition.Add (g.hashPosition (position), position);
			foreach (var p in g.children(position, c)) {
				var newPosition = p.apply (position);
				if (!transposition.ContainsKey (g.hashPosition (newPosition))) {
					var newINode = new INode (newPosition);
					children.Add (newINode);
					if (g.gamePosition (newPosition) < 0)
						newINode.Expand (g, c ^ 1);
				} else {
					var tabledPosition = transposition [g.hashPosition (newPosition)];
					if (g.SamePosition (tabledPosition, newPosition))
						System.Console.WriteLine ("Warning: we have a collision!");
				}
			}
		}

		public int Solve (Game g, int c)
		{
			int unit = c ^ 1;
			int pos = g.gamePosition (position);
			if (pos < 0) {
				if (c == 0) {
					foreach (var child in children)
						unit = unit & child.Solve (g, c ^ 1);
				} else {
					foreach (var child in children)
						unit = unit | child.Solve (g, c^1);
				}
				return unit;
			} else {
				if (pos == 1)
					return 0;
				else
					return 1;
			}
		}

		public void show (Game g)
		{
			if (position [2 * Game.l].bits > 3 || position [2 * Game.l + 1].bits > 3)
				Console.WriteLine (g.prettyPrint (position));
			foreach (var child in children)
				child.show (g);
		}

		public int Depth ()
		{
			var maxDepth = 0;
			foreach (var child in children)
				maxDepth = Math.Max(maxDepth, child.Depth());
			return maxDepth + 1;
		}

		public int Size ()
		{
			var size = 1;
			foreach (var child in children)
				size += child.Size ();
			return size;
		}
	}
}

