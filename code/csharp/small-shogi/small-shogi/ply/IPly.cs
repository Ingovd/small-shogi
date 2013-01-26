using System;

namespace smallshogi
{
	public abstract class Ply
	{
		// Colour of moving player
		protected int c;

		public abstract BitBoard[] apply (BitBoard[] position);

		public abstract int pieceMoved();

		// Index for hand bitboard
		public int handI (int c)
		{
			return 2 * Game.l + c;
		}

		// Index for piece bitboard
		public int pieceI (int c, int i)
		{
			return i + c * Game.l;
		}

		// Index for promoted piece bitboard
		public int piecePI (int c, int i) {
			return Game.promote[i] + c * Game.l;
		}

		// Index for enemy piece bitboard
		public int pieceEI (int c, int i)
		{
			return i + (c ^ 1) * Game.l;
		}
	}
}