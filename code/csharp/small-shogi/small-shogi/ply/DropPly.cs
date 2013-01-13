using System;

namespace smallshogi
{
    using Bits = System.UInt32;
    using B = BitBoard;

	public class DropPly : Ply
	{
		// Index of dropped piece
		int dI;
		// Bitboard specifying where the piece is dropped
		Bits location;
		public DropPly (int c, int dI, Bits location)
		{
			this.c = c;
			this.dI = dI;
			this.location = location;
		}

		public override Bits[] apply (Bits[] position)
		{
			var result = new Bits[position.Length];
			Array.Copy (position, result, position.Length);
			// Drop the piece in question
			var pI = pieceI  (c, dI);
			result [pI] ^= location;
			// Remove the piece from the player's hand
			Bits mask = Game.handMask[dI];
			var hI = handI (c);
			result[hI] = result[hI];
			B.PopMasked(ref result[hI], mask);

			return result;
		}

		public override int pieceMoved ()
		{
			return dI;
		}
	}
}