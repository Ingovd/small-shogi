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

		public override void apply (BitBoard[] position)
		{
			// Drop the piece in question
			position [pieceI  (c, dI)].And (location);
			// Remove the piece from the player's hand
			var mask = new BitBoard(Game.handMask[dI]);
			position[handI(c)].PopMasked(mask);
		}
	}
}