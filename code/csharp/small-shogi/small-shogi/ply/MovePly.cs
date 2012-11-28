using System;

namespace smallshogi
{
	public class MovePly : IPly
	{
		// Colour of moving player
		int c;
		// Index piece moved and piece captured
		int mI, cI;
		// A bitboard for moving, promoting and capturing
		BitBoard piece, promo, capt;
		public MovePly (int c, int mI, BitBoard piece)
		{
			this.c = c;
			this.mI = mI;
			this.piece = piece;
			this.promo = new BitBoard();

			// Default value
			cI = -1;
		}

		void IPly.apply (BitBoard[] position)
		{
			// Move the piece in question
			position [pieceI  (c, mI)].And (piece);
			// Possibly promote it
			position [piecePI (c, mI)].And (promo);
			// If a piece is captured update hand and enemy piece
			if (cI >= 0) {
				position [pieceEI (c, cI)].And (capt);
				var mask = new BitBoard(Game.handMask[Game.demote[cI]]);
				position[handI(c)].PushMasked(mask);
			}
		}

		// Index for hand bitboard
		int handI (int c)
		{
			return 2 * Game.l + c;
		}

		// Index for piece bitboard
		int pieceI (int c, int i)
		{
			return i + c * Game.l;
		}

		// Index for promoted piece bitboard
		int piecePI (int c, int i) {
			return Game.promote[i] + c * Game.l;
		}

		// Index for enemy piece bitboard
		int pieceEI (int c, int i)
		{
			return i + ((c + 1) & 1) * Game.l;
		}
	}
}

