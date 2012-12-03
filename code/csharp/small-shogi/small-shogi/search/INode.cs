using System;
using System.Collections.Generic;

namespace smallshogi
{
	public class INode
	{
		public BitBoard[] position;
		public List<INode> children;
        public int value;
		public static Dictionary<int, INode> transposition = new Dictionary<int, INode> ();

		public INode (BitBoard[] position)
		{
			this.position = position;
			children = new List<INode> ();
		}

		public void Expand (Game g, int c)
		{
			transposition.Add (g.hashPosition (position), this);
			foreach (var p in g.children(position, c)) {
				var newPosition = p.apply (position);
                //Console.WriteLine(g.prettyPrint(newPosition));
                var newINode = new INode(newPosition);
                children.Add(newINode);
				if (!transposition.ContainsKey (g.hashPosition (newPosition))) {
					if (g.gamePosition (newPosition) < 0)
                        newINode.Expand(g, c ^ 1);
				} else {
					var tabledPosition = transposition [g.hashPosition (newPosition)];
					if (g.SamePosition (tabledPosition.position, newPosition))
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
				maxHeight = Math.Max(maxHeight, child.Height());
			return maxHeight + 1;
		}

		public int Size ()
		{
			var size = 1;
			foreach (var child in children)
				size += child.Size ();
			return size;
		}

        public override int GetHashCode()
        {
            var hash = 982451653;
            foreach (BitBoard b in position)
                hash = 31 * hash + (int)b.bits;
            return hash;
        }
	}
}

