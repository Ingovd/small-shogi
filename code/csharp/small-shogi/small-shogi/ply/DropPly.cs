using System;

namespace smallshogi
{
    using Bits = System.UInt32;
    using B = BitBoard;

	public class DropPly : Ply
	{
		// Index of dropped piece unadjusted for colour
		int dropIndex;
		// Bitboard specifying where the piece is dropped
		Bits location;
		public DropPly (int c, int dropIndex, Bits location)
		{
			this.c = c;
			this.dropIndex = dropIndex;
			this.location = location;
		}


		public override Bits[] Apply (Bits[] position, Game g)
		{
			//Create a new set of bits
			var result = new Bits[position.Length];
			Array.Copy (position, result, position.Length);

			// Drop the piece in question by setting the location bit
			result [g.PieceIndex  (c, dropIndex)] |= location;

			// Remove the piece from the player's hand
			Bits mask = g.handMask[dropIndex];
			var handIndex = g.HandIndex (c);
			result[handIndex] = result[handIndex];
			B.PopMasked(ref result[handIndex], mask);

			return result;
		}

		public override int PieceMoved ()
		{
			return dropIndex;
		}
	}
}