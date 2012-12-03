using System;
using System.Collections.Generic;

namespace smallshogi
{
	public class Node
	{
		public BitBoard[] position;
		public int depth;
		public int won;
		public Node child;

		public Node (BitBoard[] position, Node child, int won, int depth)
		{
			this.position = position;
			this.child = child;
			this.won = won;
			this.depth = depth;
		}

		public void show (Game g)
		{
			Console.WriteLine(g.prettyPrint(position));
			if(depth > 0)
				child.show (g);
		}
	}
}