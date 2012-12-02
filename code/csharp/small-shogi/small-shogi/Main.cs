using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            System.Console.BufferHeight = 500;
			var ul = new Move (-1, -1);
			var u = new Move (0, -1);
			var ur = new Move (1, -1);
			var l = new Move (-1, 0);
			var r = new Move (1, 0);
			var dl = new Move (-1, 1);
			var d = new Move (0, 1);
			var dr = new Move (1, 1);
			Move[] kingArray = {ul, u, ur, l, r, dl, d, dr};
			Move[] bishopArray = {ul, ur, dl, dr};
			Move[] rookArray = {u, l, r, d};
			Move[] pawnArray = {d};
			Move[] tokinArray = {ul, u, ur, l, r, d};
			var kingMoves = new List<Move> (kingArray);
			var bishopMoves = new List<Move> (bishopArray);
			var rookMoves = new List<Move> (rookArray);
			var pawnMoves = new List<Move> (pawnArray);
			var tokinMoves = new List<Move> (tokinArray);

			var king = new Piece (kingMoves, Type.King);
			var bishop = new Piece (bishopMoves, Type.Bishop);
			var rook = new Piece (rookMoves, Type.Rook);
			var pawn = new Piece (pawnMoves, Type.Pawn, tokinMoves, Type.Tokin);
			Piece[] pieces = { king, bishop, rook, pawn };

			var white = new Dictionary<int, Type> ();
			var black = new Dictionary<int, Type> ();
			white [ 0] = Type.Bishop;
			white [ 1] = Type.King;
			white [ 2] = Type.Rook;
			white [ 4] = Type.Pawn;
			black [ 9] = Type.Rook;
			black [10] = Type.King;
			black [11] = Type.Bishop;
			black [ 7] = Type.Pawn;

			Game g = new Game (white, black, 4, 3, 1, pieces);

			/*Console.WriteLine(g.prettyPrint(g.startingPos));
			foreach (var p in g.children (g.startingPos, 1)) {
				Console.WriteLine ();
				Console.WriteLine(g.prettyPrint(p.apply (g.startingPos)));
			}*/

			BitBoard[] position = g.startingPos;
			int c = 0;
			for(int i = 0; i < 15; ++i) {
				var plies = g.children (position, c);
                var player = c == 0 ? "White" : "Black";
                System.Console.WriteLine(player + " has " + plies.Count + " moves.");
                Ply last = null;
				foreach (var p in plies) {
					int j = p.pieceMoved();
					var s = Piece.showType (j < pieces.Length ? pieces [j].type : pieces [Game.demote [j]].ptype);
					Console.WriteLine ("Moving " + s);
                    System.Console.WriteLine(g.prettyPrint(p.apply(position)));
                    last = p;
				}
                if(last != null)
                    position = last.apply(position);
				c ^= 1;
			}

			/*
			var a = new BitBoard();
			var b = new BitBoard();
			var source = new BitBoard[2];
			source[0] = a; source[1] = b;
			a.Set (2);
			b.Set (3);
			Console.WriteLine(a.ToString(3, 12));
			Console.WriteLine(b.ToString(3, 12));

			var copy = new BitBoard[2];
			Array.Copy (source, copy, 2);
			a.Set (4);
			Console.WriteLine(copy[0].ToString(3, 12));*/

            Console.Read();
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
