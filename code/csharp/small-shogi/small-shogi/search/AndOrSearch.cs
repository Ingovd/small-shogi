using System;
using System.Collections.Generic;

namespace smallshogi
{
	public class AndOrSearch
	{
		Game g;
		Dictionary<int, BitBoard[]> transposition;
		int totalPlies = 0;

		public AndOrSearch (Game g)
		{
			this.g = g;
			transposition = new Dictionary<int, BitBoard[]> ();
		}

		public Node Start ()
		{
			return Solve (g.startingPos, 1);
		}

		// c=1 Or, c=0 And
		public Node Solve (BitBoard[] position, int c)
		{
			totalPlies++;
			Console.WriteLine (totalPlies);

			var gamePosition = g.gamePosition (position);
			if (gamePosition < 0) {
				var plies = g.children (position, c);
				Func<int, int, int> combine;
				Func<int, int, bool> minmax;
				int minMaxDepth;
				int e;
				if (c == 0) {
					combine = (x, y) => x & y;
					minmax = (x, y) => x <= y;
					minMaxDepth = 0;
					e = 1;
				} else {
					combine = (x, y) => x | y;
					minmax = (x, y) => y <= x;
					minMaxDepth = Int32.MaxValue;
					e = 0;
				}

				var hash = g.hashPosition (position);
				if (transposition.ContainsKey (hash)) {
					if (g.SamePosition (transposition [hash], position))
						System.Console.WriteLine ("Warning: we have a collision!");
					return new Node (position, null, e, 0);
				}
				transposition.Add (hash, position);

				var result = e;
				Node minMaxMove = null;

				foreach (var p in plies) {
					var newPosition = p.apply (position);
					// Recursive call
					var plyResult = Solve (newPosition, c ^ 1);
					if (minmax (minMaxDepth, plyResult.depth)) {
						minMaxDepth = plyResult.depth;
						minMaxMove = plyResult;
					}
					result = combine (result, plyResult.won);
					if (result != e) {
						return new Node (position, minMaxMove, result, minMaxDepth + 1);
					}
				}
				//System.Console.WriteLine(g.prettyPrint(position));
				return new Node (position, minMaxMove, result, minMaxDepth + 1);
			} else {
				if (gamePosition == 1)
					return new Node (position, null, 0, 0);
				else
					return new Node (position, null, 1, 0);
			}
		}
	}
}

