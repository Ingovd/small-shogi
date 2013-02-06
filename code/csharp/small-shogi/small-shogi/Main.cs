using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using smallshogi.search;

namespace smallshogi
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			GameSetup setup = new GameSetup (4, 3);
			setup.SetPromotionRanks (1);
			//setup.AddWhitePiece (0, 0, Type.Bishop);
			setup.AddWhitePiece (1, 0, Type.King);
			setup.AddWhitePiece (2, 0, Type.Rook);
			//setup.AddWhitePiece (1, 1, Type.Pawn);
			//setup.AddBlackPiece (2, 3, Type.Bishop);
			setup.AddBlackPiece (1, 3, Type.King);
			setup.AddBlackPiece (0, 3, Type.Rook);
			//setup.AddBlackPiece (1, 2, Type.Pawn);
			Game g = new Game(setup);
			Console.WriteLine(g.prettyPrint(g.startingPos));
			var pn = new PNSearch (false, 15);
			pn.Prove (g);
			var bestgame = pn.BestGame ();
            foreach (var pos in bestgame)
            {
                Console.WriteLine(g.prettyPrint(pos));
            }
		}
	}
}
