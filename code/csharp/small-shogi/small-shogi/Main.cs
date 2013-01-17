using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using smallshogi.search;

namespace smallshogi
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//Game g = new Game (white, black, 4, 3, 1, pieces);
			/*white [ 0] = Type.Bishop;
			white [ 1] = Type.King;
			white [ 2] = Type.Rook;
			//white [ 4] = Type.Pawn;
			black [ 9] = Type.Rook;
			black [10] = Type.King;
			black [11] = Type.Bishop;
			//black [ 7] = Type.Pawn;*/

			/*GameSetup setup = new GameSetup (3, 3);
			setup.SetPromotionRanks (1);
			setup.AddWhitePiece (0, 0, Type.King);
			setup.AddWhitePiece (1, 0, Type.Bishop);
			setup.AddWhitePiece (2, 0, Type.Rook);
			setup.AddBlackPiece (0, 2, Type.Rook);
			setup.AddBlackPiece (1, 2, Type.Bishop);
			setup.AddBlackPiece (2, 2, Type.King);
			Game g = new Game (setup);*/
			
			//GameSetup setup =  new GameSetup(5, 1);
			//setup.SetPromotionRanks(1);
			//setup.AddWhitePiece(0,0, Type.King);
			//setup.AddBlackPiece(5,0, Type.King);
			//Game g = new Game (setup);

			int start = 4;

			Game g = new Game(new GameSetup(start));
			var root = new Node (g.startingPos, 1);
			PNSearch pnSearch = new PNSearch ();
			Stopwatch sw = new Stopwatch ();
			sw.Start ();
			pnSearch.Search (root, g);
			sw.Stop ();
			System.Console.WriteLine ("Done expanding in: " + sw.ElapsedMilliseconds + " milliseconds.");
			System.Console.WriteLine ("Number of nodes:   " + Node.transposition.Count);
			//System.Console.WriteLine("Tree depth:        " + root.Height());
			System.Console.WriteLine ("Game value:        " + (root.pn==0?1:-1));
			var bestGame = root.GetLongestGame(g);
					foreach(var node in bestGame)
						Console.WriteLine(g.prettyPrint(node.position));

			/*var root = new BNode (g.startingPos, 1);
			BNode.Prove (root, g);
			//root.Browse(g);
			var bestGame = root.DFSearch(g, root.value);
			foreach(var node in bestGame)
				Console.WriteLine(g.prettyPrint(node.position));
			root.Browse (g);*/

			GameSetup setup;
			Game g2;
			BNode root2;
			int[] wins = new int[3];
			for (int i = start; i < 1372; i++) {
				setup = new GameSetup (i);
				g2 = new Game(setup);
				root2 = new BNode (g2.startingPos, 1);
				if(g2.gamePosition(root2.position, 1) < 0) {
					BNode.Prove (root2, g2);
                    //Console.WriteLine(root2.WinningStrategySize());
					//root2.Browse	(g2);
                    var bestGame2 = root2.GetLongestGame(g2);
					foreach(var node in bestGame2)
						Console.WriteLine(g2.prettyPrint(node.position));
					wins[root2.value+1]++;
					Console.WriteLine("Black: {0}\nWhite: {1}\nNone:  {2}",wins[2], wins[0], wins[1]);
					BNode.Reset();
				}
			}
		}
	}
}
