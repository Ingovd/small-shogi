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
			white [ 4] = Type.Pawn;
			black [ 9] = Type.Rook;
			black [10] = Type.King;
			black [11] = Type.Bishop;
			black [ 7] = Type.Pawn;
            /*white[0] = Type.King;
            white[1] = Type.Gold;
            white[2] = Type.Silver;
            white[3] = Type.Bishop;
            white[4] = Type.Rook;
            white[5] = Type.Pawn;
            black[24] = Type.King;
            black[23] = Type.Gold;
            black[22] = Type.Silver;
            black[21] = Type.Bishop;
            black[20] = Type.Rook;
            black[19] = Type.Pawn;*/


			
			Game g = new Game (white, black, 4, 3, 1, pieces);
            //Game g = new Game(white, black, 5, 5, 1, pieces);

			/*BitBoard[] position = g.startingPos;
			int c = 0;
			for(int i = 0; i < 10; ++i) {
				var plies = g.children (position, c);
                var player = c == 0 ? "White" : "Black";
                System.Console.WriteLine(player + " has " + plies.Count + " moves.");
                BitBoard[] last = null;
                Ply lastPly = null;
				foreach (var p in plies) {
                    var newPos = p.apply(position);
                    if (g.gamePosition(newPos) < 0)
                    {
                        last = newPos;
                        lastPly = p;
                    }
				}
                int j = lastPly.pieceMoved();
                var s = Piece.showType(j < pieces.Length ? pieces[j].type : pieces[Game.demote[j]].ptype);
                Console.WriteLine("Moving " + s);
                System.Console.WriteLine(g.prettyPrint(last));
                position = last;
				c ^= 1;
			}*/

			/*var search = new AndOrSearch(g);
			var sequence = search.Start();
			string s = sequence.won == 1 ? "Black" : "White";
			System.Console.WriteLine(s + " won after " + sequence.depth + " plies.");
			sequence.show(g);*/

			var root = new INode(g.startingPos, 1);
			Stopwatch sw = new Stopwatch();
			sw.Start();
			INode.BreadthFirst(root, g);
			sw.Stop();
			System.Console.WriteLine("Done expanding in: " + sw.ElapsedMilliseconds + " milliseconds.");
			System.Console.WriteLine("Number of nodes:   " + root.Size());
			System.Console.WriteLine("Tree depth:        " + root.Height());
			System.Console.WriteLine("Game value:        " + root.Solve (g, 1));

            foreach (var child in root.children)
            {
                System.Console.WriteLine(g.prettyPrint(child.position));
                System.Console.WriteLine("Game value: " + child.value);
            }


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
