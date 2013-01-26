using System;

namespace smallshogi
{
	public class DropPly : Ply
	{
		// Index of dropped piece
		int dI;
		// Bitboard specifying where the piece is dropped
		BitBoard location;
		public DropPly (int c, int dI, BitBoard location)
		{
			this.c = c;
			this.dI = dI;
			this.location = location;
		}

		public override BitBoard[] apply (BitBoard[] position)
		{
			var result = new BitBoard[position.Length];
			Array.Copy (position, result, position.Length);
			// Drop the piece in question
			var pI = pieceI  (c, dI);
			result [pI] = new BitBoard(result [pI]);
			result [pI].Xor (location);
			// Remove the piece from the player's hand
			var mask = new BitBoard(Game.handMask[dI]);
			var hI = handI (c);
			result[hI] = new BitBoard(result[hI]);
			result[hI].PopMasked(mask);

			return result;
		}

		public override int pieceMoved ()
		{
			return dI;
		}
	}
}