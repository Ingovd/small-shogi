using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace smallshogi
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//System.Console.BufferHeight = 500;
			// Define moves
			var ul = new Move (-1, -1);
			var u = new Move (0, -1);
			var ur = new Move (1, -1);
			var l = new Move (-1, 0);
			var r = new Move (1, 0);
			var dl = new Move (-1, 1);
			var d = new Move (0, 1);
			var dr = new Move (1, 1);
			//var nl = new Move(-1, -2);
			//var nr = new Move(1, -2);
			// Define moves per piece
			Move[] kingArray = {ul, u, ur, l, r, dl, d, dr};
			Move[] bishopArray = {ul, ur, dl, dr};
			Move[] rookArray = {u, l, r, d};
			Move[] pawnArray = {d};
			Move[] tokinArray = {ul, u, ur, l, r, d};
			// Move[] silverArray = {ul, u, ur, dr, dl};
			//Move[] knightArray = { nr, nl };
			var kingMoves = new List<Move> (kingArray);
			var bishopMoves = new List<Move> (bishopArray);
			var rookMoves = new List<Move> (rookArray);
			var pawnMoves = new List<Move> (pawnArray);
			var tokinMoves = new List<Move> (tokinArray);
			// var silverMoves = new List<Move>(silverArray);
			//var knightMoves = new List<Move>(knightArray);

			// Instantiate the Piece objects
			var king = new Piece (kingMoves, Type.King);
			var bishop = new Piece (bishopMoves, Type.Bishop);
			var rook = new Piece (rookMoves, Type.Rook);
			var pawn = new Piece (pawnMoves, Type.Pawn, tokinMoves, Type.Tokin);
			//var silver = new Piece(silverMoves, Type.Silver, tokinMoves, Type.PSilver);
			//var knight = new Piece(knightMoves, Type.Knight, tokinMoves, Type.PKnight);
			//var gold = new Piece(tokinMoves, Type.Gold);
			Piece[] pieces = { king, bishop, rook, pawn };

			// Set up the initial board configuration
			var white = new Dictionary<int, Type> ();
			var black = new Dictionary<int, Type> ();
			white [ 0] = Type.Bishop;
			white [ 1] = Type.King;
			white [ 2] = Type.Rook;
			//white [ 4] = Type.Pawn;
			black [ 9] = Type.Rook;
			black [10] = Type.King;
			black [11] = Type.Bishop;
			//black [ 7] = Type.Pawn;*/
			/*white[0] = Type.King;
            white[1] = Type.Bishop;
			white[2] = Type.Rook;
            black[8] = Type.King;
            black[7] = Type.Bishop;
			black[6] = Type.Rook;*/
			//white [0] = Type.King;
			//black [4] = Type.King;

			
			Game g = new Game (white, black, 4, 3, 1, pieces);
			//Game g = new Game (white, black, 3, 3, 1, pieces);
			//Game g = new Game (white, black, 5, 1, 1, pieces);

			var root = new Node (g.startingPos, 1);
			PNSearch pnSearch = new PNSearch ();
			Stopwatch sw = new Stopwatch ();
			sw.Start ();
			pnSearch.Search (root, g);
			sw.Stop ();
			System.Console.WriteLine ("Done expanding in: " + sw.ElapsedMilliseconds + " milliseconds.");
			System.Console.WriteLine ("Number of nodes:   " + root.Size ());
			//System.Console.WriteLine("Tree depth:        " + root.Height());
			System.Console.WriteLine ("Game value:        " + root.pn);

			Node end, temp;
			while ((end = root.GetCycle ()) != null) {
				Console.WriteLine("Start of a cycle");
				while (end != null) {
					Console.WriteLine(g.prettyPrint(end.position));
					temp = end.previous;
					end.previous = null;
					end = temp;
				}
				Console.WriteLine("End of a cycle");
			}

			//root.Show(g);

            //Console.Read();
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
