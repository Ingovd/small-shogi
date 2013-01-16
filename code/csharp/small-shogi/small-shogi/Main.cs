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

			/*var root = new Node (g.startingPos, 1);
			PNSearch pnSearch = new PNSearch ();
			Stopwatch sw = new Stopwatch ();
			sw.Start ();
			pnSearch.Search (root, g);
			sw.Stop ();
			System.Console.WriteLine ("Done expanding in: " + sw.ElapsedMilliseconds + " milliseconds.");
			System.Console.WriteLine ("Number of nodes:   " + root.Size ());
			//System.Console.WriteLine("Tree depth:        " + root.Height());
			System.Console.WriteLine ("Game value:        " + root.pn);*/

			/*var root = new BNode (g.startingPos, 1);
			BNode.Prove (root, g);
			//root.Browse(g);
			var bestGame = root.DFSearch(g, root.value);
			foreach(var node in bestGame)
				Console.WriteLine(g.prettyPrint(node.position));
			root.Browse (g);*/

			GameSetup setup;
			Game g;
			BNode root;
			for (int i = 0; i < 1372; i++) {
				setup = new GameSetup (i);
				g = new Game(setup);
				root = new BNode (g.startingPos, 1);
				if(g.gamePosition(root.position, 1) < 0) {
					BNode.Prove (root, g);
					var bestGame = root.DFSearch(g, root.value);
					foreach(var node in bestGame)
						Console.WriteLine(g.prettyPrint(node.position));
				}
			}
		}

        static void DisplayBitBoard(BitBoard BitBoard)
        {
            for (int i = 0; i < 32; i++)
            {
				if(i % 3 == 0 && i != 0)
					Console.WriteLine();
                bool bit = BitBoard.Get(i);
                Console.Write(bit ? 1 : 0);
            }
            Console.WriteLine();
        }
	}
}
