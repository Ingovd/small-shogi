using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			var ul = new Move(-1, -1);
			var u  = new Move( 0, -1);
			var ur = new Move( 1, -1);
			var l  = new Move(-1,  0);
			var r  = new Move( 1,  0);
			var dl = new Move(-1,  1);
			var d  = new Move( 0,  1);
			var dr = new Move( 1,  1);
			Move[] kingArray = {ul, u, ur, l, r, dl, d, dr};
			Move[] bishopArray = {ul, ur, dl, dr};
			Move[] rookArray = {u, l, r, d};
			var kingMoves   = new List<Move>(kingArray);
			var bishopMoves = new List<Move>(bishopArray);
			var rookMoves   = new List<Move>(rookArray);

            var king   = new Piece(kingMoves, Type.King);
			var bishop = new Piece(bishopMoves, Type.Bishop);
			var rook   = new Piece(rookMoves, Type.Rook);
            Piece[] pieces = { king, bishop, rook };

			var white = new Dictionary<int, Type>();
			var black = new Dictionary<int, Type>();
			white[ 0] = Type.Bishop;
			white[ 1] = Type.King;
			white[ 2] = Type.Rook;
			black[ 9] = Type.Rook;
			black[10] = Type.King;
			black[11] = Type.Bishop;

            Game g = new Game(white, black, 4, 3, pieces);

            foreach (var b in g.children (g.startingPos, 0))
				Console.WriteLine(b.toString(3, 12));

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
